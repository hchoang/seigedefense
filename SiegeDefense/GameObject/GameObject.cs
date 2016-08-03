using Microsoft.Xna.Framework;
using System;

namespace SiegeDefense {
    public abstract class GameObject {
        private static GameServiceContainer serviceLocator;

        protected GameServiceContainer ServiceLocator {
            get {
                if (serviceLocator == null) {
                    throw new ArgumentNullException("ServiceLocator");
                }
                return serviceLocator;
            }
        }

        public static void Initialize(GameServiceContainer serviceLocator) {
            if (serviceLocator != null)
                throw new Exception("Duplicate initialization of GameObject");

            GameObject.serviceLocator = serviceLocator;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
