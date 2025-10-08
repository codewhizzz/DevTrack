using DevTrack.Application.Features.Users.Commands.RegisterUser;
using DevTrack.Application.Features.Users.Queries.LoginUser;
using DevTrack.IntegrationTests.Testing;
using System;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using static DevTrack.IntegrationTests.Testing.TestHelpers;

namespace DevTrack.IntegrationTests.Features.Authentication;

public class AuthenticationTests : IntegrationTestBase
{
    public AuthenticationTests(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "TestPassword123!"
        };

        // Act
        var response = await Client.PostAsync("/api/auth/register", GetStringContent(command));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await DeserializeResponse<RegisterUserResult>(response);
        result.UserId.Should().NotBeEmpty();
        result.Token.Should().NotBeNullOrEmpty();

        // Verify user exists in database
        var user = await DbContext.Users.FindAsync(result.UserId);
        user.Should().NotBeNull();
        user!.Email.Should().Be(command.Email);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var email = "duplicate@example.com";
        var command = new RegisterUserCommand
        {
            Name = "First User",
            Email = email,
            Password = "TestPassword123!"
        };

        // First registration
        await Client.PostAsync("/api/auth/register", GetStringContent(command));

        // Act - Try to register with same email
        var duplicateCommand = new RegisterUserCommand
        {
            Name = "Second User",
            Email = email,
            Password = "TestPassword123!"
        };
        var response = await Client.PostAsync("/api/auth/register", GetStringContent(duplicateCommand));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange - Register user first
        var registerCommand = new RegisterUserCommand
        {
            Name = "Login Test User",
            Email = "logintest@example.com",
            Password = "TestPassword123!"
        };
        await Client.PostAsync("/api/auth/register", GetStringContent(registerCommand));

        var loginQuery = new LoginUserQuery
        {
            Email = registerCommand.Email,
            Password = registerCommand.Password
        };

        // Act
        var response = await Client.PostAsync("/api/auth/login", GetStringContent(loginQuery));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await DeserializeResponse<LoginUserResult>(response);
        result.Token.Should().NotBeNullOrEmpty();
        result.Name.Should().Be(registerCommand.Name);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/test/protected");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_ShouldReturnOk()
    {
        // Arrange - Register and get token
        var registerCommand = new RegisterUserCommand
        {
            Name = "Protected Test User",
            Email = "protectedtest@example.com",
            Password = "TestPassword123!"
        };
        var registerResponse = await Client.PostAsync("/api/auth/register", GetStringContent(registerCommand));
        var registerResult = await DeserializeResponse<RegisterUserResult>(registerResponse);

        // Act
        SetAuthorizationHeader(registerResult.Token);
        var response = await Client.GetAsync("/api/test/protected");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}