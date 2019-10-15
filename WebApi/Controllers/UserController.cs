using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Token.Common.Interfaces;
using Users.Common.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private IUserService _userService;
        private ITokenService _tokenService;

        public UserController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginRequest login)
        {
            if (!ModelState.IsValid) return BadRequestResponse(ModelState.ToString());

            var authenticateResponse = await _userService.AuthenticateAsync(login.Username, login.Password);
            if (!authenticateResponse.Success)
                return HandleAuthenticationFailure(authenticateResponse);

            var user = authenticateResponse.Data;
            if (user == null) return NotFoundResponse("User not found");


            var tokenResponse = await _tokenService.GenerateTokenSetAsync(user.Username);
            if (!tokenResponse.Success)
                return HandleServiceFailure(tokenResponse);

            return OkResponse(tokenResponse.Data);

        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody]RefreshTokenRequest request)
        {
            var token = parseTorken(HttpContext);
            if (string.IsNullOrEmpty(token)) return UnauthorizedResponse();

            var refreshResponse = await _tokenService
                .RefreshUserTokenAsync(token, request.RefreshToken);
            if (!refreshResponse.Success)
                return HandleServiceFailure(refreshResponse);

            return OkResponse(refreshResponse.Data);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate()
        {
            var token = parseTorken(HttpContext);
            if (string.IsNullOrEmpty(token)) return UnauthorizedResponse();

            var validationResponse = await _tokenService.ValidateTokenAsync(token);
            if (!validationResponse.Success)
                return HandleAuthenticationFailure(validationResponse);

            var userName = validationResponse.Data.Identity.Name;

            return OkResponse(userName);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var token = parseTorken(HttpContext);

            var validation = await _tokenService.ValidateTokenAsync(token);
            if (!validation.Success)
                return HandleAuthenticationFailure(validation);

            var getUserResponse = await _userService.GetAllUsersAsync();
            if (!getUserResponse.Success)
                return HandleServiceFailure(getUserResponse);

            return OkResponse(getUserResponse.Data);
        }

        private string parseTorken(HttpContext httpContext)
        {
            StringValues authHeader;
            var success = HttpContext.Request.Headers.TryGetValue("Authorization", out authHeader);
            var bearerToken = authHeader.ToString();

            if (
                string.IsNullOrEmpty(bearerToken) ||
                !bearerToken.Contains("Bearer ")
              )
                return null;

            return bearerToken.Split(' ')[1];
        }

        private IActionResult HandleServiceFailure(IAppWrapper response)
        {
            Console.WriteLine(response.Exception.Message);
            if (response.Exception.InnerException != null)
                Console.WriteLine(response.Exception.InnerException.Message);
            return this.BadRequestResponse(response.Message);
        }

        private IActionResult HandleAuthenticationFailure(IAppWrapper response)
        {
            Console.WriteLine(response.Exception.Message);
            if (response.Exception.InnerException != null)
                Console.WriteLine(response.Exception.InnerException.Message);
            return this.UnauthorizedResponse();
        }
    }
}
