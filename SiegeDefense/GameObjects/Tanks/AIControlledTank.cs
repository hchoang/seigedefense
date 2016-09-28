using Microsoft.Xna.Framework;

namespace SiegeDefense {
    class AIControlledTank : Tank
    {
        private float visibleRange;
        private float fireRange;
        public Tank enemy { get; private set; }

        public AIControlledTank(ModelType modelType, Vector3 position, GameObjectComponent TankAI, Tank enemy) : base(modelType)
        {
            this.transformation.Position = position;
            this.AddComponent(TankAI);
            //this.AddComponent(new EnemyTankAI());
            this.enemy = enemy;
            this.visibleRange = 600;
            this.fireRange = 400;
        }

        public bool isInVisibleRange(_3DGameObject target)
        {
            if (Vector3.Distance(transformation.Position, target.transformation.Position) <= visibleRange)
                return true;
            return false;
        }

        public bool isInFireRange(_3DGameObject target)
        {
            if (Vector3.Distance(transformation.Position, target.transformation.Position) <= fireRange)
                return true;
            return false;
        }
    }
}
