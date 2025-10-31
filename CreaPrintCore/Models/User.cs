namespace CreaPrintCore.Models
{
    public class User : AuditableEntity
    {
        public required string Username { get; set; }
        // PasswordHash will be set by the service when creating the user
        public string PasswordHash { get; set; } = string.Empty;
        public UserRights Rights { get; set; } = UserRights.None;
        // Email address for the user
        public required string Email { get; set; }

        // Whether the user's account is active (has been activated)
        public bool IsActive { get; set; } = false;

        // Token used for account activation (one-time)
        public string? ActivationToken { get; set; }

        // Expiration time for the activation token
        public DateTime? ActivationTokenExpires { get; set; }
    }
}