using Bogus;
using Moq;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Exceptions;
using TechChallenge.Domain.Helpers;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Services;

namespace TechChallenge.Tests.Domains;

public class EmployeeDomainServiceTest
{
    private readonly Mock<IEmployeeQueryStore> _mockQueryStore;
    private readonly Mock<IEmployeePhoneQueryStore> _mockPhoneStore;
    private readonly Faker _faker;

    public EmployeeDomainServiceTest()
    {
        _mockQueryStore = new Mock<IEmployeeQueryStore>();
        _mockPhoneStore = new Mock<IEmployeePhoneQueryStore>();
        _faker = new Faker("pt_BR");
    }

    [Fact(DisplayName = "DomainService | CreateEmployee | Should throw when Leader tries to create Director")]
    public void CreateEmployee_LeaderTriesToCreateDirector_ShouldThrow()
    {
        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);
        var birthDate = _faker.Date.Past(30, DateTime.Now.AddYears(-18));

        var exception = Assert.Throws<DomainValidationException>(() =>
            service.CreateEmployee(
                firstName: _faker.Name.FirstName(),
                lastName: _faker.Name.LastName(),
                email: _faker.Internet.Email(),
                documentNumber: _faker.Random.ReplaceNumbers("###########"),
                password: "Test@123",
                birthDate: birthDate,
                employeeRoleType: EmployeeRoleType.Director,
                phones: [],
                userAuthRole: EmployeeRoleType.Leader,
                authEmployeeId: _faker.Random.Int(1, 100)
            )
        );

        Assert.Equal(ErrorType.HIGHER_PERMISSION, exception.ErrorType);
        Assert.Contains("higher or equal permissions", exception.Message);
    }

    [Fact(DisplayName = "DomainService | CreateEmployee | Should create employee when Leader creates Employee")]
    public void CreateEmployee_LeaderCreatesEmployee_ShouldSucceed()
    {
        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);
        var authEmployeeId = _faker.Random.Int(1, 100);
        var birthDate = _faker.Date.Past(30, DateTime.Now.AddYears(-18));

        var employee = service.CreateEmployee(
            firstName: _faker.Name.FirstName(),
            lastName: _faker.Name.LastName(),
            email: _faker.Internet.Email(),
            documentNumber: _faker.Random.ReplaceNumbers("###########"),
            password: "Test@123",
            birthDate: birthDate,
            employeeRoleType: EmployeeRoleType.User,
            phones: [],
            userAuthRole: EmployeeRoleType.Leader,
            authEmployeeId: authEmployeeId
        );

        Assert.NotNull(employee);
        Assert.Equal(EmployeeRoleType.User, employee.Role);
        Assert.Equal(authEmployeeId, employee.ManagerId);
    }

    [Fact(DisplayName = "DomainService | CreateEmployee | Should throw when trying to create user with equal permissions")]
    public void CreateEmployee_LeaderTriesToCreateLeader_ShouldThrow()
    {
        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var exception = Assert.Throws<DomainValidationException>(() =>
            service.CreateEmployee(
                firstName: _faker.Name.FirstName(),
                lastName: _faker.Name.LastName(),
                email: _faker.Internet.Email(),
                documentNumber: _faker.Random.ReplaceNumbers("###########"),
                password: "Test@123",
                birthDate: _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
                employeeRoleType: EmployeeRoleType.Leader,
                phones: [],
                userAuthRole: EmployeeRoleType.Leader,
                authEmployeeId: _faker.Random.Int(1, 100)
            )
        );

        Assert.Equal(ErrorType.HIGHER_PERMISSION, exception.ErrorType);
    }

    [Fact(DisplayName = "DomainService | DeleteEmployee | Should throw when trying to delete root administrator")]
    public async Task DeleteEmployee_TryToDeleteAdmin_ShouldThrow()
    {
        var adminEmployee = new Faker<EmployeeEntity>()
            .RuleFor(e => e.Id, f => f.Random.Int(1, 100))
            .RuleFor(e => e.ManagerId, _ => null)
            .RuleFor(e => e.Role, _ => EmployeeRoleType.Director)
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .Generate();

        _mockQueryStore
            .Setup(x => x.GetByIdAsync(adminEmployee.Id))
            .ReturnsAsync(adminEmployee);

        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var exception = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.DeleteEmployeeAsync(adminEmployee.Id, EmployeeRoleType.Director)
        );

        Assert.Equal(ErrorType.CANNOT_DELETE_ROOT_ADMIN, exception.ErrorType);
    }

    [Fact(DisplayName = "DomainService | DeleteEmployee | Should throw when trying to delete user with higher permissions")]
    public async Task DeleteEmployee_LeaderTriesToDeleteDirector_ShouldThrow()
    {
        var directorEmployee = new Faker<EmployeeEntity>()
            .RuleFor(e => e.Id, f => f.Random.Int(1, 100))
            .RuleFor(e => e.ManagerId, f => f.Random.Int(1, 100))
            .RuleFor(e => e.Role, _ => EmployeeRoleType.Director)
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .Generate();

        _mockQueryStore
            .Setup(x => x.GetByIdAsync(directorEmployee.Id))
            .ReturnsAsync(directorEmployee);

        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var exception = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.DeleteEmployeeAsync(directorEmployee.Id, EmployeeRoleType.Leader)
        );

        Assert.Equal(ErrorType.HIGHER_PERMISSION, exception.ErrorType);
    }

    [Fact(DisplayName = "DomainService | DeleteEmployee | Should delete employee when Leader deletes Employee")]
    public async Task DeleteEmployee_LeaderDeletesEmployee_ShouldSucceed()
    {
        var employeeToDelete = new Faker<EmployeeEntity>()
            .RuleFor(e => e.Id, f => f.Random.Int(1, 100))
            .RuleFor(e => e.ManagerId, f => f.Random.Int(1, 100))
            .RuleFor(e => e.Role, _ => EmployeeRoleType.User)
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .Generate();

        _mockQueryStore
            .Setup(x => x.GetByIdAsync(employeeToDelete.Id))
            .ReturnsAsync(employeeToDelete);

        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var result = await service.DeleteEmployeeAsync(employeeToDelete.Id, EmployeeRoleType.Leader);

        Assert.NotNull(result);
        Assert.Equal(employeeToDelete.Id, result.Id);
    }

    [Fact(DisplayName = "DomainService | GetAllAsync | Should throw when Employee tries to list")]
    public async Task GetAll_EmployeeTries_ShouldThrow()
    {
        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var exception = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.GetAllAsync(EmployeeRoleType.User, _faker.Random.Int(1, 100))
        );

        Assert.Equal(ErrorType.INSUFFICIENT_PERMISSIONS, exception.ErrorType);
        Assert.Contains("Only leaders and directors", exception.Message);
    }

    [Fact(DisplayName = "DomainService | GetAllAsync | Should return employees when Leader requests")]
    public async Task GetAll_LeaderRequests_ShouldReturnEmployees()
    {
        var employees = new Faker<EmployeeEntity>()
            .RuleFor(e => e.Id, f => f.IndexFaker + 1)
            .RuleFor(e => e.Role, _ => EmployeeRoleType.User)
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .RuleFor(e => e.BirthDate, f => f.Date.Past(30, DateTime.Now.AddYears(-18)))
            .Generate(5);

        var authEmployeeId = _faker.Random.Int(1, 100);

        _mockQueryStore
            .Setup(x => x.GetAllAsync(authEmployeeId))
            .ReturnsAsync(employees);

        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var result = await service.GetAllAsync(EmployeeRoleType.Leader, authEmployeeId);

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
    }

    [Fact(DisplayName = "DomainService | GetByIdAsync | Should throw when Employee tries to view details")]
    public async Task GetById_EmployeeTries_ShouldThrow()
    {
        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var exception = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.GetByIdAsync(_faker.Random.Int(1, 100), EmployeeRoleType.User)
        );

        Assert.Equal(ErrorType.INSUFFICIENT_PERMISSIONS, exception.ErrorType);
    }

    [Fact(DisplayName = "DomainService | GetByIdAsync | Should return employee with phones when Leader requests")]
    public async Task GetById_LeaderRequests_ShouldReturnEmployeeWithPhones()
    {
        var employee = new Faker<EmployeeEntity>()
            .RuleFor(e => e.Id, f => f.Random.Int(1, 100))
            .RuleFor(e => e.Role, _ => EmployeeRoleType.User)
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .Generate();

        var phones = new Faker<EmployeePhoneEntity>()
            .RuleFor(p => p.Number, f => f.Phone.PhoneNumber("(##) #####-####"))
            .RuleFor(p => p.Type, f => f.PickRandom<PhoneType>())
            .Generate(2);

        _mockQueryStore
            .Setup(x => x.GetByIdAsync(employee.Id))
            .ReturnsAsync(employee);

        _mockPhoneStore
            .Setup(x => x.GetPhonesByEmployeeId(employee.Id))
            .ReturnsAsync(phones);

        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var result = await service.GetByIdAsync(employee.Id, EmployeeRoleType.Leader);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
        Assert.NotNull(result.Phones);
        Assert.Equal(2, result.Phones.Count);
    }

    [Fact(DisplayName = "DomainService | LoginEmployee | Should throw when credentials are invalid")]
    public async Task Login_InvalidCredentials_ShouldThrow()
    {
        var email = _faker.Internet.Email();

        _mockQueryStore
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync((EmployeeEntity?)null);

        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var exception = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.LoginEmployeeAsync(email, _faker.Internet.Password())
        );

        Assert.Equal(ErrorType.INVALID_CREDENTIALS, exception.ErrorType);
    }

    [Fact(DisplayName = "DomainService | LoginEmployee | Should return employee when credentials are valid")]
    public async Task Login_ValidCredentials_ShouldReturnEmployee()
    {
        var password = "Test@123";
        var hashedPassword = PasswordHelper.CreateHashPassword(password);

        var employee = new Faker<EmployeeEntity>()
            .RuleFor(e => e.Id, f => f.Random.Int(1, 100))
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .RuleFor(e => e.Password, _ => hashedPassword)
            .RuleFor(e => e.Role, _ => EmployeeRoleType.User)
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())
            .Generate();

        _mockQueryStore
            .Setup(x => x.GetByEmailAsync(employee.Email))
            .ReturnsAsync(employee);

        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var result = await service.LoginEmployeeAsync(employee.Email, password);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
        Assert.Equal(employee.Email, result.Email);
    }

    [Fact(DisplayName = "DomainService | UpdateEmployee | Should throw when trying to update user with higher permissions")]
    public async Task UpdateEmployee_LeaderTriesToUpdateDirector_ShouldThrow()
    {
        var directorEmployee = new Faker<EmployeeEntity>()
            .RuleFor(e => e.Id, f => f.Random.Int(1, 100))
            .RuleFor(e => e.Role, _ => EmployeeRoleType.Director)
            .RuleFor(e => e.FirstName, f => f.Name.FirstName())
            .RuleFor(e => e.LastName, f => f.Name.LastName())
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .Generate();

        _mockQueryStore
            .Setup(x => x.GetByIdAsync(directorEmployee.Id))
            .ReturnsAsync(directorEmployee);

        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var exception = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.UpdateEmployee(
                employeeId: directorEmployee.Id,
                firstName: _faker.Name.FirstName(),
                lastName: _faker.Name.LastName(),
                email: _faker.Internet.Email(),
                birthDate: _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
                phones: [],
                documentNumber: _faker.Random.ReplaceNumbers("###########"),
                userAuthRole: EmployeeRoleType.Leader
            )
        );

        Assert.Equal(ErrorType.HIGHER_PERMISSION, exception.ErrorType);
    }

    [Fact(DisplayName = "DomainService | UpdateEmployee | Should update employee when Leader updates Employee")]
    public async Task UpdateEmployee_LeaderUpdatesEmployee_ShouldSucceed()
    {
        var employeeToUpdate = new Faker<EmployeeEntity>()
            .RuleFor(e => e.Id, f => f.Random.Int(1, 100))
            .RuleFor(e => e.FirstName, _ => "OldFirstName")
            .RuleFor(e => e.LastName, _ => "OldLastName")
            .RuleFor(e => e.Role, _ => EmployeeRoleType.User)
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .Generate();

        var newFirstName = _faker.Name.FirstName();
        var newLastName = _faker.Name.LastName();
        var newPhone = _faker.Phone.PhoneNumber("(##) #####-####");

        _mockQueryStore
            .Setup(x => x.GetByIdAsync(employeeToUpdate.Id))
            .ReturnsAsync(employeeToUpdate);

        var service = new EmployeeDomainService(_mockQueryStore.Object, _mockPhoneStore.Object);

        var result = await service.UpdateEmployee(
            employeeId: employeeToUpdate.Id,
            firstName: newFirstName,
            lastName: newLastName,
            email: _faker.Internet.Email(),
            birthDate: _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
            phones: new Dictionary<string, PhoneType>
            {
                { newPhone, PhoneType.Mobile }
            },
            documentNumber: _faker.Random.ReplaceNumbers("###########"),
            userAuthRole: EmployeeRoleType.Leader
        );

        Assert.NotNull(result);
        Assert.Equal(newFirstName, result.FirstName);
        Assert.Equal(newLastName, result.LastName);
        Assert.Single(result.Phones);
    }
}
