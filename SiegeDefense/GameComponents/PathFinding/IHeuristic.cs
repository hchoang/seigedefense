using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public interface IHeuristic {
        double GetHeuristic(INode a, INode b);
    }
}
