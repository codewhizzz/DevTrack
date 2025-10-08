using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace DevTrack.Infrastructure.Persistence;

public static class SeedData
{
    public static void SeedRoles(ModelBuilder modelBuilder)
    {
        // Create role instances with explicit IDs for consistency
        var adminRoleId = Guid.Parse("a0000000-0000-0000-0000-000000000001");
        var maintainerRoleId = Guid.Parse("a0000000-0000-0000-0000-000000000002");
        var developerRoleId = Guid.Parse("a0000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<Role>().HasData(
            new { Id = adminRoleId, Name = "Admin", Description = "Full system access" },
            new { Id = maintainerRoleId, Name = "Maintainer", Description = "Can manage projects and users" },
            new { Id = developerRoleId, Name = "Developer", Description = "Can create and modify issues" }
        );
    }
}