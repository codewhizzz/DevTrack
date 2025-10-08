using System;
using System.Collections.Generic;
using DevTrack.Domain.Common;
using DevTrack.Domain.Events;

namespace DevTrack.Domain.Entities;

public class Issue : BaseEntity 
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Key { get; private set; } // e.g., PROJ-123
    public int IssueNumber { get; private set; } // Sequential per project
    public string Title { get; private set; }
    public string Description { get; private set; }
    public IssueStatus Status { get; private set; }
    public IssuePriority Priority { get; private set; }
    public IssueType Type { get; private set; }
    
    public Guid ReporterId { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public Guid? SprintId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    private readonly List<Comment> _comments = new();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private readonly List<IssueLabel> _labels = new();
    public IReadOnlyCollection<IssueLabel> Labels => _labels.AsReadOnly();

    private Issue() { }

    public Issue(Guid projectId, string projectKey, int issueNumber, string title, string description, 
        IssuePriority priority, IssueType type, Guid reporterId)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        IssueNumber = issueNumber;
        Key = $"{projectKey}-{issueNumber}";
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description;
        Status = IssueStatus.Open;
        Priority = priority;
        Type = type;
        ReporterId = reporterId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string description, IssuePriority priority, IssueType type)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description;
        Priority = priority;
        Type = type;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignTo(Guid? userId)
    {
        var previousAssignee = AssigneeId;
        AssigneeId = userId;
        UpdatedAt = DateTime.UtcNow;

        // Raise domain event
        AddDomainEvent(new IssueAssignedDomainEvent(Id, Key, previousAssignee, userId));
    }

    public void UpdateStatus(IssueStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        if (newStatus == IssueStatus.Resolved || newStatus == IssueStatus.Closed)
        {
            ResolvedAt = DateTime.UtcNow;
        }
    }

    public void MoveToSprint(Guid? sprintId)
    {
        SprintId = sprintId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddComment(Comment comment)
    {
        _comments.Add(comment);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddLabel(Label label)
    {
        if (_labels.Any(l => l.LabelId == label.Id))
            return;

        _labels.Add(new IssueLabel(Id, label.Id));
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum IssueStatus
{
    Open,
    InProgress,
    InReview,
    Resolved,
    Closed,
    Reopened
}

public enum IssuePriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum IssueType
{
    Bug,
    Feature,
    Task,
    Epic,
    Story
}