namespace TechChallenge.Domain.Interfaces;

public interface IDapperQueryExecutor
{
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null);
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
}
