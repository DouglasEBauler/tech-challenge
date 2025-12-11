using Bogus;
using FluentAssertions;
using Moq;
using TechChallenge.Application.Queries.GetAllEmployes;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Tests.Applications.Queries
{
    public class GetAllCommandHandlerTest
    {
        private readonly Mock<IEmployeeDomainService> _domainServiceMock;
        private readonly GetAllCommandHandler _handler;
        private readonly Faker _faker;

        public GetAllCommandHandlerTest()
        {
            _domainServiceMock = new Mock<IEmployeeDomainService>();
            _handler = new GetAllCommandHandler(_domainServiceMock.Object);
            _faker = new Faker("pt_BR");
        }

        private List<EmployeeEntity> CreateEmployeeListMock(int count)
        {
            return [.. Enumerable.Range(1, count).Select(i => new EmployeeEntity
            {
                Id = i,
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                Email = _faker.Internet.Email(),
                BirthDate = _faker.Date.Past(25),
                Role = EmployeeRoleType.User,
                Phones =
                [
                    new() { Number = _faker.Phone.PhoneNumber(), Type = PhoneType.Mobile },
                    new() { Number = _faker.Phone.PhoneNumber(), Type = PhoneType.Landline }
                ],
                Manager = new EmployeeEntity
                {
                    FirstName = "Gerente",
                    LastName = $" {i}"
                }
            })];
        }

        [Fact(DisplayName = "Handler | Get All Employees | Should retrieve data and map all properties correctly")]
        public async Task Should_RetrieveData_AndMapAllPropertiesCorrectly()
        {
            var authEmployeeId = 100;
            var command = new GetAllCommand();
            command = command with
            {
                AuthRole = EmployeeRoleType.Director,
                AuthEmployeeId = 100
            };

            var employeesMocked = CreateEmployeeListMock(2);

            _domainServiceMock
                .Setup(x => x.GetAllAsync(command.AuthRole, command.AuthEmployeeId))
                .ReturnsAsync(employeesMocked);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Employees.Should().HaveCount(2);

            var firstEmployeeResult = result.Employees.First(e => e.EmployeeId == 1);
            var firstEmployeeMock = employeesMocked.First(e => e.Id == 1);

            firstEmployeeResult.EmployeeId.Should().Be(firstEmployeeMock.Id);
            firstEmployeeResult.FirstName.Should().Be(firstEmployeeMock.FirstName);
            firstEmployeeResult.LastName.Should().Be(firstEmployeeMock.LastName);
            firstEmployeeResult.Email.Should().Be(firstEmployeeMock.Email);
            firstEmployeeResult.BirthDate.Should().Be(firstEmployeeMock.BirthDate);
            firstEmployeeResult.Role.Should().Be(firstEmployeeMock.Role);

            firstEmployeeResult.ManagerName.Should().Be("Gerente 1");

            firstEmployeeResult.Phones.Should().HaveCount(2);
            firstEmployeeResult.Phones.Should().Contain(firstEmployeeMock.Phones.First().Number);

            _domainServiceMock.Verify(x => x.GetAllAsync(
                EmployeeRoleType.Director,
                authEmployeeId
            ), Times.Once);
        }

        [Fact(DisplayName = "Handler | Get All Employees | Should return an empty list when no employees are found")]
        public async Task Should_ReturnEmptyList_WhenNoEmployeesAreFound()
        {
            var command = new GetAllCommand();

            _domainServiceMock
                .Setup(x => x.GetAllAsync(It.IsAny<EmployeeRoleType>(), It.IsAny<int>()))
                .ReturnsAsync([]);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Employees.Should().BeEmpty();

            _domainServiceMock.Verify(x => x.GetAllAsync(It.IsAny<EmployeeRoleType>(), It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Handler | Get All Employees | Should handle null Manager property gracefully")]
        public async Task Should_HandleNullManagerGracefully()
        {
            var command = new GetAllCommand();

            var employeesMocked = new List<EmployeeEntity>
            {
                new() 
                {
                    Id = 50,
                    FirstName = "Test",
                    Phones = [],
                    Manager = null,
                }
            };

            _domainServiceMock
                .Setup(x => x.GetAllAsync(It.IsAny<EmployeeRoleType>(), It.IsAny<int>()))
                .ReturnsAsync(employeesMocked);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Employees.Should().HaveCount(1);

            var employeeResult = result.Employees.First();
            employeeResult.ManagerName.Should().BeNullOrEmpty();
        }
    }
}
