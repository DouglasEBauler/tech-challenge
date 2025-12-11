using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infra.QueryStore;

public class EmployeePhoneQueryStore(IDapperQueryExecutor executor) : IEmployeePhoneQueryStore
{
    private const string SQL_GET_ALL_PHONES_BY_EMPLOYEE_ID = @"
        SELECT 
            ""ID"" as Id,
            ""TYPE"" as Type,
            ""NUMBER"" as Number,
            ""EMPLOYEE_ID"" as EmployeeId
        FROM ""EMPLOYEE_PHONE""
       WHERE ""EMPLOYEE_ID"" = @EmployeeId;
    ";

    private readonly IDapperQueryExecutor _executor = executor;

    public async Task<List<EmployeePhoneEntity>?> GetPhonesByEmployeeId(int employeeId)
    {
        var result = await _executor.QueryAsync<EmployeePhoneEntity>(SQL_GET_ALL_PHONES_BY_EMPLOYEE_ID, new { EmployeeId = employeeId });

        return [.. result];
    }
}
