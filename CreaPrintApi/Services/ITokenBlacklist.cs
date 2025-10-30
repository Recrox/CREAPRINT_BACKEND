namespace CreaPrintApi.Services
{
 public interface ITokenBlacklist
 {
 void RevokeToken(string token);
 bool IsRevoked(string token);
 }
}