using Npgsql;
using Testcontainers.PostgreSql;

namespace MiniWebApp.UserApi.Test;

[Collection("ApiCollection")]
public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;

    public string ConnectionString => _container.GetConnectionString();

    public PostgresContainerFixture()
    {
        _container = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(false)
            .WithReuse(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        // 🔴 Drop existing schema safely
        const string resetSql = """
            DROP SCHEMA IF EXISTS public CASCADE;
            CREATE SCHEMA public;
            GRANT ALL ON SCHEMA public TO postgres;
            GRANT ALL ON SCHEMA public TO public;
            """;

        await using (var resetCmd = new NpgsqlCommand(resetSql, connection))
        {
            await resetCmd.ExecuteNonQueryAsync();
        }

        // 🟢 Apply schema
        var schemaSql = await File.ReadAllTextAsync("Schema/schema.sql");
        await using var cmd = new NpgsqlCommand(schemaSql, connection);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

[CollectionDefinition("ApiCollection")]
public sealed class ApiCollection
    : ICollectionFixture<PostgresContainerFixture>
{
}