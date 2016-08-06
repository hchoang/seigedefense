using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SiegeDefense.GameObjects {
    public abstract class GameObject : DrawableGameComponent{

        private static Game _game;
        protected List<GameObject> childObjects = new List<GameObject>();

        protected List<T> findObject<T>() where T : GameObject {
            GameObject root = Game.Services.GetService<Stack<GameObject>>().Peek();

            return findObject<T>(root);
        }

        protected List<T> findObject<T>(GameObject baseNode) where T : GameObject {
            List<T> objectsFound = new List<T>();

            foreach(GameObject go in baseNode.childObjects) {
                if (go.GetType() == typeof(T))
                    objectsFound.Add((T)go);

                objectsFound.AddRange(findObject<T>(go));
            }

            return objectsFound;
        }

        public static void Initialize(Game game) {
            _game = game;
        }

        public GameObject() : base(_game) {

        }

        public override void Draw(GameTime gameTime) {
            foreach(GameObject go in childObjects) {
                go.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime) {
            foreach(GameObject go in childObjects) {
                go.Update(gameTime);
            }
        }
    }
}
