using evasafe.API.data;
using evasafe.API.utils;
using System.ComponentModel.DataAnnotations;

namespace evasafe.API.dtos
{
    public class LoginDto
    {
        [Required]
        public string User { get; set; } = null!;
        [Required]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password must have at least 8 characters")]
        public string Password { get; set; } = null!;
    }
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password must have at least 8 characters")]
        public string Password { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Organization { get; set; }
        public string? JobTitle { get; set; }
    }
    public class UserDetailsDto
    {
        [Required]
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Organization { get; set; }
        public string? JobTitle { get; set; }
    }

    public class AppUserDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool? Enabled { get; set; }
        public string? HashString { get; set; }
        public string? Organization { get; set; }
        public string? JobTitle { get; set; }
        public string SubscriptionType { get; set; } = null!;
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }

        public AppUserDto(EvAppUser user, EvUserSubscription sub) {
            this.Username = user.Username;
            this.Email = user.Email;
            this.Phone = user.Phone;
            this.DateCreated = user.DateCreated;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.LastLoginDate = user.LastLoginDate;
            this.Enabled = user.Enabled;
            this.HashString = user.HashString;
            this.Organization = user.Organization;
            this.JobTitle = user.JobTitle;
            if (sub != null) {
                AppSubscriptions appSub = (AppSubscriptions)sub.SubscriptionType;
                this.SubscriptionType = appSub.ToString();
                this.SubscriptionStartDate = DateTime.UtcNow;//sub.DateCreated;
                this.SubscriptionEndDate = sub.EndDate;
            }
        }
    }

    public class EnableDto
    {
        public string User { get; set; } = null!;
        public bool Enabled { get; set; }
        public string Xcode { get; set; } = null!;
    }

    public class SubscribeDto
    {
        public string Username { get; set; } = null!;
        public int SubscriptionType { get; set; }
        public int NumberOfDays { get; set; }
    }
}
