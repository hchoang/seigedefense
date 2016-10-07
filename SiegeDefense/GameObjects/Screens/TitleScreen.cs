using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense {
    public class TitleScreen : GameObject {
        public SpriteRenderer background { get; set; }
        public TitleMenu menu { get; set; }
        public TitleScreen() {
            Game.Components.Add(this);

            background = new SpriteRenderer(Game.Content.Load<Texture2D>(@"Sprites\MainMenuBackground"));
            AddComponent(background);

            menu = new TitleMenu();
        }
    }
}
