using System;
using System.Collections.Generic;

namespace wishlist.Models
{
    public partial class UserLists
    {
        public int UserId { get; set; }
        public int ListId { get; set; }

        public virtual Lists List { get; set; }
        public virtual Users User { get; set; }
    }
}
