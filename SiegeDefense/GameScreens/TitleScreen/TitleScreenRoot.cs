using System;
using Microsoft.Xna.Framework;

namespace SiegeDefense.GameComponents.TitleScreen {
    public class TitleScreenRoot : GameObject {

        public TitleScreenRoot() {
            AddChild(new TitleScreenBackground());
            AddChild(new TitleMenu());
        }
    }
}
