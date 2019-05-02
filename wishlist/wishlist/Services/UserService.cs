using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using wishlist.Helpers;
using wishlist.Interfaces;
using wishlist.Models;

namespace wishlist.Services
{
    public class UserService : IUserService
    {
        
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
       private readonly WishlistDBContext _context;
        private DbSet<Users> _users;

        public UserService(WishlistDBContext context)
        {
            _context = context;
            _users = context.Users;
        }

        public Users Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Pswd == password);

            // return null if user not found
            if (user == null)
                return null;

            // remove password before returning
            user.Pswd = null;

            return user;
        }

        public Users Create(Users user)
        {
            _users.Add(new Users { Username = user.Username, Pswd = user.Pswd });
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return null;
            }

            return user;
        }

        public IEnumerable<Users> GetAll()
        {
            // return users
            return _users.Select(s => new Users
            {
                Username = s.Username,
                UserRole = s.UserRole
            }).ToList();
        }

        public Users GetById(int id)
        {
            var user = _users.FirstOrDefault(x => x.UserId == id);

            // return user without password
            if (user != null)
                user.Pswd = null;

            return user;
        }
    }
}
