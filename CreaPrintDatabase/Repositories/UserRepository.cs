using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using CreaPrintCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
 private readonly CreaPrintDbContext _dbContext;
 public UserRepository(CreaPrintDbContext context) : base(context)
 {
 _dbContext = context;
 }

 public async Task<User?> GetByUsernameAsync(string username)
 {
 return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
 }

 public async Task<User> CreateAsync(User user)
 {
 return await base.CreateAsync(user);
 }

 public async Task<IEnumerable<User>> GetAllAsync()
 {
 return await _dbContext.Users.ToListAsync();
 }
}
