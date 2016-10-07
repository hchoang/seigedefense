using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;

namespace SiegeDefense {
    public abstract class GameObject : DrawableGameComponent{

        #region Components
        private List<GameObjectComponent> components = new List<GameObjectComponent>();
        public virtual string Tag { get; set; } = "";

        public T GetComponent<T>() where T : GameObjectComponent {
            return (T)components.Where(x => x is T).FirstOrDefault();
        }

        public virtual void AddComponent(GameObjectComponent component) {
            component.baseObject = this;
            components.Add(component);
            component.componentInit();
        }

        public override void Draw(GameTime gameTime) {
            foreach (GameObjectComponent component in components) {
                component.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime) {
            foreach (GameObjectComponent component in components) {
                component.Update(gameTime);
            }
        }

        public virtual Transformation transformation { get; protected set; }
        #endregion

        // avoid passing Game to every constructor
        protected static Game _game;
        public static void Initialize(Game game) {
            _game = game;
        }

        public GameObject() : base(_game) {
            transformation = new Transformation();
        }

        public virtual List<T> FindObjects<T>() where T : GameObject {
            return _game.Components.Where(x => x is T).Cast<T>().ToList();
        }

        public static List<GameObject> FindObjectsByTag(string tag) {
            if (tag == null)
                return new List<GameObject>();

            return _game.Components.Cast<GameObject>().Where(x => tag.Equals(x.Tag)).ToList();
        }
    }
}
