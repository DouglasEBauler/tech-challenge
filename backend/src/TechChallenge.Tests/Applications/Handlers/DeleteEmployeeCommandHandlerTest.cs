using Bogus;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Commands.DeleteEmployee;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Tests.Applications.Handlers;

public class DeleteEmployeeCommandHandlerTest
{
    private readonly Mock<IEmployeeDomainService> _domainServiceMock;
    private readonly Mock<IEmployeeCommandStore> _commandStoreMock;
    private readonly DeleteEmployeeCommandHandler _handler;
    private readonly Faker _faker;

    public DeleteEmployeeCommandHandlerTest()
    {
        _domainServiceMock = new Mock<IEmployeeDomainService>();
        _commandStoreMock = new Mock<IEmployeeCommandStore>();
        _handler = new DeleteEmployeeCommandHandler(_domainServiceMock.Object, _commandStoreMock.Object);
        _faker = new Faker();
    }

    private static DeleteEmployeeCommand CreateCommandMock(int employeeId)
        => new(employeeId);

    [Fact(DisplayName = "Handler | Delete Employee | Should call domain service, persist deletion, and return Ok")]
    public async Task Should_CallDomainService_PersistDeletion_AndReturnOk()
    {
        var employeeToDeleteId = _faker.Random.Int(1, 99);
        var command = CreateCommandMock(employeeToDeleteId);

        var employeeMock = new EmployeeEntity { Id = employeeToDeleteId };

        _domainServiceMock
            .Setup(x => x.DeleteEmployeeAsync(command.EmployeeId, command.AuthRole))
            .ReturnsAsync(employeeMock);

        _commandStoreMock
            .Setup(x => x.DeleteAsync(employeeMock))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        _domainServiceMock.Verify(x => x.DeleteEmployeeAsync(
            employeeToDeleteId,
            It.IsAny<EmployeeRoleType>()
        ), Times.Once);
        
        _commandStoreMock.Verify(x => x.DeleteAsync(employeeMock), Times.Once);
    }
}
