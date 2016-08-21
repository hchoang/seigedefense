using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.SoundBank
{
    class SoundBankManager : GameObject
    {
        private Dictionary<String, SoundEffectInstance> sounds;

        public SoundBankManager()
        {
            sounds = new Dictionary<string, SoundEffectInstance>();
        }

        protected override void LoadContent()
        {
            SoundEffect effect = Game.Content.Load<SoundEffect>(@"Sound/tank-fire");
            sounds.Add("Tank", effect.CreateInstance());
            base.LoadContent();
        }

        public SoundEffectInstance FindSound(String name)
        {
            return sounds[name];
        }
    }
}
