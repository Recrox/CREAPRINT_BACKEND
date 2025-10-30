using System.Threading.Tasks;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace CreaPrintCore.Services
{
 public class UserService : IUserService
 {
 private readonly IUserRepository _repository;

 public UserService(IUserRepository repository)
 {
 _repository = repository;
 }

 public async Task<User?> AuthenticateAsync(string username, string password)
 {
 var user = await _repository.GetByUsernameAsync(username);
 if (user == null) return null;

 var hash = ComputeHash(password);
 if (hash != user.PasswordHash) return null;
 return user;
 }

 public async Task<User> CreateAsync(User user, string password)
 {
 user.PasswordHash = ComputeHash(password);
 return await _repository.CreateAsync(user);
 }

 public async Task<IEnumerable<User>> GetAllAsync()
 {
 return await _repository.GetAllAsync();
 }

 private static string ComputeHash(string input)
 {
 using var sha = SHA256.Create();
 var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
 return Convert.ToBase64String(bytes);
 }
 }
}
