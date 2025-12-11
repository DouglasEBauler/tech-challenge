using Microsoft.AspNetCore.Mvc;

namespace TechChallenge.Api.Attributes;

public class ApiResponseCreated(Type type) 
    : ProducesResponseTypeAttribute(type, StatusCodes.Status201Created);
