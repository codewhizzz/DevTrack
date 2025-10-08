using DevTrack.Domain.Common;
using System;

namespace DevTrack.Domain.Events;

public class IssueAssignedDomainEvent : IDomainEvent
{
    public Guid IssueId { get; }
    public string IssueKey { get; }
    public Guid? PreviousAssigneeId { get; }
    public Guid? NewAssigneeId { get; }
    public DateTime OccurredOn { get; }

    public IssueAssignedDomainEvent(
        Guid issueId, 
        string issueKey,
        Guid? previousAssigneeId, 
        Guid? newAssigneeId)
    {
        IssueId = issueId;
        IssueKey = issueKey;
        PreviousAssigneeId = previousAssigneeId;
        NewAssigneeId = newAssigneeId;
        OccurredOn = DateTime.UtcNow;
    }
}