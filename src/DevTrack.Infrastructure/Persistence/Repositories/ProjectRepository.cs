using DevTrack.Application.Common.Interfaces;
using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Project> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Projects
            .Include(p => p.Members)
            .Where(p => p.Members.Any(m => m.UserId == userId) && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        return await _context.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId, cancellationToken);
    }

    public async Task<bool> ProjectKeyExistsAsync(string key, CancellationToken cancellationToken)
    {
        return await _context.Projects
            .AnyAsync(p => p.Key == key.ToUpperInvariant(), cancellationToken);
    }

    public async Task<int> GetNextIssueNumberAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var lastIssueNumber = await _context.Issues
            .Where(i => i.ProjectId == projectId)
            .MaxAsync(i => (int?)i.IssueNumber, cancellationToken) ?? 0;

        return lastIssueNumber + 1;
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
    }

    public void Update(Project project)
    {
        _context.Projects.Update(project);
    }
}