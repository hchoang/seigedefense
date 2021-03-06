﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class TankBullet : _3DGameObject
    {
        public int damage { get; private set; }
        public _3DGameObject owner { get; set; }
        public TankBullet(ModelType modelType, _3DGameObject owner)
        {
            this.damage = 20;
            this.owner = owner;

            Model bulletModel = Game.Content.Load<Model>(modelType.ToDescription());
            renderer = new ModelRenderer(bulletModel);
            AddComponent(renderer);

            collider = new Collider(bulletModel);
            AddComponent(collider);

            physics = new BulletPhysics();
            AddComponent(physics);
        }

        public override void Update(GameTime gameTime) {
            // collision checking with other vehicles
            OnlandVehicle collidedVehicle = null;
            foreach (OnlandVehicle vehicle in FindObjectsInPartition<OnlandVehicle>()) {
                if (owner == vehicle) continue;

                if (collider.Intersect(vehicle.collider)) {
                    //Console.Out.WriteLine("Intersect");
                    collidedVehicle = vehicle;
                    break;
                }
            }

            if (collidedVehicle != null) {
                collidedVehicle.Damaged(this.damage);
                if (this.owner.Tag.Equals("Player")) {
                    FindObjects<GameLevelManager>()[0].Point += 10;
                }
                //Game.Components.Remove(this);
                RemoveFromGameWorld();
            }

            // remove bullet if outside map or hit the ground
            if (!map.IsInsideMap(transformation.Position)) {
                //Game.Components.Remove(this);
                RemoveFromGameWorld();
                return;
            }

            float bulletHeight = transformation.Position.Y;
            float mapHeight = map.GetHeight(transformation.Position);
            if (bulletHeight < mapHeight) {
                RemoveFromGameWorld();
                //Game.Components.Remove(this);
            }

            base.Update(gameTime);
        }
    }
}
