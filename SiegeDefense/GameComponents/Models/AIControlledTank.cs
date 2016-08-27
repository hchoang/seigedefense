using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SiegeDefense.GameComponents.Models
{
    class AIControlledTank : Tank
    {
        private float visibleRange;
        public Tank enemy { get; private set; }

        public AIControlledTank(Model model, Vector3 position, GameObject TankAI, Tank enemy) : base(model)
        {
            this.Position = position;
            this.AddChild(TankAI);
            this.enemy = enemy;
            this.visibleRange = 300;
        }

        public bool isInRange(BaseModel model)
        {
            if (Vector3.Distance(Position, model.Position) <= visibleRange)
                return true;
            return false;
        }
    }
}
