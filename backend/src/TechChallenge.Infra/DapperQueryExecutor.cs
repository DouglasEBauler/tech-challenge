using Dapper;
using System.Data;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infra;

public class DapperQueryExecutor(IDbConnection db) : IDapperQueryExecutor
{
    private readonly IDbConnection _db = db;

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        => await _db.QueryAsync<T>(sql, param);

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        => await _db.QueryFirstOrDefaultAsync<T>(sql, param);
}
