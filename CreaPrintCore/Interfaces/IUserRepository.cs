using System.Threading.Tasks;
using System.Collections.Generic;
using CreaPrintCore.Models.Users;

namespace CreaPrintCore.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task UpdateAsync(User user);
        // Find a user by activation token (used for account activation)
        Task<User?> GetByActivationTokenAsync(string token);
    }
}