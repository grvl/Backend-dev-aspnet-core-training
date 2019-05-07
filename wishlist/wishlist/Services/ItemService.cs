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
        private readonly ContextMockHelper _propertyModifier;

        public ItemService(WishlistDBContext context, ContextMockHelper propertyModifier= null)
        {
            _context = context;
            _propertyModifier = propertyModifier == null? new ContextMockHelper() : propertyModifier;

        }

        public virtual ReturnObject<Item> Create(Item item)
        {
            if (ItemExists(item.ItemId))
            {
                return new ReturnObject<Item> { Message = "Item already exists." };
            }

            item.ItemId = 0;

            _context.Item.Add(item);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return new ReturnObject<Item> { Message = "Error saving item into the DB." };
            }

            return new ReturnObject<Item> {Value = item };
        }

        public ReturnObject<Item> Delete(int id)
        {
            var item =  _context.Item.FirstOrDefault(i => i.ItemId == id);
            if (item == null)
            {
                return new ReturnObject<Item> { Message = "Item not found." };
            }

            _context.Item.Remove(item);
            try
            {
                _context.SaveChanges();
            }
            
            catch (Exception)
            {
                return new ReturnObject<Item> { Message = "Error deleting item from the DB." };
            }

            return new ReturnObject<Item> { Value = null };
        }

        public ReturnObject<Item> Edit(int id, Item item)
        {
            if (id != item.ItemId)
            {
                return new ReturnObject<Item> { Message = "Invalid item ID." };
            }

            if (!ItemExists(id))
            {
                return new ReturnObject<Item> { Message = "Item not found." };
            }

            _propertyModifier.SetPropertyIsModified(_context, item);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return new ReturnObject<Item> { Message = "Error saving item into the DB." };
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


        public bool IsListOwnerOrAdmin(Item item, ClaimsPrincipal User)
        {
            var id = item.ListId;
            var currentUserId = int.Parse(User.Identity.Name);
            var userIsOwner = _context.UserList.Where(ul => ul.ListId == id && ul.UserId == currentUserId).FirstOrDefault() != null;
            return userIsOwner || User.IsInRole(Role.Admin) ;
        }

        public bool IsListOwnerOrAdmin(int itemId, ClaimsPrincipal User)
        {
            var item = _context.Item.Where(i => i.ItemId == itemId).FirstOrDefault();
            if (item == null)
            {
                return false;
            }

            return IsListOwnerOrAdmin(item, User);
        }

    }
}
