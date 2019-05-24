using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wishlist.Helpers;
using wishlist.Interfaces;
using wishlist.Models;

namespace wishlist.Services
{
    public class ListService : IListService
    {
        private readonly WishlistDBContext _context;
        private readonly ContextMockHelper _mockhelper;

        public ListService(WishlistDBContext context, ContextMockHelper mockHelper = null)
        {
            _context = context;
            _mockhelper = mockHelper == null ? new ContextMockHelper() : mockHelper;
        }

        public ReturnObject<List> Create(List list, int userId)
        {     
            try
            {
                //Add to the List and UserList DBs
                _mockhelper.RunStoredProcedure(_context, userId, list);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return new ReturnObject<List> { Message = "An error occured while trying to save the new list." };
            }

            var created = _context.List.OrderByDescending(l => l.ListId).FirstOrDefault();

            if (created == null)
            {
                return new ReturnObject<List> { Message = "Failed to retrieve the new list." };
            }

            return new ReturnObject<List> { Value = created };
        }

        public ReturnObject<List> Delete(int id)
        {
            var items = _context.Item.Where(i => i.ListId == id);
            var userList = _context.UserList.Where(ul => ul.ListId == id);
            var list = _context.List.FirstOrDefault(l => l.ListId == id);
            if (list == null)
            {
                return new ReturnObject<List> { Message = "List not found." };
            }

            if (userList.Count() == 0)
            {
                _context.Item.RemoveRange(items);
                _context.List.Remove(list);
                _context.SaveChanges();
                return new ReturnObject<List> { Message = "List without owner." };
            }

            _context.Item.RemoveRange(items);
            _context.UserList.RemoveRange(userList);
            _context.List.Remove(list);
            _context.SaveChanges();

            return new ReturnObject<List> ();
        }

        public ReturnObject<List> Edit(int id, List list)
        {
            if (id != list.ListId)
            {
                return new ReturnObject<List> { Message = "Invalid list Id." };
            }

            if (!ListExists(id))
            {
                return new ReturnObject<List> { Message = "List not found." };
            }

            _mockhelper.SetPropertyIsModified(_context, list);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return new ReturnObject<List> { Message = "Error saving list into the DB." };
            }

            return new ReturnObject<List> { Value = list };
        }

        public ReturnObject<PaginatedObject<List>> GetAll(int currentUserId, ObjectPagination objectPagination, SearchQuery searchQuery)
        {
            var list = _context.List
            .Where(l => l.ListName.Contains(searchQuery.query) && l.UserList.Any(u => u.UserId == currentUserId))
            .Skip((objectPagination.Page - 1) * objectPagination.Size).Take(objectPagination.Size).ToList();

            if (list == null)
            {
                return new ReturnObject<PaginatedObject<List>> { Message = "Error in retrieving the list." };
            }

            var paginatedList = new PaginatedObject<List>("list?", objectPagination, list, _context.List.Where(l => l.UserList.Any(u => u.UserId == currentUserId)).Count());

            return new ReturnObject<PaginatedObject<List>> { Value = paginatedList };
        }

        public ReturnObject<ListWithPaginatedItems> GetById(int id, ObjectPagination objectPagination)
        {
            var list = _context.List.Where(l => l.ListId == id).Include(l => l.UserList).Select(l => new List {
                ListId = l.ListId,
                ListName = l.ListName,
                UserList = l.UserList
            }).FirstOrDefault();
            if (list == null)
            {
                return new ReturnObject<ListWithPaginatedItems> { Message = "List not found." };
            }
            //var userList = _context.UserList.Where(ul => ul.ListId == id).ToList();
            var items = _context.Item.Where(i => i.ListId == list.ListId).Select(i => new Item {
                ItemId = i.ItemId,
                ItemName = i.ItemName,
                Quantity = i.Quantity,
                Price = i.Price,
                Bought = i.Bought
            }).Skip((objectPagination.Page - 1) * objectPagination.Size).Take(objectPagination.Size).ToList();
            var paginatedItems = new PaginatedObject<Item>($"list/{id}?", objectPagination, items, _context.Item.Where(i => i.ListId == list.ListId).Count());

            return new ReturnObject<ListWithPaginatedItems> { Value = new ListWithPaginatedItems { list = list, paginatedItems = paginatedItems } };
        }

        public ReturnObject<List> Share(int listId, int userId, bool editPermission)
        {
            var list = _context.List.FirstOrDefault(l => l.ListId == listId);

            if (_context.Users.FirstOrDefault(u => u.UserId == userId) == null || list == null)
            {
                return new ReturnObject<List> { Message = "Invalid user or list." };
            }
            if( _context.UserList.FirstOrDefault(u => u.UserId == userId && u.ListId == listId) != null)
            {
                return new ReturnObject<List> { Message = "This user already has access to the list." };
            }

            try
            {
                _context.UserList.Add(new UserList
                {
                    ListId = listId,
                    UserId = userId,
                    EditPermission = editPermission
                });

                _context.SaveChanges();
            }
            catch (Exception)
            {
                return new ReturnObject<List> { Message = "Failed to save changes into the DB." };
            }

            return new ReturnObject<List> { Value = list};

        }

        private bool ListExists(int id)
        {
            return _context.List.Any(e => e.ListId == id);
        }

      }
}
