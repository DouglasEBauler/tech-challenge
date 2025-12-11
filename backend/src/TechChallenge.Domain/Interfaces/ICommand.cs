using MediatR;

namespace TechChallenge.Domain.Interfaces;

public interface ICommand<TResponse> : IRequest<TResponse>
    where TResponse : ICommandResult;
