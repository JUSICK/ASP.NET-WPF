using System;
using System.Collections.Generic;

namespace MyFullStackAppApi.Models;

public partial class Credential
{
    public ulong Id { get; set; }

    public ulong UserId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string HashAlgorithm { get; set; } = null!;

    public DateTime LastChanged { get; set; }

    public virtual User User { get; set; } = null!;
}
