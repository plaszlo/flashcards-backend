using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace FlashcardBackend.Data;

public class DataContextDapper
{
    private readonly IConfiguration _config;

    private readonly string _connectionString;

    public DataContextDapper(IConfiguration config)
    {
        _config = config;
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<T?> LoadDataSingleAsync<T>(string sql, object? parameters = null)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);

        return await dbConnection.QuerySingleOrDefaultAsync<T>(sql, parameters);
    }

    public async Task<IEnumerable<T>> LoadDataAsync<T>(string sql, object? parameters = null)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);

        return await dbConnection.QueryAsync<T>(sql, parameters);
    }

    public async Task<bool> ExecuteSqlAsync(string sql, object? parameters = null)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);
        
        return await dbConnection.ExecuteAsync(sql, parameters) > 0;
    }
}