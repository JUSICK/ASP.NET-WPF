using System;
using System.Collections.Generic;

namespace MyFullStackAppApi.Models;

public partial class User
{
    public ulong Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 0 = no, 1 = yes
    /// </summary>
    public bool IsActive { get; set; }

    public virtual Credential? Credential { get; set; }
}
