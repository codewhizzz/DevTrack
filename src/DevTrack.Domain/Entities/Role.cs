using System;

namespace DevTrack.Domain.Entities;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    private Role() { }

    public Role(string name, string description = null)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
    }

    // Predefined roles
    public static Role Developer => new("Developer", "Can create and modify issues");
    public static Role Maintainer => new("Maintainer", "Can manage projects and users");
    public static Role Admin => new("Admin", "Full system access");
}