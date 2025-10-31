using System.Threading.Tasks;
using CreaPrintCore.Models;
using System.Collections.Generic;

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