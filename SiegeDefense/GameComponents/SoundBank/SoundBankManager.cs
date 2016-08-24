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
            sounds.Add("Tank", Game.Content.Load<SoundEffect>(@"Sound/tank-fire").CreateInstance());
            //sounds.Add("TankMoving", Game.Content.Load<SoundEffect>(@"").Duration());
            //sounds.Add()
            base.LoadContent();
        }

        public SoundEffectInstance FindSound(String name)
        {
            return sounds[name];
        }
    }
}
