using Microsoft.EntityFrameworkCore;
using MyFullStackAppApi.Data; 
using MyFullStackAppApi.Models;
using MyFullStackAppApi.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BCrypt.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString, 
        ServerVersion.AutoDetect(connectionString)
    ));

builder.Services.AddOpenApi();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var keyString = builder.Configuration["JwtSettings:Key"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,

            ValidateLifetime = true
        };
    });

builder.Services.AddProblemDetails();
var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/register", async (RegisterDto request, AppDbContext db) =>
{
    bool userExists = await db.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email);
    if (userExists)
    {
        return Results.Conflict(new { error = "Użytkownik o podanej nazwie lub e-mailu już istnieje." });
    }
    
    string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 8);
    
    var newUser = new User
    {
        Username = request.Username,
        Email = request.Email,
        IsActive = true
    };

    var newCredential = new Credential
    {
        PasswordHash = passwordHash,
        HashAlgorithm = "bcrypt"
    };
    
    newUser.Credential = newCredential;
    
    db.Users.Add(newUser);
    await db.SaveChangesAsync();

    return Results.Ok(new { message = "Użytkownik zarejestrowany pomyślnie." });
});

app.MapPost("api/login", async (LoginDto request, AppDbContext db, IConfiguration configuration) =>
{
    var user = await db.Users
        .Include(u => u.Credential)
        .FirstOrDefaultAsync(u => u.Email == request.Email);
    
    if (user == null || user.Credential == null || !user.IsActive)
    {
        return Results.Unauthorized();
    }
    
    bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, user.Credential.PasswordHash);
    
    if (!isPasswordCorrect)
    {
        return Results.Unauthorized(); 
    }
    
    var keyString = configuration["JwtSettings:Key"];
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email) 
    };
    
    var token = new JwtSecurityToken(
        issuer: configuration["JwtSettings:Issuer"],
        audience: configuration["JwtSettings:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(24), 
        signingCredentials: credentials);
    
    var tokenHandler = new JwtSecurityTokenHandler();
    var stringToken = tokenHandler.WriteToken(token);
    
    return Results.Ok(new { token = stringToken });
});

app.MapGet("api/user", async (ClaimsPrincipal userPrincipal, AppDbContext db) =>
{
    var userIdString = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (!ulong.TryParse(userIdString, out ulong userId))
    {
        return Results.Unauthorized();
    }
    
    var user = await db.Users.FindAsync(userId);
    
    if (user == null || !user.IsActive)
    {
        return Results.Forbid(); 
    }
    
    return Results.Ok(new 
    { 
        email = user.Email, 
        message = "Dostęp do chronionego zasobu przyznany." 
    });
}).RequireAuthorization();


app.Run();
