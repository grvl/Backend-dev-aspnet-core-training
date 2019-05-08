using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace wishlist.Models
{
    public partial class Users
    {
        public Users()
        {
            UserList = new HashSet<UserList>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Pswd { get; set; }
        public string UserRole { get; set; }
        [NotMapped]
        public string Token { get; set; }

        public virtual ICollection<UserList> UserList { get; set; }
    }
}
