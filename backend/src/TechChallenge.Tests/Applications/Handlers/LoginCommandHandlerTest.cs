using Bogus;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechChallenge.Application.Commands.Login;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Interfaces;
using TechChallenge.JwtService;

namespace TechChallenge.Tests.Applications.Handlers;

public class LoginCommandHandlerTest
{
    private readonly Mock<IEmployeeDomainService> _domainServiceMock;
    private readonly Mock<IJwtSecurityService> _jwtServiceMock;
    private readonly LoginCommandHandler _handler;
    private readonly Faker _faker;

    public LoginCommandHandlerTest()
    {
        _domainServiceMock = new Mock<IEmployeeDomainService>();
        _jwtServiceMock = new Mock<IJwtSecurityService>();
        _handler = new LoginCommandHandler(_domainServiceMock.Object, _jwtServiceMock.Object);
        _faker = new Faker("en");
    }

    private LoginCommand CreateCommandMock()
    {
        return new LoginCommand(
            Email: _faker.Internet.Email(),
            Password: _faker.Internet.Password()
        );
    }

    [Fact(DisplayName = "Handler | Login | Should call DomainService, generate JWT, and return success result")]
    public async Task Should_CallDomainService_GenerateJwt_AndReturnSuccessResult()
    {
        var command = CreateCommandMock();
        var expectedToken = _faker.Random.Hash();

        var employeeMock = new EmployeeEntity
        {
            Id = _faker.Random.Int(1, 100),
            Email = command.Email
        };

        _domainServiceMock
            .Setup(x => x.LoginEmployeeAsync(command.Email, command.Password))
            .ReturnsAsync(employeeMock);

        _jwtServiceMock
            .Setup(x => x.GenerateToken(employeeMock))
            .Returns(expectedToken);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().Be(expectedToken);

        _domainServiceMock.Verify(x => x.LoginEmployeeAsync(
            command.Email,
            command.Password
        ), Times.Once);

        _jwtServiceMock.Verify(x => x.GenerateToken(employeeMock), Times.Once);
    }

    [Fact(DisplayName = "Handler | Login | Should return DomainService error or exception when credentials are invalid")]
    public async Task Should_ReturnError_WhenLoginFails()
    {
        var command = CreateCommandMock();

        _domainServiceMock
            .Setup(x => x.LoginEmployeeAsync(command.Email, command.Password))
            .ReturnsAsync((EmployeeEntity)null!);

        _domainServiceMock
            .Setup(x => x.LoginEmployeeAsync(command.Email, command.Password))
            .ThrowsAsync(new UnauthorizedAccessException("Credenciais inválidas."));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));

        _jwtServiceMock.Verify(x => x.GenerateToken(It.IsAny<EmployeeEntity>()), Times.Never);
    }
}
