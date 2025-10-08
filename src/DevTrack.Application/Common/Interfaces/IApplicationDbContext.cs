using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevTrack.Domain.Common;

namespace DevTrack.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    List<BaseEntity> GetDomainEventEntities();
}