using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SiegeDefense.GameComponents {
    public abstract class GameObject : DrawableGameComponent{

        private static Game _game;
        private List<GameObject> childObjects = new List<GameObject>();
        public GameObject ParentObject { get; set; } = null;
        public string Tag { get; set; } = "";
        protected GameManager gameManager;

        public static void Initialize(Game game) {
            _game = game;
            GameManager gameManager = new GameManager(_game);
            _game.Services.AddService(gameManager);
        }

        protected GameObject(Game game) : base(game) {
        }

        public GameObject() : base(_game) {
            gameManager = Game.Services.GetService<GameManager>();
        }

        public static List<T> FindObjects<T>() where T : GameObject {
            return _game.Components.Where(x => x is T).Cast<T>().ToList();
        }

        public static List<GameObject> FindObjectsByTag(string tag) {
            if (tag == null)
                return new List<GameObject>();

            return _game.Components.Cast<GameObject>().Where(x => tag.Equals(x.Tag)).ToList();
        }

        public T FindComponents<T>() where T : GameObject {
            GameObject root = this;
            while (root.ParentObject != null) root = root.ParentObject;

            return FindComponents<T>(root);
        }

        private static T FindComponents<T>(GameObject current) where T : GameObject {
            if (current is T)
                return (T)current;

            T retValue = null;
            foreach(GameObject childObject in current.childObjects) {
                retValue = FindComponents<T>(childObject);
                if (retValue != null)
                    return retValue;
            }

            return retValue;
        }

        public virtual void AddChild(GameObject child) {
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
