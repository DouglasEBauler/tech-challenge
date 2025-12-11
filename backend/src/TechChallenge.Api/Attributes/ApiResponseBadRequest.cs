using Microsoft.AspNetCore.Mvc;
using TechChallenge.Application.Results;

namespace TechChallenge.Api.Attributes;

public class ApiResponseBadRequest() 
    : ProducesResponseTypeAttribute(typeof(CommandResult), StatusCodes.Status400BadRequest);
