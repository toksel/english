using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace English;

public class DataAccess : IDataAccess
{
    private readonly ILogger<DataAccess> _logger;
    private IDbConnection? _connection;
    private readonly string _connectionString;

    public DataAccess(ILogger<DataAccess> logger, IConfiguration config)
    {
        _logger = logger;
        _connectionString = config.GetConnectionString("Db");
    }
    
    public async Task<IEnumerable<Phrase>> GetEntities(int count)
    {
        var statement = $"select top {count} * from dbo.Phrases order by Rank asc, newid()";
        return await EnsureConnection().QueryAsync<Phrase>(statement, new { count });
    }

    public async Task UpdateEntities(IEnumerable<Phrase> phrases)
    {
        var statement = $"update ph set ph.Rank = @Rank from dbo.Phrases as ph where ph.Id = @Id";
        await EnsureConnection().ExecuteAsync(statement, phrases);
    }

    public async Task InsertEntity(Phrase phrase)
    {
        var statement = "insert into dbo.Phrases(Rank, Text, Reference) values (@Rank, @Text, @Reference)";
        await EnsureConnection().ExecuteAsync(statement, phrase);
    }

    private IDbConnection EnsureConnection()
    {
        if (_connection is null || _connection.State == ConnectionState.Closed)
        {
            _connection = new SqlConnection(_connectionString);
        }
        return _connection;
    }

    public void Dispose() => _connection?.Dispose();
}