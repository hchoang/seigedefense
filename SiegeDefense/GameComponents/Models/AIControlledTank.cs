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
        private float fireRange;
        public Tank enemy { get; private set; }

        public AIControlledTank(Model model, Vector3 position, GameObject TankAI, Tank enemy) : base(model)
        {
            this.Position = position;
            this.AddChild(TankAI);
            this.enemy = enemy;
            this.visibleRange = 600;
            this.fireRange = 400;
        }

        public bool isInVisibleRange(BaseModel model)
        {
            if (Vector3.Distance(Position, model.Position) <= visibleRange)
                return true;
            return false;
        }

        public bool isInFireRange(BaseModel model)
        {
            if (Vector3.Distance(Position, model.Position) <= fireRange)
                return true;
            return false;
        }

        public override void Fire()
        {
            base.Fire();
        }
    }
}
