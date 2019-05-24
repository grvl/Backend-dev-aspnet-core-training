using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wishlist.Models;

namespace wishlist.Helpers
{
    public class ListWithPaginatedItems
    {
        public List list { get; set; }
        //public List<UserList> userList { get; set; }
        public PaginatedObject<Item> paginatedItems { get; set; }
    }
}
