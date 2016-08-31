using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace SiegeDefense.GameComponents.SoundBank
{
    public enum SoundType {
        TankFire
    }
    public class SoundBankManager : GameObject
    {
        private Dictionary<SoundType, SoundEffect> sounds;

        public SoundBankManager()
        {
            sounds = new Dictionary<SoundType, SoundEffect>();
            sounds.Add(SoundType.TankFire, Game.Content.Load<SoundEffect>(@"Sound/tank-fire"));
        }

        protected override void LoadContent()
        {
            
            //sounds.Add("TankMoving", Game.Content.Load<SoundEffect>(@"").Duration());
            //sounds.Add()
            base.LoadContent();
        }

        public void PlaySound(SoundType type) {
            SoundEffectInstance se = sounds[type].CreateInstance();
            se.Play();
        }

        public SoundEffect FindSound(SoundType type)
        {
            return sounds[type];
        }
    }
}
