﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public interface INode {
        Dictionary<INode, double> adjacentNodes { get; set; }
    }
}
