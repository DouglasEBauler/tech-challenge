using System.Text.Json.Serialization;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Exceptions;

public class DomainValidationException(string? message, ErrorType errorType)
    : Exception(message)
{
    [JsonIgnore]
    public ErrorType ErrorType => errorType;
}
