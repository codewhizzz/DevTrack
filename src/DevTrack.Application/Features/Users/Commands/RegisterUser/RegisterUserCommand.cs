using DevTrack.Application.Common.Interfaces;
using DevTrack.Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<RegisterUserResult>
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}

public record RegisterUserResult(Guid UserId, string Token);

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);
        if (emailExists)
            throw new InvalidOperationException("User with this email already exists");

        // Create new user
        var user = new User(request.Name, request.Email);
        user.SetPassword(_passwordHasher.HashPassword(request.Password));

        // Get default role (Developer)
        var developerRole = await _roleRepository.GetByNameAsync("Developer", cancellationToken);
        if (developerRole != null)
            user.AddRole(developerRole);

        await _userRepository.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate token
        var roles = user.UserRoles.Select(ur => "Developer").ToList(); // Simplified for now
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        return new RegisterUserResult(user.Id, token);
    }
}