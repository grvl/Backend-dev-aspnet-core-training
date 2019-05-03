using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using wishlist.Helpers;
using wishlist.Interfaces;
using wishlist.Models;

namespace wishlist.Services
{
    public class ItemService : IItemService
    {
        private readonly WishlistDBContext _context;

        public ItemService(WishlistDBContext context)
        {
            _context = context;
        }

        public async Task<ReturnObject<Item>> CreateAsync(Item item)
        {

            _context.Item.Add(item);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (ItemExists(item.ItemId))
                {
                    return new ReturnObject<Item> { Message = "Item already exists." };
                }
                else
                {
                    return new ReturnObject<Item> { Message = "Error saving item into the DB." };
                }
            }
            return new ReturnObject<Item> {Value = item };
        }

        public async Task<ReturnObject<Item>> DeleteAsync(int id)
        {
            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return new ReturnObject<Item> { Message = "Item not found." };
            }

            _context.Item.Remove(item);
            await _context.SaveChangesAsync();

            return new ReturnObject<Item> { Value = item };
        }

        public async Task<ReturnObject<Item>> EditAsync(int id, Item item)
        {
            if (id != item.ItemId)
            {
                return new ReturnObject<Item> { Message = "Invalid item ID." };
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!ItemExists(id))
                {
                    return new ReturnObject<Item> { Message = "Item not found." };
                }
                else
                {
                    return new ReturnObject<Item> { Message = "Error saving item into the DB." };
                }
            }

            return new ReturnObject<Item> { Value = item };
        }

        public ReturnObject<Item> GetById(int id)
        {
            var item = _context.Item.Where(i => i.ItemId == id).Include(i => i.List).FirstOrDefault();

            if (item == null)
            {
                return new ReturnObject<Item> { Message = "Item not found" } ;
            }

           return new ReturnObject<Item> { Value = item } ;
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.ItemId == id);
        }


        public ReturnObject<bool> IsListOwnerOrAdmin(int itemId, ClaimsPrincipal User)
        {
            var item = _context.Item.Where(i => i.ItemId == itemId).FirstOrDefault();
            if (item == null)
            {
                return new ReturnObject<bool> { Message = "Item not found" };
            }

            var id = item.ListId;
            var currentUserId = int.Parse(User.Identity.Name);
            var userIsOwner = _context.UserList.Where(ul => ul.ListId == id && ul.UserId == currentUserId).Count() > 0;
            return new ReturnObject<bool> { Value = (userIsOwner || User.IsInRole(Role.Admin)) };
        }

    }
}
