using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense{
    public class StateMachine {
        public Dictionary<string, AIState> stateMap { get; set; } = new Dictionary<string, AIState>();
        public Dictionary<string, Func<bool>> conditionMap { get; set; } = new Dictionary<string, Func<bool>>();
    }
}
