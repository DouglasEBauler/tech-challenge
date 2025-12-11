using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TechChallenge.Api.Controllers;
using TechChallenge.Application.Commands.Login;

namespace TechChallenge.Tests.APIs;

public class LoginControllerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Faker _faker;
    private readonly LoginController _controller;

    public LoginControllerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _faker = new Faker();
        _controller = new LoginController(_mediatorMock.Object);
    }

    private LoginCommandResult CreateSuccessfulLoginResultMock()
    {
        return new LoginCommandResult(
            true,
            _faker.Random.Hash()
        );
    }

    private static LoginCommandResult CreateFailedLoginResultMock()
    {
        return new LoginCommandResult(
            false,
            string.Empty
        );
    }

    [Fact(DisplayName = "LoginController | Login | Should return 200 Ok with token when credentials are valid")]
    public async Task Should_ReturnOkWithToken_WhenLoginIsSuccessful()
    {
        var command = new LoginCommand(
            _faker.Internet.Email(),
            _faker.Internet.Password()
        );
        var expectedResult = CreateSuccessfulLoginResultMock();

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var response = await _controller.Login(command);

        response.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedResult);

        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "LoginController | Login | Should return 400 Bad Request when credentials are invalid")]
    public async Task Should_ReturnBadRequest_WhenLoginFails()
    {
        var command = new LoginCommand(
            _faker.Internet.Email(),
            _faker.Internet.Password()
        );
        var failedResult = CreateFailedLoginResultMock();

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResult);

        var response = await _controller.Login(command);

        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(failedResult);

        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}
