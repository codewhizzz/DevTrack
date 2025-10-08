using DevTrack.Domain.Entities;
using System.Collections.Generic;

namespace DevTrack.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, IEnumerable<string> roles);
}