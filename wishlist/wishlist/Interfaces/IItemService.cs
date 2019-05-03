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
        Task<ReturnObject<Item>> CreateAsync(Item item);
        Task<ReturnObject<Item>> EditAsync(int id, Item item);
        Task<ReturnObject<Item>> DeleteAsync(int id);
        ReturnObject<bool> IsListOwnerOrAdmin(int itemId, ClaimsPrincipal User);
    }
}
