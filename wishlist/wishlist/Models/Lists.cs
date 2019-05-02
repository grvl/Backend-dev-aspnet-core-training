using System;
using System.Collections.Generic;

namespace wishlist.Models
{
    public partial class Lists
    {
        public Lists()
        {
            Items = new HashSet<Items>();
            UserLists = new HashSet<UserLists>();
        }

        public int ListId { get; set; }
        public string ListName { get; set; }

        public virtual ICollection<Items> Items { get; set; }
        public virtual ICollection<UserLists> UserLists { get; set; }
    }
}
