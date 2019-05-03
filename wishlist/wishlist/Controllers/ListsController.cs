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
    public class ListController : ControllerBase
    {
        private readonly WishlistDBContext _context;
        private readonly IListService _listService;

        public ListController(WishlistDBContext context, IListService listService)
        {
            _context = context;
            _listService = listService;
        }

        // GET: api/List
        [HttpGet("{page}/{pageSize}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [SwaggerOperation(
            Summary = "Get all lists owned or shared with the user.",
            Description = "Can't access a list of lists for another user."
        )]
        public ActionResult<List> GetList(int page, int pageSize)
        {
            var response =  _listService.GetAll(int.Parse(User.Identity.Name), page, pageSize);

            if (response.HasMessage())
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(response.Value);
        }

        // GET: api/List/5
        [HttpGet("{id}/page/pageSize")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [ProducesResponseType(statusCode: 403)]
        [SwaggerOperation(
            Summary = "Get information from a list.",
            Description = "Normal users can only access lists they own, while admins can access any list."
        )]
        public ActionResult<List> GetList(int id, int page, int pageSize)
        {
            if (!IsListOwnerOrAdmin(id))
            {
                return Forbid();
            }

            var response = _listService.GetById(id, page, pageSize);

            if (response.HasMessage())
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(response.Value);
        }

        // PUT: api/List/5
        [HttpPut("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [ProducesResponseType(statusCode: 403)]
        [SwaggerOperation(
            Summary = "Edit information from a list.",
            Description = "Normal users can only access lists they own, while admins can access any list."
        )]
        public async Task<ActionResult> PutList(int id, List List)
        {
            if (!IsListOwnerOrAdmin(id))
            {
                return Forbid();
            }


            var response = await _listService.EditAsync(id, List);

            if(response.HasMessage())
            {
                return BadRequest(new {message = response.Message});
            }

            return Ok(response.Value);
        }

        // PUT: api/List/5
        [HttpPut("{id}/{userId}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [ProducesResponseType(statusCode: 403)]
        [SwaggerOperation(
            Summary = "Share a list with another user.",
            Description = "Normal users can only share lists they own, while admins can share any list."
        )]
        public async Task<ActionResult> ShareList(int id, int userId)
        {
            if (!IsListOwnerOrAdmin(id))
            {
                return Forbid();
            }if (!IsListOwnerOrAdmin(id))
            {
                return Forbid();
            }


            var response = await _listService.ShareAsync(id, userId);

            if (response.HasMessage())
            {
                return BadRequest(new {message = response.Message});
            }

            return Ok(response.Value);
        }

        // POST: api/List
        [HttpPost]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [SwaggerOperation(
            Summary = "Create a new list.",
            Description = "The list will start with just it's creator as an owner, and more can be added with the share feature."
        )]
        public async Task<ActionResult> PostList(List List)
        {

            var response = await _listService.CreateAsync(List, User.Identity.Name);
            if (response.HasMessage())
            {
                return BadRequest(new {message = response.Message});
            }

            return Ok(response.Value);
        }

        // DELETE: api/List/5
        [HttpDelete("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [ProducesResponseType(statusCode: 403)]
        [SwaggerOperation(
            Summary = "Delete a list.",
            Description = "Normal users can only access lists they own, while admins can access any list."
        )]
        public async Task<ActionResult> DeleteList(int id)
        {
            if (!IsListOwnerOrAdmin(id))
            {
                return Forbid();
            }

            var response = await _listService.DeleteAsync(id);
            if (response.HasMessage())
            {
                return BadRequest(new {message = response.Message});
            }

            return NoContent();
        }

        private bool IsListOwnerOrAdmin(int id)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            var userIsOwner = _context.UserList.Where(ul => ul.ListId == id && ul.UserId == currentUserId).Count() > 0;
            return (userIsOwner || User.IsInRole(Role.Admin));
        }
    }
}
