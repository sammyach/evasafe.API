using evasafe.API.Data;
using System.ComponentModel.DataAnnotations;

namespace evasafe.API.dtos
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password must have at least 8 characters")]
        public string Password { get; set; } = null!;
    }
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password must have at least 8 characters")]
        public string Password { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
    }
    public class UserDetailsDto
    {      
        public string Phone { get; set; } = null!;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Organization { get; set; }
        public string? JobTitle { get; set; }
    }

    public class AppUserDto
    {
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool? Enabled { get; set; }
        public string? HashString { get; set; }

        public AppUserDto(EvAppUser user) {
            this.Email = user.Email;
            this.Phone = user.Phone;
            this.DateCreated = user.DateCreated;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.LastLoginDate = user.LastLoginDate;
            this.Enabled = user.Enabled;
            this.HashString = user.HashString;
        }
    }

    public class EnableDto
    {
        public string Email { get; set; } = null!;
        public bool Enabled { get; set; }
        public string Xcode { get; set; } = null!;
    }
}
