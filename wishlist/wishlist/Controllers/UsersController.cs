using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Authenticate([FromBody]Users userParam)
        {
            var user = _userService.Authenticate(userParam.Username, userParam.Pswd);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            // authentication successful so generate jwt token
            user.Token = _jwtService.CreateJwtToken(user);

            // remove password before returning
            user.Pswd = null;

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]Users userParam)
        {
            var user = _userService.Create(userParam);

            if (user == null)
                return BadRequest(new { message = "Failed to register" });

            return Ok();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            // only allow admins to access other user records
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
            {
                return Forbid();
            }

            return Ok(user);
        }
    }
    //private readonly WishlistDBContext _context;

    //public UsersController(WishlistDBContext context)
    //{
    //    _context = context;
    //}

    //// GET: api/Users
    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
    //{
    //    return await _context.Users.ToListAsync();
    //}

    //// GET: api/Users/5
    //[HttpGet("{id}")]
    //public async Task<ActionResult<Users>> GetUsers(int id)
    //{
    //    var users = await _context.Users.FindAsync(id);

    //    if (users == null)
    //    {
    //        return NotFound();
    //    }

    //    return users;
    //}

    //[AllowAnonymous]
    //[HttpPost("authenticate")]
    //public IActionResult Authenticate([FromBody]User userParam)
    //{
    //    var user = _context.Authenticate(userParam.Username, userParam.Password);

    //    if (user == null)
    //        return BadRequest(new { message = "Username or password is incorrect" });

    //    return Ok(user);
    //}

    //// PUT: api/Users/5
    //[HttpPut("{id}")]
    //public async Task<IActionResult> PutUsers(int id, Users users)
    //{
    //    if (id != users.UserId)
    //    {
    //        return BadRequest();
    //    }

    //    _context.Entry(users).State = EntityState.Modified;

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (!UsersExists(id))
    //        {
    //            return NotFound();
    //        }
    //        else
    //        {
    //            throw;
    //        }
    //    }

    //    return NoContent();
    //}

    //// POST: api/Users
    //[HttpPost]
    //public async Task<ActionResult<Users>> PostUsers(Users users)
    //{
    //    _context.Users.Add(users);
    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateException)
    //    {
    //        if (UsersExists(users.UserId))
    //        {
    //            return Conflict();
    //        }
    //        else
    //        {
    //            throw;
    //        }
    //    }

    //    return CreatedAtAction("GetUsers", new { id = users.UserId }, users);
    //}

    //// DELETE: api/Users/5
    //[HttpDelete("{id}")]
    //public async Task<ActionResult<Users>> DeleteUsers(int id)
    //{
    //    var users = await _context.Users.FindAsync(id);
    //    if (users == null)
    //    {
    //        return NotFound();
    //    }

    //    _context.Users.Remove(users);
    //    await _context.SaveChangesAsync();

    //    return users;
    //}

    //private bool UsersExists(int id)
    //{
    //    return _context.Users.Any(e => e.UserId == id);
    //}
}
