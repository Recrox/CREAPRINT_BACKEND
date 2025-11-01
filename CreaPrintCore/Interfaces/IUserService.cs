using System.Threading.Tasks;
using System.Collections.Generic;
using CreaPrintCore.Models.Users;

namespace CreaPrintCore.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User> CreateAsync(User user, string password);
        Task<IEnumerable<User>> GetAllAsync();
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}