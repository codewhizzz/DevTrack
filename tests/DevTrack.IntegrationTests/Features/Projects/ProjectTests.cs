using DevTrack.Application.Features.Projects.Commands.CreateProject;
using DevTrack.Application.Features.Users.Commands.RegisterUser;
using DevTrack.Domain.Entities;
using DevTrack.IntegrationTests.Testing;
using DevTrack.Application.Features.Projects.Queries.GetProjectById;
using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using static DevTrack.IntegrationTests.Testing.TestHelpers;

namespace DevTrack.IntegrationTests.Features.Projects;

public class ProjectTests : IntegrationTestBase
{
    public ProjectTests(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task<string> GetAuthToken()
    {
        var command = new RegisterUserCommand
        {
            Name = "Project Test User",
            Email = $"projecttest_{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!"
        };
        var response = await Client.PostAsync("/api/auth/register", GetStringContent(command));
        var result = await DeserializeResponse<RegisterUserResult>(response);
        return result.Token;
    }

    [Fact]
    public async Task CreateProject_WithValidData_ShouldCreateProject()
    {
        // Arrange
        var token = await GetAuthToken();
        SetAuthorizationHeader(token);

        var command = new CreateProjectCommand
        {
            Name = "Test Project",
            Description = "Test Description",
            Key = "TEST"
        };

        // Act
        var response = await Client.PostAsync("/api/projects", GetStringContent(command));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await DeserializeResponse<CreateProjectResult>(response);
        result.Id.Should().NotBeEmpty();
        result.Key.Should().Be("TEST");

        // Verify in database
        var project = await DbContext.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == result.Id);
            
        project.Should().NotBeNull();
        project!.Name.Should().Be(command.Name);
        project.Members.Should().HaveCount(1); // Creator should be added as member
        project.Members.First().Role.Should().Be(ProjectRole.Maintainer);
    }

    [Fact]
    public async Task CreateProject_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Name = "Unauthorized Project",
            Description = "Should fail",
            Key = "FAIL"
        };

        // Act
        var response = await Client.PostAsync("/api/projects", GetStringContent(command));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
public async Task GetProject_AsMember_ShouldReturnProject()
{
    // Arrange
    var token = await GetAuthToken();
    SetAuthorizationHeader(token);

    // Create project
    var createCommand = new CreateProjectCommand
    {
        Name = "Get Test Project",
        Description = "Test Description",
        Key = "GET"
    };
    var createResponse = await Client.PostAsync("/api/projects", GetStringContent(createCommand));
    var createResult = await DeserializeResponse<CreateProjectResult>(createResponse);

    // Act
    var response = await Client.GetAsync($"/api/projects/{createResult.Id}");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    
    // Use the actual DTO type instead of dynamic
    var project = await DeserializeResponse<ProjectDto>(response);
    project.Name.Should().Be(createCommand.Name);
    project.Key.Should().Be(createCommand.Key);
    project.Description.Should().Be(createCommand.Description);
}
}