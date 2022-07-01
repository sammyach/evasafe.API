using System;
using System.Collections.Generic;

namespace evasafe.API.data
{
    public partial class EvUserSubscription
    {
        public int Id { get; set; }
        public string User { get; set; } = null!;
        public int SubscriptionType { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual EvAppUser UserNavigation { get; set; } = null!;
    }
}
