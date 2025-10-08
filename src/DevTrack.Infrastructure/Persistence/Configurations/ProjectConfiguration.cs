using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.Key)
            .IsRequired()
            .HasMaxLength(10);
            
        builder.HasIndex(p => p.Key)
            .IsUnique();
            
        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.HasMany(p => p.Members)
            .WithOne()
            .HasForeignKey(pm => pm.ProjectId);

        builder.HasMany(p => p.Issues)
            .WithOne()
            .HasForeignKey(i => i.ProjectId);

        builder.HasMany(p => p.Sprints)
            .WithOne()
            .HasForeignKey(s => s.ProjectId);
    }
}