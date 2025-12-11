using MediatR;
using Microsoft.AspNetCore.Http;
using TechChallenge.Application.Extensions;
using TechChallenge.Domain.Interfaces;

namespace TechChallenge.Application.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : ICommandResult
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IAuthCommand<TResponse> authenticated)
        {
            var employeeRoleType = _httpContextAccessor.HttpContext?.User.GetEmployeeRole();

            if (!employeeRoleType.HasValue || employeeRoleType == 0)
            {
                var fail = Activator.CreateInstance(typeof(TResponse), false, "USER_UNAUTHORIZED", null);
                return (TResponse)fail!;
            }

            var employeeId = _httpContextAccessor.HttpContext?.User.GetEmployeeId();
            
            if (!employeeId.HasValue || employeeId == 0)
            {
                var fail = Activator.CreateInstance(typeof(TResponse), false, "USER_UNAUTHORIZED", null);
                return (TResponse)fail!;
            }

            authenticated.AuthRole = employeeRoleType.Value;
            authenticated.AuthEmployeeId = employeeId.Value;
        }

        return await next(cancellationToken);
    }
}
