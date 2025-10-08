using DevTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Common.Interfaces;

public interface IProjectRepository
{
    Task<Project> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Project> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Project>> GetProjectsForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> IsUserMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken);
    Task<bool> ProjectKeyExistsAsync(string key, CancellationToken cancellationToken);
    Task<int> GetNextIssueNumberAsync(Guid projectId, CancellationToken cancellationToken);
    Task AddAsync(Project project, CancellationToken cancellationToken);
    void Update(Project project);
}