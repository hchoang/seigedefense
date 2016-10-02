using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {

    public abstract class AIState {
        public string StateName { get; set; }
        public Dictionary<string, string> Transitions { get; set; } = new Dictionary<string, string>();
    }
}
