using Bogus;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Queries.GetEmployeeById;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Helpers;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Tests.Applications.Queries;

public class GetEmployeeByIdCommandHandlerTest
{
    private readonly Mock<IEmployeeDomainService> _domainServiceMock;
    private readonly GetEmployeeByIdCommandHandler _handler;
    private readonly Faker _faker;

    public GetEmployeeByIdCommandHandlerTest()
    {
        _domainServiceMock = new Mock<IEmployeeDomainService>();
        _handler = new GetEmployeeByIdCommandHandler(_domainServiceMock.Object);
        _faker = new Faker("pt_BR");
    }

    private static GetEmployeeByIdCommand CreateCommandMock(int employeeId)
        => new(Id: employeeId);

    private EmployeeEntity CreateEmployeeMock(int id, string plainDocumentNumber)
    {
        return new EmployeeEntity
        {
            Id = id,
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            Email = _faker.Internet.Email(),
            DocumentNumber = $"{ "ENCRYPTED_" + EncryptionHelper.EncryptDocumentNumber(plainDocumentNumber)}",
            BirthDate = _faker.Date.Past(30),
            Role = EmployeeRoleType.User,
            Phones =
            [
                new() { Number = _faker.Phone.PhoneNumber(), Type = PhoneType.Mobile },
                new() { Number = _faker.Phone.PhoneNumber(), Type = PhoneType.Landline }
            ]
        };
    }

    [Fact(DisplayName = "Handler | Get Employee By Id | Should retrieve data, decrypt document number, and map correctly")]
    public async Task Should_RetrieveData_DecryptDocumentNumber_AndMapCorrectly()
    {
        var employeeId = _faker.Random.Int(1, 100);
        var authRole = EmployeeRoleType.Admin;
        var plainDocumentNumber = _faker.Person.Random.Int(1111111, 9999999).ToString();
        var command = CreateCommandMock(employeeId) with 
        { 
            AuthRole = authRole
        };

        var employeeMocked = CreateEmployeeMock(employeeId, plainDocumentNumber);

        _domainServiceMock
            .Setup(x => x.GetByIdAsync(command.Id, command.AuthRole))
            .ReturnsAsync(employeeMocked);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        result.EmployeeId.Should().Be(employeeMocked.Id);
        result.FirstName.Should().Be(employeeMocked.FirstName);
        result.Email.Should().Be(employeeMocked.Email);

        result.DocumentNumber.Should().Be(plainDocumentNumber);
        result.DocumentNumber.Should().NotContain("ENCRYPTED_");

        result.Phones.Should().HaveCount(2);

        var firstPhoneResult = result.Phones.First();
        var firstPhoneMock = employeeMocked.Phones.First();

        firstPhoneResult.Type.Should().Be(firstPhoneMock.Type.ToString());
        firstPhoneResult.Number.Should().Be(firstPhoneMock.Number);


        _domainServiceMock.Verify(x => x.GetByIdAsync(
            command.Id,
            command.AuthRole
        ), Times.Once);
    }

    [Fact(DisplayName = "Handler | Get Employee By Id | Should propagate exception when employee is not found")]
    public async Task Should_PropagateException_WhenEmployeeNotFound()
    {
        var employeeId = 999;
        var command = CreateCommandMock(employeeId) with
        {
            AuthRole = EmployeeRoleType.User
        };

        _domainServiceMock
            .Setup(x => x.GetByIdAsync(command.Id, command.AuthRole))
            .ThrowsAsync(new Exception("Employee not found by Domain Service"));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

        _domainServiceMock.Verify(x => x.GetByIdAsync(
            command.Id,
            command.AuthRole
        ), Times.Once);
    }
}
