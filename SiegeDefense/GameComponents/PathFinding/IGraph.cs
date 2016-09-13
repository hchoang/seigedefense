using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameComponents.PathFinding {
    public interface IGraph {
        List<INode> nodes { get; set; }
    }
}
