using DevTrack.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Common.Interfaces;

public interface IIssueRepository
{
    Task<Issue> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Issue> GetByIdWithCommentsAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Issue issue, CancellationToken cancellationToken);
    void Update(Issue issue);
}