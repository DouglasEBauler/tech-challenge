namespace TechChallenge.Domain.Interfaces;

public interface ICommandResult
{
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}