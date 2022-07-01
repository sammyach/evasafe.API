using System;
using System.Collections.Generic;

namespace evasafe.API.data
{
    public partial class EvAppSubscription
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Active { get; set; }
    }
}
