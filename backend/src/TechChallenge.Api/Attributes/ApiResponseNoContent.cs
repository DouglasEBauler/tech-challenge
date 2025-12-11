using Microsoft.AspNetCore.Mvc;

namespace TechChallenge.Api.Attributes;

public class ApiResponseNoContent() 
    : ProducesResponseTypeAttribute(StatusCodes.Status204NoContent);
