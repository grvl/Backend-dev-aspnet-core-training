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
        private List<Users> _users = new List<Users>
        {
            new Users {Username = "test", Pswd = "test" }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public Users Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Pswd == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Pswd = null;

            return user;
        }

        public IEnumerable<Users> GetAll()
        {
            // return users without passwords
            return _users.Select(x => {
                x.Pswd = null;
                return x;
            });
        }

        IEnumerable<Users> IUserService.GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
