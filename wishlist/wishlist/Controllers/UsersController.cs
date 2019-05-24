using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using wishlist.Helpers;
using wishlist.Interfaces;
using wishlist.Models;
using wishlist.Services;

namespace wishlist.Controllers
{
    [Authorize]
    [Route("api/wishlist/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UsersController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [SwaggerOperation(
            Summary = "Authenticates a user and returns a jwt token.",
            Description = "Doen't require authentication."
        )]
        public IActionResult Authenticate([FromBody]Users userParam)
        {
            var response = _userService.Authenticate(userParam.Username, userParam.Pswd);

            if (response.HasMessage())
                return BadRequest(new { message = response.Message });

            Users user = response.Value;
            // authentication successful so generate jwt token
            user.Token = _jwtService.CreateJwtToken(user);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [SwaggerOperation(
            Summary = "Create a user but doesn't log them in.",
            Description = "Doesn't require authentication."
        )]
        public IActionResult Register([FromBody]Users userParam)
        {
            var response = _userService.Create(userParam);

            if (response.HasMessage())
                return BadRequest(new { message = response.Message});

            return Ok(response.Value);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [SwaggerOperation(
            Summary = "Get information from all users.",
            Description = "Only admin role can use this command."
        )]
        public IActionResult GetAll()
        {
            var response = _userService.GetAll();

            if (response.HasMessage())
                return BadRequest(new { message = response.Message });

            return Ok(response.Value);
        }

        [HttpGet("search")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [SwaggerOperation(
            Summary = "Search for usernames.",
            Description = "To facilitate sharing lists."
        )]
        public IActionResult SearchUser([FromQuery]SearchQuery searchUser, [FromQuery]ObjectPagination objectPagination)
        {
            var response = _userService.Search(searchUser, objectPagination);

            if (response.HasMessage())
                return BadRequest(new { message = response.Message });

            return Ok(response.Value);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [ProducesResponseType(statusCode: 403)]
        [SwaggerOperation(
            Summary = "Get information from a user.",
            Description = "Only admin role can get information from another user."
        )]
        public IActionResult GetById(int id)
        {
            var response = _userService.GetById(id);

            if (response.HasMessage())
            {
                return BadRequest(new { message = response.Message });
            }

            // only allow admins to access other user records
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
            {
                return Forbid();
            }

            return Ok(response.Value);
        }
    }
}
