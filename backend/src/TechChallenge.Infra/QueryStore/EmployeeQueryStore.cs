using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Infra.QueryStore;

public class EmployeeQueryStore(IDapperQueryExecutor executor) : IEmployeeQueryStore
{
    private const string SQL_GET_ALL_EMPLOYEES = @"
        SELECT 
            ""ID"" as Id,
            ""FIRST_NAME"" as FirstName,
            ""LAST_NAME"" as LastName,
            ""EMAIL"" as Email,
            ""DOCUMENT_NUMBER"" as DocumentNumber,
            ""PASSWORD"" as Password,
            ""BIRTH_DATE"" as BirthDate,
            ""MANAGER_ID"" as ManagerId,
            ""ROLE"" as Role
        FROM ""EMPLOYEE""
       WHERE ""MANAGER_ID"" = @ManagerId;
    ";

    private const string SQL_GET_EMPLOYEE_BY_EMAIL = @"
        SELECT 
            ""ID"" as Id,
            ""FIRST_NAME"" as FirstName,
            ""LAST_NAME"" as LastName,
            ""EMAIL"" as Email,
            ""DOCUMENT_NUMBER"" as DocumentNumber,
            ""PASSWORD"" as Password,
            ""BIRTH_DATE"" as BirthDate,
            ""MANAGER_ID"" as ManagerId,
            ""ROLE"" as Role
        FROM ""EMPLOYEE""
        WHERE ""EMAIL"" = @Email;
    ";

    private const string SQL_GET_EMPLOYEE_BY_ID = @"
        SELECT 
            ""ID"" as Id,
            ""FIRST_NAME"" as FirstName,
            ""LAST_NAME"" as LastName,
            ""EMAIL"" as Email,
            ""DOCUMENT_NUMBER"" as DocumentNumber,
            ""PASSWORD"" as PasswordHash,
            ""BIRTH_DATE"" as BirthDate,
            ""MANAGER_ID"" as ManagerId,
            ""ROLE"" as Role
        FROM ""EMPLOYEE""
        WHERE ""ID"" = @Id;
    ";
    
    private const string SQL_EMAIL_EXISTS = @"
        SELECT EXISTS(
            SELECT 1 
              FROM ""EMPLOYEE"" 
             WHERE ""EMAIL"" = @Email
        );
    ";

    private const string SQL_DOCUMENT_NUMBER_EXISTS = @"
        SELECT EXISTS(
            SELECT 1 
              FROM ""EMPLOYEE"" 
             WHERE ""DOCUMENT_NUMBER_INDEX"" = @DocumentNumberIndex
        );
    ";

    private const string SQL_EMPLOYEE_EXISTS_BY_ID = @"
        SELECT EXISTS(
            SELECT 1 
              FROM ""EMPLOYEE"" 
             WHERE ""ID"" = @EmployeeId
        );
    ";

    private readonly IDapperQueryExecutor _executor = executor;

    public async Task<List<EmployeeEntity>?> GetAllAsync(int managerId)
    {
        var result = await _executor.QueryAsync<EmployeeEntity>(SQL_GET_ALL_EMPLOYEES, new { ManagerId = managerId });

        return [.. result];
    }        

    public async Task<EmployeeEntity?> GetByEmailAsync(string email)
        => await _executor.QueryFirstOrDefaultAsync<EmployeeEntity>(SQL_GET_EMPLOYEE_BY_EMAIL, new { Email = email });

    public async Task<EmployeeEntity?> GetByIdAsync(int id)
        => await _executor.QueryFirstOrDefaultAsync<EmployeeEntity>(SQL_GET_EMPLOYEE_BY_ID, new { Id = id });

    public async Task<bool> GetEmployeeExistsById(int employeeId)
        => await _executor.QueryFirstOrDefaultAsync<bool>(SQL_EMPLOYEE_EXISTS_BY_ID, new { EmployeeId = employeeId });

    public async Task<bool> GetEmailExists(string email)
        => await _executor.QueryFirstOrDefaultAsync<bool>(SQL_EMAIL_EXISTS, new { Email = email });

    public async Task<bool> GetDocumentNumberExists(string documentNumberIndex)
        => await _executor.QueryFirstOrDefaultAsync<bool>(SQL_DOCUMENT_NUMBER_EXISTS, new { DocumentNumberIndex = documentNumberIndex });
}
