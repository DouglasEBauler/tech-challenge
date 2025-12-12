using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Helpers;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Domain.Services;

public class EmployeeDomainService(IEmployeeQueryStore employeeQueryStore, IEmployeePhoneQueryStore employeePhoneQueryStore)
    : IEmployeeDomainService
{
    private readonly IEmployeeQueryStore _employeeQueryStore = employeeQueryStore;
    private readonly IEmployeePhoneQueryStore _employeePhoneQueryStore = employeePhoneQueryStore;

    public EmployeeEntity CreateEmployee(string firstName,
        string lastName,
        string email,
        string documentNumber,
        string password,
        DateTime birthDate,
        EmployeeRoleType employeeRoleType,
        Dictionary<string, PhoneType> phones,
        EmployeeRoleType userAuthRole,
        int authEmployeeId)
    {
        if (employeeRoleType > userAuthRole)
            throw new DomainValidationException("You cannot create a user with higher permissions", ErrorType.HIGHER_PERMISSION);

        var hashPassword = PasswordHelper.CreateHashPassword(password);
        var documentIndex = EncryptionHelper.CreateIndexHash(documentNumber);
        var documentEncrypted = EncryptionHelper.EncryptDocumentNumber(documentNumber);

        return new EmployeeEntity
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            DocumentNumber = documentEncrypted,
            DocumentNumberIndex = documentIndex,
            BirthDate = birthDate,
            ManagerId = authEmployeeId,
            Role = employeeRoleType,
            Password = hashPassword,
            Phones = [.. phones.Select(phone => new EmployeePhoneEntity
            {
                Number = phone.Key,
                Type = phone.Value
            })]
        };
    }

    public async Task<EmployeeEntity> UpdateEmployee(int employeeId, string firstName, string lastName, string email, DateTime birthDate, Dictionary<string, PhoneType> phones, string documentNumber, EmployeeRoleType userAuthRole)
    {
        var employeeUpdate = await _employeeQueryStore.GetByIdAsync(employeeId)
            ?? throw new DomainValidationException("Employee not found", ErrorType.INVALID_INPUT);

        if (employeeUpdate.Role > userAuthRole)
            throw new DomainValidationException("You cannot create a user with higher permissions", ErrorType.HIGHER_PERMISSION);

        var documentEncrypted = EncryptionHelper.EncryptDocumentNumber(documentNumber);
        var documentIndex = EncryptionHelper.CreateIndexHash(documentNumber);

        employeeUpdate.FirstName = firstName;
        employeeUpdate.LastName = lastName;
        employeeUpdate.Email = email;
        employeeUpdate.BirthDate = birthDate;
        employeeUpdate.DocumentNumber = documentEncrypted;
        employeeUpdate.DocumentNumberIndex = documentIndex;
        employeeUpdate.Phones = [.. phones.Select(phone => new EmployeePhoneEntity
        {
            Number = phone.Key,
            Type = phone.Value
        })];


        return employeeUpdate;
    }

    public async Task<EmployeeEntity> DeleteEmployeeAsync(int employeeId, EmployeeRoleType userAuthRole)
    {
        var employeeDelete = await _employeeQueryStore.GetByIdAsync(employeeId)
            ?? throw new DomainValidationException("Employee not found", ErrorType.INVALID_INPUT);

        if (employeeDelete.Role > userAuthRole)
            throw new DomainValidationException("You cannot create a user with higher permissions", ErrorType.HIGHER_PERMISSION);

        return employeeDelete;
    }

    public async Task<List<EmployeeEntity>> GetAllAsync(EmployeeRoleType userAuthRole, int authEmployeeId)
    {
        if (EmployeeRoleType.User >= userAuthRole)
            throw new DomainValidationException("You cannot create a user with higher permissions", ErrorType.HIGHER_PERMISSION);

        return await _employeeQueryStore.GetAllAsync(authEmployeeId)
            ?? throw new DomainValidationException("Employee not found", ErrorType.INVALID_INPUT);
    }

    public async Task<EmployeeEntity> GetByIdAsync(int employeeId, EmployeeRoleType userAuthRole)
    {
        if (EmployeeRoleType.User >= userAuthRole)
            throw new DomainValidationException("You cannot create a user with higher permissions", ErrorType.HIGHER_PERMISSION);

        var employee = await _employeeQueryStore.GetByIdAsync(employeeId)
            ?? throw new DomainValidationException("Employee not found", ErrorType.EMPLOYEE_NOT_FOUND);

        var employeePhones = await _employeePhoneQueryStore.GetPhonesByEmployeeId(employeeId)
            ?? throw new DomainValidationException($"No phones found by employee {employeeId}", ErrorType.EMPLOYEE_HAS_NO_FOUND_PHONES);

        employee.Phones = employeePhones;

        return employee;
    }

    public async Task<EmployeeEntity> LoginEmployeeAsync(string email, string password)
    {
        var employee = await _employeeQueryStore.GetByEmailAsync(email)
            ?? throw new DomainValidationException("Invalid credentials", ErrorType.INVALID_CREDENTIALS);

        if (!PasswordHelper.ValidatePassword(password, employee.Password))
            throw new DomainValidationException("Invalid credentials", ErrorType.INVALID_CREDENTIALS);

        return employee;
    }
}
