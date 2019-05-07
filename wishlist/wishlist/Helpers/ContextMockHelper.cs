using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wishlist.Models;

namespace wishlist.Helpers
{
    public class ContextMockHelper
    {
        public virtual bool SetPropertyIsModified(WishlistDBContext _context, Item item)
        {
            _context.Entry(item).State = EntityState.Modified;

            return true;
        }

        public virtual bool SetPropertyIsModified(WishlistDBContext _context, List list)
        {
            _context.Entry(list).State = EntityState.Modified;

            return true;
        }

        public virtual bool RunStoredProcedure(WishlistDBContext _context, int userId, List list)
        {
            _context.Database.ExecuteSqlCommand($"exec [dbo].[sp_InsertUserListInfo] @ListName=@p0, @UserId=@p1",
                parameters: new String[] { list.ListName.ToString(), userId.ToString() });

            return true;
        }
    }
}
