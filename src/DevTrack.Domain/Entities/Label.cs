using System;

namespace DevTrack.Domain.Entities;

public class Label
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; }
    public string Color { get; private set; } // Hex color code

    private Label() { }

    public Label(Guid projectId, string name, string color)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Color = color ?? "#808080"; // Default gray
    }
}

public class IssueLabel
{
    public Guid IssueId { get; private set; }
    public Guid LabelId { get; private set; }

    private IssueLabel() { }

    public IssueLabel(Guid issueId, Guid labelId)
    {
        IssueId = issueId;
        LabelId = labelId;
    }
}