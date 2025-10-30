using System.Collections.Concurrent;

namespace CreaPrintApi.Services
{
 public class InMemoryTokenBlacklist : ITokenBlacklist
 {
 private readonly ConcurrentDictionary<string, DateTime> _revoked = new();

 public void RevokeToken(string token)
 {
 if (string.IsNullOrEmpty(token)) return;
 // store with expiry far in the future; cleanup not implemented for simplicity
 _revoked[token] = DateTime.UtcNow;
 }

 public bool IsRevoked(string token)
 {
 if (string.IsNullOrEmpty(token)) return true;
 return _revoked.ContainsKey(token);
 }
 }
}