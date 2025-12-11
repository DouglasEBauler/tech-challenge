using Microsoft.AspNetCore.Mvc;

namespace TechChallenge.Api.Attributes;

public class ApiResponseOk(Type type) 
    : ProducesResponseTypeAttribute(type, StatusCodes.Status200OK);
