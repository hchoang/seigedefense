using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public abstract class GameObjectComponent : GameObject {
        public GameObject baseObject { get; set; }
    }
}
