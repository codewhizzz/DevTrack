using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Persistence.Configurations;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.Key)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(i => i.Key)
            .IsUnique();
            
        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(i => i.Description)
            .HasMaxLength(2000);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(i => i.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(i => i.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Sprint>()
            .WithMany()
            .HasForeignKey(i => i.SprintId);

        builder.HasMany(i => i.Comments)
            .WithOne()
            .HasForeignKey(c => c.IssueId);

        builder.HasMany(i => i.Labels)
            .WithOne()
            .HasForeignKey(il => il.IssueId);
    }
}