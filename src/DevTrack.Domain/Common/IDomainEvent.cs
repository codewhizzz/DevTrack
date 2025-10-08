using MediatR;
using System;

namespace DevTrack.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}