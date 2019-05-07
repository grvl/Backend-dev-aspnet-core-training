using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using wishlist.Helpers;
using wishlist.Models;

namespace wishlist.Interfaces
{
    public interface IItemService
    {
        ReturnObject<Item> GetById(int id);
        ReturnObject<Item> Create(Item item);
        ReturnObject<Item> Edit(int id, Item item);
        ReturnObject<Item> Delete(int id);
        bool IsListOwnerOrAdmin(Item item, ClaimsPrincipal User);
        bool IsListOwnerOrAdmin(int itemId, ClaimsPrincipal User);
    }
}
