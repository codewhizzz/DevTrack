using System;

namespace DevTrack.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string UserName { get; }
    bool IsAuthenticated { get; }
}