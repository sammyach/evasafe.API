using System;
using System.Collections.Generic;

namespace evasafe.API.Data
{
    public partial class EvUserAccountActionToken
    {
        public int Id { get; set; }
        public string? User { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenDate { get; set; }
        public int? Action { get; set; }

        public virtual EvAccountActionsType? ActionNavigation { get; set; }
        public virtual EvAppUser? UserNavigation { get; set; }
    }
}
