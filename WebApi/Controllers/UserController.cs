using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        public IActionResult Authenticate([FromBody] UserLoginRequest login)
        {
            if (!ModelState.IsValid) return BadRequestResponse(ModelState.ToString());

            var user = _userService.Authenticate(login.Username, login.Password);
            if (user == null) return BadRequestResponse("Invalid Credentials");

            var tokenSet = _tokenService.GenerateTokenSet(user.Username);

            return OkResponse(tokenSet);

        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody]RefreshTokenRequest request)
        {
            StringValues authHeader;
            var success = HttpContext.Request.Headers.TryGetValue("Authorization", out authHeader);
            var bearerToken = authHeader.ToString();

            if (
                string.IsNullOrEmpty(bearerToken) ||
                !bearerToken.Contains("Bearer ")
              )
                return UnauthorizedResponse();

            var token = bearerToken.Split(' ')[1];

            var newToken = _tokenService.RefreshUserToken(token, request.RefreshToken);

            return OkResponse(newToken);
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<string>> Get()
        {
            var users = _userService.GetAllUsers();
            return OkResponse(users);
        }
    }
}
