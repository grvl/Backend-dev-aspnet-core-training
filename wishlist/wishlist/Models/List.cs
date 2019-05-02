using System;
using System.Collections.Generic;

namespace wishlist.Models
{
    public partial class List
    {
        public List()
        {
            Item = new HashSet<Item>();
            UserList = new HashSet<UserList>();
        }

        public int ListId { get; set; }
        public string ListName { get; set; }

        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<UserList> UserList { get; set; }
    }
}
