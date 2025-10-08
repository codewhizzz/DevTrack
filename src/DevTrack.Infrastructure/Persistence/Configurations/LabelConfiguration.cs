using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Persistence.Configurations;

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(l => l.Color)
            .IsRequired()
            .HasMaxLength(7); // #RRGGBB

        builder.HasIndex(l => new { l.ProjectId, l.Name })
            .IsUnique();
    }
}

public class IssueLabelConfiguration : IEntityTypeConfiguration<IssueLabel>
{
    public void Configure(EntityTypeBuilder<IssueLabel> builder)
    {
        builder.HasKey(il => new { il.IssueId, il.LabelId });

        builder.HasOne<Label>()
            .WithMany()
            .HasForeignKey(il => il.LabelId);
    }
}