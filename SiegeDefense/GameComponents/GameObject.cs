using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents {
    public abstract class GameObject : DrawableGameComponent{

        private static Game _game;

        public static void Initialize(Game game) {
            _game = game;
        }

        public GameObject() : base(_game) {
        }

        protected virtual void GetDependentComponents() {
            foreach (GameObject childObject in childObjects)
                childObject.GetDependentComponents();
        }

        public static List<T> FindObjects<T>() where T : GameObject {
            return _game.Components.Where(x => x.GetType() == typeof(T)).Cast<T>().ToList();
        }

        public T FindComponent<T>() where T : GameObject {
            GameObject root = this;
            while (root.ParentObject != null) root = root.ParentObject;

            return FindComponent<T>(root);
        }

        private static T FindComponent<T>(GameObject current) where T : GameObject {
            if (current.GetType() == typeof(T))
                return (T)current;

            T retValue = null;
            foreach(GameObject childObject in current.childObjects) {
                retValue = FindComponent<T>(childObject);
                if (retValue != null)
                    return retValue;
            }

            return retValue;
        }

        private List<GameObject> childObjects = new List<GameObject>();
        public GameObject ParentObject { get; set; } = null;
        public void AddChild(GameObject child) {
            childObjects.Add(child);
            child.ParentObject = this;
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
