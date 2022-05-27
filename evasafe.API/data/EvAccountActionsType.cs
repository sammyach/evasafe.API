using System;
using System.Collections.Generic;

namespace evasafe.API.Data
{
    public partial class EvAccountActionsType
    {
        public EvAccountActionsType()
        {
            EvUserAccountActionTokens = new HashSet<EvUserAccountActionToken>();
        }

        public int Id { get; set; }
        public string ActionType { get; set; } = null!;

        public virtual ICollection<EvUserAccountActionToken> EvUserAccountActionTokens { get; set; }
    }
}
