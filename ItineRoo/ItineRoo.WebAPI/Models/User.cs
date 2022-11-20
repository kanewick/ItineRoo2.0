namespace ItineRoo.WebAPI.Models
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string AuthRefreshToken { get; set; } = string.Empty;
        public DateTime AuthTokenCreated { get; set; }
        public DateTime AuthTokenExpires { get; set; }
    }
}
