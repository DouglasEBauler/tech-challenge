using Bogus;
using FluentAssertions;
using TechChallenge.Application.Commands.Login;
using TechChallenge.Application.Validators;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Applications.Validators;

public class LoginValidatorTest
{
    private readonly LoginValidator _validator;
    private readonly Faker _faker;

    public LoginValidatorTest()
    {
        _validator = new LoginValidator();
        _faker = new Faker("en");
    }

    private LoginCommand CreateValidCommand()
    {
        return new LoginCommand(
            Email: _faker.Internet.Email(),
            Password: _faker.Internet.Password()
        );
    }

    [Fact(DisplayName = "Validator | Login | Should pass validation when Email and Password are provided")]
    public async Task Should_PassValidation_WhenCommandIsComplete()
    {
        var command = CreateValidCommand();

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory(DisplayName = "Validator | Login | Should fail when Email or Password are missing")]
    [InlineData(null, "ValidPassword", "Email is required.", nameof(LoginCommand.Email))]
    [InlineData("", "ValidPassword", "Email is required.", nameof(LoginCommand.Email))]
    [InlineData("valid@email.com", null, "Password is required.", nameof(LoginCommand.Password))]
    [InlineData("valid@email.com", "", "Password is required.", nameof(LoginCommand.Password))]
    public async Task Should_Fail_WhenRequiredFieldsAreMissing(
        string? email,
        string? password,
        string expectedMessage,
        string expectedField)
    {
        var command = new LoginCommand(email!, password!);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
              .Which.ErrorMessage.Should().Be(expectedMessage);
        result.Errors.Should().ContainSingle()
              .Which.PropertyName.Should().Be(expectedField);
        result.Errors.Should().ContainSingle()
              .Which.CustomState.Should().Be(ErrorType.INVALID_INPUT);
    }
}
