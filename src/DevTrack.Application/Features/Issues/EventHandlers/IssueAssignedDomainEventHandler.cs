using DevTrack.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Features.Issues.EventHandlers;

public class IssueAssignedDomainEventHandler : INotificationHandler<IssueAssignedDomainEvent>
{
    private readonly ILogger<IssueAssignedDomainEventHandler> _logger;

    public IssueAssignedDomainEventHandler(ILogger<IssueAssignedDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(IssueAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Issue {IssueKey} was assigned from user {PreviousAssignee} to user {NewAssignee}", 
            notification.IssueKey,
            notification.PreviousAssigneeId?.ToString() ?? "unassigned",
            notification.NewAssigneeId?.ToString() ?? "unassigned");

        // TODO: In the future, send email notification here

        return Task.CompletedTask;
    }
}