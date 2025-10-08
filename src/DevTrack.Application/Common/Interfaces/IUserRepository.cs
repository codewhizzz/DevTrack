using DevTrack.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Update(User user);
}