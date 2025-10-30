namespace CreaPrintCore.Models
{
    public class User : AuditableEntity
    {
        public required string Username { get; set; }
        // PasswordHash will be set by the service when creating the user
        public string PasswordHash { get; set; } = string.Empty;
        public UserRights Rights { get; set; } = UserRights.None;
    }
}