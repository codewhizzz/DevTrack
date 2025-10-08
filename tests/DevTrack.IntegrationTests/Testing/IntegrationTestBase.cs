using DevTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Npgsql;

namespace DevTrack.IntegrationTests.Testing;

public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebApplicationFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly ApplicationDbContext DbContext;
    private Respawner? _respawner;
    private NpgsqlConnection? _dbConnection;

    protected IntegrationTestBase(IntegrationTestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected void SetAuthorizationHeader(string token)
    {
        Client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task InitializeAsync()
    {
        var connectionString = DbContext.Database.GetDbConnection().ConnectionString;
        _dbConnection = new NpgsqlConnection(connectionString);
        await _dbConnection.OpenAsync();
        
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" },
                TablesToIgnore = new Respawn.Graph.Table[] 
                { 
                    new("__EFMigrationsHistory"), 
                    new("Roles") 
                }
            });
    }

    public async Task DisposeAsync()
    {
        if (_respawner != null && _dbConnection != null)
        {
            await _respawner.ResetAsync(_dbConnection);
        }
        
        if (_dbConnection != null)
        {
            await _dbConnection.DisposeAsync();
        }
        
        Scope?.Dispose();
    }
}