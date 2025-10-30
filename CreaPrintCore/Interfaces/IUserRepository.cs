using System.Threading.Tasks;
using CreaPrintCore.Models;
using System.Collections.Generic;

namespace CreaPrintCore.Interfaces
{
 public interface IUserRepository
 {
 Task<User?> GetByUsernameAsync(string username);
 Task<User> CreateAsync(User user);
 Task<IEnumerable<User>> GetAllAsync();
 }
}