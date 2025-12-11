using Bogus;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Commands.UpdateEmployee;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Tests.Applications.Handlers;

public class UpdateEmployeeCommandHandlerTest
{
    private readonly Mock<IEmployeeDomainService> _domainServiceMock;
    private readonly Mock<IEmployeeCommandStore> _commandStoreMock;
    private readonly UpdateEmployeeCommandHandler _handler;
    private readonly Faker _faker;

    public UpdateEmployeeCommandHandlerTest()
    {
        _domainServiceMock = new Mock<IEmployeeDomainService>();
        _commandStoreMock = new Mock<IEmployeeCommandStore>();
        _handler = new UpdateEmployeeCommandHandler(_domainServiceMock.Object, _commandStoreMock.Object);
        _faker = new Faker("pt_BR");
    }

    private UpdateEmployeeCommand CreateCommandMock(int employeeId, EmployeeRoleType authRole)
    {
        var oldestReferenceDate = DateTime.UtcNow.AddYears(-18).AddDays(-1);
        var youngestReferenceDate = DateTime.UtcNow.AddYears(-25);

        return new UpdateEmployeeCommand(
            EmployeeId: employeeId,
            FirstName: _faker.Person.FirstName,
            LastName: _faker.Person.LastName,
            Email: _faker.Internet.Email(),
            _faker.Person.Random.Int(11111, 99999).ToString(),
            _faker.Date.Between(youngestReferenceDate, oldestReferenceDate),
            EmployeeRoleType.User,
            Phones:
            [
                new (PhoneType.Mobile, _faker.Phone.PhoneNumber()),
            ]
        );
    }

    [Fact(DisplayName = "Handler | Update Employee | Should call domain service, persist update, and return Ok")]
    public async Task Should_CallDomainService_PersistUpdate_AndReturnOk()
    {
        var employeeToUpdateId = _faker.Random.Int(1, 99);
        var authRole = EmployeeRoleType.Director;
        var command = CreateCommandMock(employeeToUpdateId, authRole);

        var updatedEmployeeMock = new EmployeeEntity
        {
            Id = employeeToUpdateId,
            FirstName = command.FirstName
        };

        _domainServiceMock
            .Setup(x => x.UpdateEmployee(
                command.EmployeeId,
                command.FirstName,
                command.LastName,
                command.Email,
                command.BirthDate,
                It.IsAny<Dictionary<string, PhoneType>>(),
                command.DocumentNumber,
                command.AuthRole
            ))
            .ReturnsAsync(updatedEmployeeMock);

        _commandStoreMock
            .Setup(x => x.UpdateAsync(updatedEmployeeMock))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        _domainServiceMock.Verify(x => x.UpdateEmployee(
            command.EmployeeId,
            command.FirstName,
            command.LastName,
            command.Email,
            command.BirthDate,
            It.Is<Dictionary<string, PhoneType>>(dict => dict.SequenceEqual(command.Phones.ToDictionary(p => p.Number, p => p.Type))),
            command.DocumentNumber,
            command.AuthRole
        ), Times.Once);

        _commandStoreMock.Verify(x => x.UpdateAsync(updatedEmployeeMock), Times.Once);
    }
}
