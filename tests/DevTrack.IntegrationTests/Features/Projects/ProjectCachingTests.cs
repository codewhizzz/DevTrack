using DevTrack.Application.Features.Projects.Commands.CreateProject;
using DevTrack.Application.Features.Users.Commands.RegisterUser;
using FluentAssertions;
using DevTrack.IntegrationTests.Testing;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using static DevTrack.IntegrationTests.Testing.TestHelpers;

namespace DevTrack.IntegrationTests.Features.Projects;

public class ProjectCachingTests : IntegrationTestBase
{
    public ProjectCachingTests(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetProject_SecondRequest_ShouldBeFasterDueToCaching()
    {
        // Arrange
        var token = await GetAuthToken();
        SetAuthorizationHeader(token);

        // Create project
        var createCommand = new CreateProjectCommand
        {
            Name = "Cache Test Project",
            Description = "Testing caching",
            Key = "CACHE"
        };
        var createResponse = await Client.PostAsync("/api/projects", GetStringContent(createCommand));
        var createResult = await DeserializeResponse<CreateProjectResult>(createResponse);

        // Act - First request (should hit database)
        var stopwatch1 = Stopwatch.StartNew();
        var response1 = await Client.GetAsync($"/api/projects/{createResult.Id}");
        stopwatch1.Stop();
        var firstRequestTime = stopwatch1.ElapsedMilliseconds;

        // Second request (should hit cache)
        var stopwatch2 = Stopwatch.StartNew();
        var response2 = await Client.GetAsync($"/api/projects/{createResult.Id}");
        stopwatch2.Stop();
        var secondRequestTime = stopwatch2.ElapsedMilliseconds;

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Second request should be significantly faster (at least 50% faster)
        secondRequestTime.Should().BeLessThan(firstRequestTime / 2);
    }

    private async Task<string> GetAuthToken()
    {
        var command = new RegisterUserCommand
        {
            Name = "Cache Test User",
            Email = $"cachetest_{Guid.NewGuid()}@example.com",
            Password = "TestPassword123!"
        };
        var response = await Client.PostAsync("/api/auth/register", GetStringContent(command));
        var result = await DeserializeResponse<RegisterUserResult>(response);
        return result.Token;
    }
}