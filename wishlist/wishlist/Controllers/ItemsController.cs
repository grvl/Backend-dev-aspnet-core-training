using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using wishlist.Helpers;
using wishlist.Interfaces;
using wishlist.Models;

namespace wishlist.Controllers
{
    [Route("api/wishlist/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly WishlistDBContext _context;

        public ItemController(WishlistDBContext context, IItemService itemService)
        {
            _itemService = itemService;
            _context = context;
        }

        // GET: api/Item/5
        [HttpGet("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [ProducesResponseType(statusCode: 403)]
        [SwaggerOperation(
            Summary = "Get an item.",
            Description = "Normal users can only access items in lists they own, while admins can access any item."
        )]
        public ActionResult<Item> GetItem(int id)
        {
            var permissionCheck = _itemService.IsListOwnerOrAdmin(id, User);
            if (!permissionCheck)
            {
                return Forbid();
            }

            var response = _itemService.GetById(id);

            if (response.HasMessage())
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(response.Value);
        }

        // PUT: api/Item/5
        [HttpPut("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [ProducesResponseType(statusCode: 403)]
        [SwaggerOperation(
            Summary = "Edit an item.",
            Description = "Normal users can only access items in lists they own, while admins can access any item."
        )]
        public ActionResult<Item> PutItem(int id, Item Item)
        {
            var permissionCheck = _itemService.IsListOwnerOrAdmin(Item, User);
            if (!permissionCheck)
            {
                return Forbid();
            }

            var response =  _itemService.Edit(id, Item);
            if (response.HasMessage())
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(response.Value);
        }

        // POST: api/Item
        [HttpPost]
        [ProducesResponseType(statusCode: 200, Type = typeof(Users))]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [ProducesResponseType(statusCode: 403)]
        [SwaggerOperation(
            Summary = "Create an item.",
            Description = "Normal users can only add items to lists they own, while admins can access any list."
        )]
        public  ActionResult<Item> PostItem(Item Item)
        {
            var permissionCheck = _itemService.IsListOwnerOrAdmin(Item, User);
            if (!permissionCheck)
            {
                return Forbid();
            }

            var response =  _itemService.Create(Item);
            if (response.HasMessage())
            {
                return BadRequest(new { message = response.Message });
            }

            var item = response.Value;

            return CreatedAtAction("GetItem", new { id = item.ItemId }, item);
        }

        // DELETE: api/Item/5
        [HttpDelete("{id}")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 400, Type = typeof(string))]
        [ProducesResponseType(statusCode: 403)]
        [SwaggerOperation(
            Summary = "Delete an item.",
            Description = "Normal users can only access items in lists they own, while admins can access any item."
        )]
        public ActionResult<Item> DeleteItem(int id)
        {
            var permissionCheck = _itemService.IsListOwnerOrAdmin(id, User);
            
            if (permissionCheck) { 
                return Forbid();
            }

            var response =  _itemService.Delete(id);
            if (response.HasMessage())
            {
                return BadRequest(new { message = response.Message });
            }

            return NoContent();

        }

    }
}
