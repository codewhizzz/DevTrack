using DevTrack.Application.Common.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevTrack.Application.Features.Users.Queries.LoginUser;

public record LoginUserQuery : IRequest<LoginUserResult>
{
    public string Email { get; init; }
    public string Password { get; init; }
}

public record LoginUserResult(Guid UserId, string Token, string Name);

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, LoginUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserQueryHandler(
        IUserRepository userRepository,
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginUserResult> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailWithRolesAsync(request.Email, cancellationToken);

        if (user == null || !user.IsActive || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        user.RecordLogin();
        _userRepository.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Get roles (simplified for now)
        var roles = user.UserRoles.Select(ur => "Developer").ToList();
        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        return new LoginUserResult(user.Id, token, user.Name);
    }
}