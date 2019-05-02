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
    public class ListsController : ControllerBase
    {
        private readonly WishlistDBContext _context;

        public ListsController(WishlistDBContext context)
        {
            _context = context;
        }

        // GET: api/Lists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lists>>> GetLists()
        {
            return await _context.Lists.ToListAsync();
        }

        // GET: api/Lists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lists>> GetLists(int id)
        {
            var lists = await _context.Lists.FindAsync(id);

            if (lists == null)
            {
                return NotFound();
            }

            return lists;
        }

        // PUT: api/Lists/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLists(int id, Lists lists)
        {
            if (id != lists.ListId)
            {
                return BadRequest();
            }

            _context.Entry(lists).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListsExists(id))
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

        // POST: api/Lists
        [HttpPost]
        public async Task<ActionResult<Lists>> PostLists(Lists lists)
        {
            _context.Lists.Add(lists);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ListsExists(lists.ListId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLists", new { id = lists.ListId }, lists);
        }

        // DELETE: api/Lists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Lists>> DeleteLists(int id)
        {
            var lists = await _context.Lists.FindAsync(id);
            if (lists == null)
            {
                return NotFound();
            }

            _context.Lists.Remove(lists);
            await _context.SaveChangesAsync();

            return lists;
        }

        private bool ListsExists(int id)
        {
            return _context.Lists.Any(e => e.ListId == id);
        }
    }
}
