using wishlist.Models;

namespace wishlist.Services
{
    public interface IJwtService
    {
        string CreateJwtToken(Users user);
    }
}