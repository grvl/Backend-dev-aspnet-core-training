using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
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
        
        private readonly WishlistDBContext _context;
        private readonly IPasswordHasher<string> _passwordHasher;

        public UserService(WishlistDBContext context, IPasswordHasher<String> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public ReturnObject<Users> Authenticate(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username && 
            _passwordHasher.VerifyHashedPassword(username, x.Pswd, password) != PasswordVerificationResult.Failed);

            // return null if user not found
            if (user == null)
                return new ReturnObject<Users> { Message = "Username os password is incorrect" };

            // remove password before returning
            user.Pswd = null;

            return new ReturnObject<Users> { Value = user };
        }

        public ReturnObject<Users> Create(Users user)
        {
            var previousUser = _context.Users.SingleOrDefault(x => x.Username == user.Username);

            if(previousUser != null)
            {
                return new ReturnObject<Users> { Message = "Username already in use." };
            }

            if(user.Pswd.Equals("") || user.Pswd == null)
            {
                return new ReturnObject<Users> { Message = "You must input a password." };
            }

            _context.Users.Add(new Users { Username = user.Username, Pswd = _passwordHasher.HashPassword(user.Username, user.Pswd) });
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return new ReturnObject<Users> { Message = "Failed to create new user." }; ;
            }

            return new ReturnObject<Users> {Value = user }; ;
        }

        public ReturnObject<PaginatedObject<Users>> Search(SearchQuery searchUser, ObjectPagination objectPagination)
        {
            var users = _context.Users.Where(u => u.Username.Contains(searchUser.query))
                .Skip((objectPagination.Page - 1) * objectPagination.Size).Take(objectPagination.Size)
                .Select(u => new Users {Username = u.Username,
                                        UserId = u.UserId}).ToList();

            if (users == null)
            {
                return new ReturnObject<PaginatedObject<Users>> { Message = "Error in retrieving the list." };
            }

            var paginatedUsers = new PaginatedObject<Users>($"users/search?query={searchUser.query}&", objectPagination, users, _context.Users.Where(u => u.Username.Contains(searchUser.query)).Count());

            return new ReturnObject<PaginatedObject<Users>> { Value = paginatedUsers };
        }

        public ReturnObject<Users> GetAll()
        {
            var users = _context.Users.Select(s => new Users
            {
                Username = s.Username,
                UserRole = s.UserRole
            });

            if (users == null){
                return new ReturnObject<Users> { Message = " Failed to load list of all users." };
            }
            // return users
            return new ReturnObject<Users> { Values = users };
        }

        public ReturnObject<Users> GetById(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == id);

            if (user == null)
            {
                return new ReturnObject<Users> { Message = "User not found." };
            }

            user.Pswd = null;

            return new ReturnObject<Users> { Value = user };
        }
    }
}
