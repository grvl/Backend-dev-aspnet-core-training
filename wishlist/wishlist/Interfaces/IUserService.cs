using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wishlist.Helpers;
using wishlist.Models;

namespace wishlist.Interfaces
{
    public interface IUserService
    {
        ReturnObject<Users> Authenticate(string username, string password);
        ReturnObject<Users> GetAll();
        ReturnObject<Users> GetById(int id);
        ReturnObject<Users> Create(Users user);
    }
}
