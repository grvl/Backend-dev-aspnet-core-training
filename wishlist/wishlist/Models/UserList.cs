using System;
using System.Collections.Generic;

namespace wishlist.Models
{
    public partial class UserList
    {
        public int UserId { get; set; }
        public int ListId { get; set; }
        public bool EditPermission { get; set; }

        public virtual List List { get; set; }
        public virtual Users User { get; set; }
    }
}
