using Bogus;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Commands.CreateEmployee;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Tests.Applications.Handlers;

public class CreateEmployeeCommandHandlerTest
{
    private readonly Mock<IEmployeeDomainService> _domainServiceMock;
    private readonly Mock<IEmployeeCommandStore> _commandStoreMock;
    private readonly CreateEmployeeCommandHandler _handler;
    private readonly Faker _faker;

    public CreateEmployeeCommandHandlerTest()
    {
        _domainServiceMock = new Mock<IEmployeeDomainService>();
        _commandStoreMock = new Mock<IEmployeeCommandStore>();
        _handler = new CreateEmployeeCommandHandler(_domainServiceMock.Object, _commandStoreMock.Object);
        _faker = new Faker("pt_BR");
    }

    private CreateEmployeeCommand CreateCommandMock()
    {
        return new CreateEmployeeCommand(
            FirstName: _faker.Person.FirstName,
            LastName: _faker.Person.LastName,
            Email: _faker.Internet.Email(),
            DocumentNumber: _faker.Person.Random.Int(11111, 99999).ToString(),
            BirthDate: _faker.Date.Past(25),
            Phones:
            [
                new (PhoneType.Mobile, _faker.Phone.PhoneNumber()),
            ],
            EmployeeRoleType.Director,
            _faker.Internet.Password()
        );
    }

    [Fact(DisplayName = "Handler | Create Employee | Should call domain service, persist and return created Id")]
    public async Task Should_CallDomainService_PersistEmployee_AndReturnCreatedId()
    {
        var command = CreateCommandMock();
        var createdEmployeeId = _faker.Random.Int(1, 99);
        var employeeMock = new EmployeeEntity { Id = createdEmployeeId };

        _domainServiceMock
            .Setup(x => x.CreateEmployee(
                command.FirstName,
                command.LastName,
                command.Email,
                command.DocumentNumber,
                command.Password,
                command.BirthDate,
                command.Role,
                It.IsAny<Dictionary<string, PhoneType>>(),
                command.AuthRole,
                command.AuthEmployeeId
            ))
            .Returns(employeeMock);

        _commandStoreMock
            .Setup(x => x.AddAsync(employeeMock))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.EmployeeId.Should().Be(createdEmployeeId);

        _domainServiceMock.Verify(x => x.CreateEmployee(
            command.FirstName,
            command.LastName,
            command.Email,
            command.DocumentNumber,
            command.Password,
            command.BirthDate,
            command.Role,
            It.Is<Dictionary<string, PhoneType>>(dict => 
                dict.SequenceEqual(command.Phones.ToDictionary(p => p.Number, p => p.Type))),
            command.AuthRole,
            command.AuthEmployeeId
        ), Times.Once);

        _commandStoreMock.Verify(x => x.AddAsync(employeeMock), Times.Once);
    }
}
