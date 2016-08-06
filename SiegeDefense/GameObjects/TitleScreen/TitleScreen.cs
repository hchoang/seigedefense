using System;
using Microsoft.Xna.Framework;

namespace SiegeDefense.GameObjects.TitleScreen {
    public class TitleScreen : GameObject {

        public TitleScreen() {
            childObjects.Add(new TitleScreenBackground());
            childObjects.Add(new TitleMenu());
        }
    }
}
