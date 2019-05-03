using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wishlist.Models;
using wishlist.Helpers;

namespace wishlist.Interfaces
{
    public interface IListService
    {
        ReturnObject<List> GetById(int id, int page, int pageSize);
        ReturnObject<List> GetAll(int currentUserId, int page, int pageSize);
        Task<ReturnObject<List>> CreateAsync(List list, string userId);
        Task<ReturnObject<List>> EditAsync(int id, List list);
        Task<ReturnObject<List>> ShareAsync(int listId, int userId);
        Task<ReturnObject<List>> DeleteAsync(int id);
    }
}
