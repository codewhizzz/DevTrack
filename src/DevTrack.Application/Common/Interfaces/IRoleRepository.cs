using DevTrack.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Common.Interfaces;

public interface IRoleRepository
{
    Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken);
}