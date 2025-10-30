using System.Threading.Tasks;
using CreaPrintCore.Models;
using System.Collections.Generic;

namespace CreaPrintCore.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User> CreateAsync(User user, string password);
        Task<IEnumerable<User>> GetAllAsync();
    }
}