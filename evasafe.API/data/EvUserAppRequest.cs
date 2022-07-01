using System;
using System.Collections.Generic;

namespace evasafe.API.data
{
    public partial class EvUserAppRequest
    {
        public int Id { get; set; }
        public string User { get; set; } = null!;
        public DateTime? TimeStamp { get; set; }
        public bool Successful { get; set; }
        public string? Remarks { get; set; }
        public string? ClientLocalTime { get; set; }
        public string? SourceIp { get; set; }
        public string? Path { get; set; }
    }
}
