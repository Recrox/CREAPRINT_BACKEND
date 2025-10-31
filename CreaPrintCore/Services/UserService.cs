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
            // Ensure new users are inactive until they activate via email
            user.IsActive = false;
            // create activation token valid for7 days
            user.ActivationToken = GenerateToken();
            user.ActivationTokenExpires = DateTime.UtcNow.AddDays(7);
            return await _repository.CreateAsync(user);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            var currentHash = ComputeHash(currentPassword);
            if (currentHash != user.PasswordHash) return false;

            user.PasswordHash = ComputeHash(newPassword);
            await _repository.UpdateAsync(user);
            return true;
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        private static string GenerateToken()
        {
            // create a32-bytes random token encoded as URL-safe base64
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }
    }
}
