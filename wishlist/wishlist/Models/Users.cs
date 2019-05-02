using System;
using System.Collections.Generic;

namespace wishlist.Models
{
    public partial class Users
    {
        public Users()
        {
            UserLists = new HashSet<UserLists>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Pswd { get; set; }
        public string Token { get; set; }

        public virtual ICollection<UserLists> UserLists { get; set; }
    }
}
