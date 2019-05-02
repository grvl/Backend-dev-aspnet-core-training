using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wishlist.Models;

namespace wishlist.Controllers
{
    [Authorize]
    [Route("api/wishlist/[controller]")]
    [ApiController]
    public class ListController : ControllerBase
    {
        private readonly WishlistDBContext _context;

        public ListController(WishlistDBContext context)
        {
            _context = context;
        }

        // GET: api/List
        [HttpGet]
        public async Task<ActionResult<IEnumerable<List>>> GetList()
        {
            return await _context.List.ToListAsync();
        }

        // GET: api/List/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List>> GetList(int id)
        {
            var List = await _context.List.FindAsync(id);

            if (List == null)
            {
                return NotFound();
            }

            return List;
        }

        // PUT: api/List/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutList(int id, List List)
        {
            if (id != List.ListId)
            {
                return BadRequest();
            }

            _context.Entry(List).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListExists(id))
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

        // POST: api/List
        [HttpPost]
        public async Task<ActionResult<List>> PostList(List List)
        {
            _context.List.Add(List);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ListExists(List.ListId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetList", new { id = List.ListId }, List);
        }

        // DELETE: api/List/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<List>> DeleteList(int id)
        {
            var List = await _context.List.FindAsync(id);
            if (List == null)
            {
                return NotFound();
            }

            _context.List.Remove(List);
            await _context.SaveChangesAsync();

            return List;
        }

        private bool ListExists(int id)
        {
            return _context.List.Any(e => e.ListId == id);
        }
    }
}
