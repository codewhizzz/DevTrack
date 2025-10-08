using DevTrack.Application.Features.Users.Commands.RegisterUser;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace DevTrack.Application.Tests.Features.Users.Commands;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var command = new RegisterUserCommand { Email = "", Name = "John", Password = "Password123!" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var command = new RegisterUserCommand { Email = "test@test.com", Name = "John", Password = "Pass1!" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new RegisterUserCommand 
        { 
            Email = "test@test.com", 
            Name = "John Doe", 
            Password = "Password123!" 
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}