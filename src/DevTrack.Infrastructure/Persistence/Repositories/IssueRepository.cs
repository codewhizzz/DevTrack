using DevTrack.Application.Common.Interfaces;
using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Infrastructure.Persistence.Repositories;

public class IssueRepository : IIssueRepository
{
    private readonly ApplicationDbContext _context;

    public IssueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Issue> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Issues
            .Include(i => i.Labels)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<Issue> GetByIdWithCommentsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Issues
            .Include(i => i.Comments)
            .Include(i => i.Labels)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task AddAsync(Issue issue, CancellationToken cancellationToken)
    {
        await _context.Issues.AddAsync(issue, cancellationToken);
    }

    public void Update(Issue issue)
    {
        _context.Issues.Update(issue);
    }
}