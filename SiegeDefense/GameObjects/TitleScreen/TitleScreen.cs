using System;
using Microsoft.Xna.Framework;

namespace SiegeDefense.GameObjects.TitleScreen {
    public class TitleScreen : GameObject {

        public TitleScreen() {
            AddChild(new TitleScreenBackground());
            AddChild(new TitleMenu());
        }
    }
}
