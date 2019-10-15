using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    public class BaseController : Controller
    {
        protected OkObjectResult OkResponse<T>(T data)
        {
            OkResponseModel<T> wrappedReposne = new OkResponseModel<T>(data);

            return Ok(new OkResponseModel<T>(data));
        }

        protected BadRequestObjectResult BadRequestResponse(string error)
        {
            return BadRequest(new BadRequestResponseModel(error));
        }

        protected BadRequestObjectResult BadRequestResponse(IEnumerable<string> errors)
        {
            return BadRequest(new BadRequestResponseModel(errors));
        }

        protected UnauthorizedObjectResult UnauthorizedResponse()
        {
            return Unauthorized(new UnauthorizedResponseModel());
        }

        protected NotFoundObjectResult NotFoundResponse(string error)
        {
            return NotFound(new NotFoundResponseModel(error));
        }
    }
}