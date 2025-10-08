using DevTrack.Application.Common.Interfaces;
using DevTrack.Domain.Common;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Common.Behaviours;

public class DomainEventBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DomainEventBehaviour(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        await DispatchDomainEvents(cancellationToken);

        return response;
    }

    private async Task DispatchDomainEvents(CancellationToken cancellationToken)
    {
        var domainEntities = _context.GetDomainEventEntities();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}