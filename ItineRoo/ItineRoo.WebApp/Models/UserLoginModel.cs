using System.ComponentModel.DataAnnotations;

namespace ItineRoo.WebApp.Models
{
    public class UserLoginModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
