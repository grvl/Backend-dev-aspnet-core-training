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

        public ListService(WishlistDBContext context)
        {
            _context = context;
        }

        public async Task<ReturnObject<List>> CreateAsync(List list, String userId)
        {
            _context.Database.ExecuteSqlCommand($"exec [dbo].[sp_InsertUserListInfo] @ListName=@p0, @UserId=@p1",
                parameters: new String[] { list.ListName.ToString(), userId});
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new ReturnObject<List> { Message = "An error occured while trying to save the new list." };
            }

            var created = await _context.List.OrderByDescending(l => l.ListId).FirstOrDefaultAsync();

            if (created == null)
            {
                return new ReturnObject<List> { Message = "Failed to retrieve the new list." };
            }

            return new ReturnObject<List> { Value = created };
        }

        public async Task<ReturnObject<List>> DeleteAsync(int id)
        {
            var userList = _context.UserList.Where(ul => ul.ListId == id);
            var List = await _context.List.FindAsync(id);
            if (List == null)
            {
                return new ReturnObject<List> { Message = "List not found." };
            }

            if (userList.Count() == 0)
            {
                _context.List.Remove(List);
                await _context.SaveChangesAsync();
                return new ReturnObject<List> { Message = "List without owner." };
            }

            _context.UserList.RemoveRange(userList);
            _context.List.Remove(List);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<ReturnObject<List>> EditAsync(int id, List list)
        {
            if (id != list.ListId)
            {
                return new ReturnObject<List> { Message = "Invalid list Id." };
            }

            _context.Entry(list).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!ListExists(id))
                {
                    return new ReturnObject<List> { Message = "List not found." };
                }
                else
                {
                    return new ReturnObject<List> { Message = "Error saving list into the DB." };
                }
            }

            return new ReturnObject<List> { Value = list };
        }

        public ReturnObject<List> GetAll(int currentUserId, int page, int pageSize)
        {
            var list = _context.List.Where(l => l.UserList.Any(u => u.UserId == currentUserId)).Skip((page - 1) * pageSize).Take(pageSize);

            if (list == null)
            {
                return new ReturnObject<List> { Message = "Error in retrieving the list." };
            }

            return new ReturnObject<List> { Values = list };
        }

        public ReturnObject<List> GetById(int id, int page, int pageSize)
        {
            var list = _context.List.Where(l => l.ListId == id).Include(l => l.UserList).FirstOrDefault();
            if (list == null)
            {
                return new ReturnObject<List> { Message = "List not found." };
            }
            var items = _context.Item.Where(i => i.ListId == list.ListId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            list.Item = items;
            return new ReturnObject<List> { Value = list };
        }

        public async Task<ReturnObject<List>> ShareAsync(int listId, int userId)
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
                await _context.UserList.AddAsync(new UserList
                {
                    ListId = listId,
                    UserId = userId
                });

                await _context.SaveChangesAsync();
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
