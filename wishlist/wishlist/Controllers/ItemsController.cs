using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wishlist.Models;

namespace wishlist.Controllers
{
    [Route("api/wishlist/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly WishlistDBContext _context;

        public ItemController(WishlistDBContext context)
        {
            _context = context;
        }

        // GET: api/Item
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItem()
        {
            return await _context.Item.ToListAsync();
        }

        // GET: api/Item/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var Item = await _context.Item.FindAsync(id);

            if (Item == null)
            {
                return NotFound();
            }

            return Item;
        }

        // PUT: api/Item/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, Item Item)
        {
            if (id != Item.ItemId)
            {
                return BadRequest();
            }

            _context.Entry(Item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Item
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item Item)
        {
            _context.Item.Add(Item);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ItemExists(Item.ItemId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetItem", new { id = Item.ItemId }, Item);
        }

        // DELETE: api/Item/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Item>> DeleteItem(int id)
        {
            var Item = await _context.Item.FindAsync(id);
            if (Item == null)
            {
                return NotFound();
            }

            _context.Item.Remove(Item);
            await _context.SaveChangesAsync();

            return Item;
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.ItemId == id);
        }
    }
}
