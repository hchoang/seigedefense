using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.SoundBank
{
    class SoundBankManager : GameObject
    {
        private Dictionary<String, SoundEffect> sounds;

        public SoundBankManager()
        {
            sounds = new Dictionary<string, SoundEffect>();
        }

        protected override void LoadContent()
        {
            sounds.Add("Tank", Game.Content.Load<SoundEffect>(@"Sound/tank-fire"));
            base.LoadContent();
        }
    }
}
