using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TechChallenge.Api.Controllers;
using TechChallenge.Application.Commands.CreateEmployee;
using TechChallenge.Application.Commands.DeleteEmployee;
using TechChallenge.Application.Commands.UpdateEmployee;
using TechChallenge.Application.Queries.GetAllEmployes;
using TechChallenge.Application.Queries.GetEmployeeById;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.APIs;

public class EmployeeControllerTest
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Faker _faker;
    private readonly EmployeeController _controller;

    public EmployeeControllerTest()
    {
        _mediatorMock = new Mock<IMediator>();
        _faker = new Faker();
        _controller = new EmployeeController(_mediatorMock.Object);
    }

    private CreateEmployeeCommandResult CreateCommandResultMock()
    {
        return new CreateEmployeeCommandResult(
            true,
            _faker.Random.Int(1, 100)
        );
    }

    private GetEmployeeByIdCommandResult CreateEmployeeQueryResultMock(int id)
    {
        return new GetEmployeeByIdCommandResult(
            true,
            id,
            _faker.Person.FirstName,
            _faker.Person.LastName,
            _faker.Person.Email,
            _faker.Person.Random.Int(11111, 99999).ToString(),
            _faker.Person.DateOfBirth,
            EmployeeRoleType.User,
            [
                new("Mobile", _faker.Phone.PhoneNumberFormat())
            ]
        );
    }

    private GetAllEmployeeCommandResult CreateEmployeeAllQueryResultMock(int id)
    {
        return new GetAllEmployeeCommandResult(
            id,
            _faker.Person.FirstName,
            _faker.Person.LastName,
            _faker.Person.Email,
            _faker.Person.DateOfBirth,
            ["Mobile"],
            _faker.Person.FirstName + _faker.Person.LastName,
            EmployeeRoleType.User
        );
    }

    private GetAllCommandResult CreateAllEmployeesQueryResultMock()
    {
        var employees = new List<GetAllEmployeeCommandResult>
        {
            CreateEmployeeAllQueryResultMock(1),
            CreateEmployeeAllQueryResultMock(2),
            CreateEmployeeAllQueryResultMock(3)
        };
        return new GetAllCommandResult(true, employees);
    }


    [Fact(DisplayName = "EmployeeController | Create Employee | Should return 201 Created when successful")]
    public async Task Should_ReturnCreated_WhenCreateIsSuccessful()
    {
        var command = new CreateEmployeeCommand(
            _faker.Person.FirstName,
            _faker.Person.LastName,
            _faker.Person.Email,
            _faker.Person.Random.Int(11111, 99999).ToString(),
            _faker.Person.DateOfBirth,
            [
                new(PhoneType.Mobile, _faker.Phone.PhoneNumberFormat())
            ],
            EmployeeRoleType.User,
            _faker.Internet.Password()
        );
        var expectedResult = CreateCommandResultMock();

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var response = await _controller.Create(command);

        response.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedResult);

        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "EmployeeController | Update Employee | Should return 200 Ok when update is successful")]
    public async Task Should_ReturnOk_WhenUpdateIsSuccessful()
    {
        var employeeId = _faker.Random.Int(1, 100);
        var expectedResult = new UpdateEmployeeCommandResult(true);

        var command = new UpdateEmployeeCommand(
            employeeId,
            _faker.Person.FirstName,
            _faker.Person.LastName,
            _faker.Person.Email,
            _faker.Person.Random.Int(11111, 99999).ToString(),
            _faker.Person.DateOfBirth,
            EmployeeRoleType.User,
            [
                new(PhoneType.Mobile, _faker.Phone.PhoneNumberFormat())
            ]
        );

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var response = await _controller.Update(employeeId, command);

        response.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<UpdateEmployeeCommand>(c => c.EmployeeId == employeeId),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = "EmployeeController | Delete Employee | Should return 204 NoContent when successful")]
    public async Task Should_ReturnNoContent_WhenDeleteIsSuccessful()
    {
        var employeeId = _faker.Random.Int(1, 100);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteEmployeeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteEmployeeCommanResult(true));

        var response = await _controller.Delete(employeeId);

        response.Should().BeOfType<NoContentResult>();

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<DeleteEmployeeCommand>(c => c.EmployeeId == employeeId),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = "EmployeeController | Get By Id | Should return 200 Ok with employee data")]
    public async Task Should_ReturnOkWithEmployeeData_WhenGetByIdIsSuccessful()
    {
        var employeeId = _faker.Random.Int(1, 100);
        var expectedResult = CreateEmployeeQueryResultMock(employeeId);
        var query = new GetEmployeeByIdCommand(employeeId);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetEmployeeByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var response = await _controller.GetById(employeeId);

        response.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedResult);

        _mediatorMock.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "EmployeeController | Get All | Should return 200 Ok with employee list")]
    public async Task Should_ReturnOkWithEmployeeList_WhenGetAllIsSuccessful()
    {
        var expectedResult = CreateAllEmployeesQueryResultMock();
        var query = new GetAllCommand();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var response = await _controller.GetAll();

        response.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedResult);

        _mediatorMock.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }
}
