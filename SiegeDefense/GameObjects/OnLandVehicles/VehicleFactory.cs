using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense {
    public sealed class VehicleFactory : GameObject {
        public static Tank CreateTank(ModelType modelType, int HP) {
            Tank tank = new Tank();
            tank.vehicleModel = _game.Content.Load<Model>(modelType.ToDescription());

            tank.renderer = new TankRenderer(tank.vehicleModel);
            tank.AddComponent(tank.renderer);

            tank.collider = new Collider(tank.vehicleModel);
            tank.AddComponent(tank.collider);

            tank.physics = new OnlandVehiclePhysics();
            tank.AddComponent(tank.physics);

            tank.HP = HP;
            float tankHeight = tank.collider.baseBoundingBox.Max.Y - tank.collider.baseBoundingBox.Min.Y;
            tank.hpRenderer = new HPRenderer(new Vector3(0, tankHeight, 0));
            tank.hpRenderer.maxHP = HP;
            tank.hpRenderer.currentHP = HP;
            tank.AddComponent(tank.hpRenderer);

            // Render bounding box
            tank.AddComponent(new WireFrameBoxRenderer(tank.collider.baseBoundingBox.GetCorners(), Color.Blue));

            return tank;
        }

        public static ExplosiveTruck CreateExplosiveTruck(ModelType modelType, int HP) {
            ExplosiveTruck truck = new ExplosiveTruck();
            truck.vehicleModel = _game.Content.Load<Model>(modelType.ToDescription());

            truck.renderer = new ModelRenderer(truck.vehicleModel);
            truck.AddComponent(truck.renderer);

            truck.collider = new Collider(truck.vehicleModel);
            truck.AddComponent(truck.collider);

            truck.physics = new OnlandVehiclePhysics();
            truck.AddComponent(truck.physics);

            truck.HP = HP;
            float truckHeight = truck.collider.baseBoundingBox.Max.Y - truck.collider.baseBoundingBox.Min.Y;
            truck.hpRenderer = new HPRenderer(new Vector3(0, truckHeight, 0));
            truck.hpRenderer.maxHP = HP;
            truck.hpRenderer.currentHP = HP;
            truck.AddComponent(truck.hpRenderer);

            // Render bounding box
            truck.AddComponent(new WireFrameBoxRenderer(truck.collider.baseBoundingBox.GetCorners(), Color.Blue));

            return truck;
        }
    }
}
