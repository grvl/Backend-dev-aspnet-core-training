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
        ReturnObject<ListWithPaginatedItems> GetById(int id, ObjectPagination objectPagination);
        ReturnObject<PaginatedObject<List>> GetAll(int currentUserId, ObjectPagination objectPagination, SearchQuery searchQuery);
        ReturnObject<List> Create(List list, int userId);
        ReturnObject<List> Edit(int id, List list);
        ReturnObject<List> Share(int listId, int userId, bool editPermission);
        ReturnObject<List> Delete(int id);
    }
}
