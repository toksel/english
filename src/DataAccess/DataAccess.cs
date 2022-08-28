using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Data;

public class DataAccess : IDataAccess
{
    private IDbConnection? _connection;
    private readonly string _connectionString;

    public DataAccess(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<IEnumerable<PhraseDto>> GetPhrases(int count)
    {
        var statement = $"select top {count} * from dbo.Phrases order by Rank asc, newid()";
        return await EnsureConnection().QueryAsync<PhraseDto>(statement, new { count });
    }

    public async Task UpdatePhrases(IEnumerable<PhraseDto> phrases)
    {
        var statement = $"update ph set ph.Rank = @Rank from dbo.Phrases as ph where ph.Id = @Id";
        await EnsureConnection().ExecuteAsync(statement, phrases);
    }

    public async Task InsertPhrase(PhraseDto phrase)
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