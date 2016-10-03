using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class Tank : _3DGameObject
    {
        public int HP { get; set; }
        public float TurretRotateSpeed { get; set; } = 0.05f;
        public float CanonRotateSpeed { get; set; } = 0.05f;

        public new TankRenderer renderer { get; set; }
        public new TankPhysics physics { get; set; }
        public HPRenderer hpRenderer { get; set; }

        public Tank(ModelType modelType, int HP)
        {
            Model tankModel = Game.Content.Load<Model>(modelType.ToDescription());
            
            renderer = new TankRenderer(tankModel);
            AddComponent(renderer);

            collider = new Collider(tankModel);
            AddComponent(collider);

            physics = new TankPhysics();
            AddComponent(physics);

            this.HP = HP;
            hpRenderer = new HPRenderer(new Vector3(0, 22.5f, 0));
            hpRenderer.maxHP = HP;
            AddComponent(hpRenderer);

            // Render bounding box
            AddComponent(new WireFrameBoxRenderer(collider.baseBoundingBox.GetCorners(), Color.Blue));
        }

        public override void Update(GameTime gameTime) {
            hpRenderer.currentHP = HP;

            base.Update(gameTime);
        }

        public bool Moveable(Vector3 testPosition) {
            if (!map.IsAccessibleByFoot(testPosition))
                return false;

            // collision check with other tanks
            Vector3 oldPosition = transformation.Position;
            transformation.Position = testPosition;

            foreach (Tank tank in FindObjectsInPartition<Tank>()) {
                if (tank == this) continue;
                if (collider.SphereIntersect(tank.collider)) {
                    transformation.Position = oldPosition;
                    return false;
                }
            }

            transformation.Position = oldPosition;
            return true;
        }

        public bool Move(Vector3 moveDirection) {
            Vector3 newPosition = transformation.Position + moveDirection;
            if (!Moveable(newPosition))
                return false;


            newPosition.Y = map.GetHeight(newPosition);
            transformation.Position = newPosition;
            Vector3 mapNormal = map.GetNormal(transformation.Position);
            transformation.Up = mapNormal;

            return true;
        }

        public void RotateTank(float rotationAngle)
        {
            transformation.RotationMatrix *= Matrix.CreateFromAxisAngle(transformation.Up, rotationAngle);
        }

        public virtual void Fire() {
            TankBullet bullet = new TankBullet(ModelType.BULLET1, this);
            bullet.Tag = this.Tag + "Bullet";

            // set bullet position & facing direction
            bullet.transformation.WorldMatrix = renderer.GetCanonHeadWorldMatrix();
            bullet.transformation.ScaleMatrix = Matrix.Identity;
            bullet.AddToGameWorld();
            //Game.Components.Add(bullet);

            // play sound
            SoundBankManager soundManager = Game.Services.GetService<SoundBankManager>();
            soundManager.PlaySound(SoundType.TankFire);
        }

        public void Destroy() {
            //Game.Components.Remove(this);
            RemoveFromGameWorld();

            // Drop power-up
            if (Tag.Equals("Enemy")) {
                Item item = new HPRestoreItem();
                item.transformation.Position = transformation.Position;
                item.transformation.ScaleMatrix = Matrix.CreateScale(10);

                //Game.Components.Add(item);
                item.AddToGameWorld();
            }
        }

        public void Damaged(TankBullet bullet)
        {
            this.HP -= bullet.damage;
            if (this.HP <= 0)
            {
                if (bullet.owner.Tag.Equals("Player"))
                {
                    Game.Services.GetService<GameManager>().Point += 10;
                }
                this.Destroy();
            }
        }
    }
}
