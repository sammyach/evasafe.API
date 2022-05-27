﻿using System;
using System.Collections.Generic;

namespace evasafe.API.Data
{
    public partial class EvAppUser
    {
        public EvAppUser()
        {
            EvUserAccountActionTokens = new HashSet<EvUserAccountActionToken>();
        }

        public string Email { get; set; } = null!;
        public string? HashString { get; set; }
        public string Phone { get; set; } = null!;
        public string? FirstName { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public byte[]? PasswordHash { get; set; }
        public string? LastName { get; set; }
        public byte[]? Nacl { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? Organization { get; set; }
        public string? JobTitle { get; set; }

        public virtual ICollection<EvUserAccountActionToken> EvUserAccountActionTokens { get; set; }
    }
}
