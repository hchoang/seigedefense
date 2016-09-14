using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace SiegeDefense
{
    public enum SoundType {
        TankFire,
        InBattleBGM
    }
    public class SoundBankManager : GameObject
    {
        private Dictionary<SoundType, SoundEffect> sounds;

        public SoundBankManager()
        {
            sounds = new Dictionary<SoundType, SoundEffect>();

            sounds.Add(SoundType.TankFire, Game.Content.Load<SoundEffect>(@"Sound/tank-fire"));
            sounds.Add(SoundType.InBattleBGM, Game.Content.Load<SoundEffect>(@"Sound/in-battle-bgm"));
        }

        protected override void LoadContent()
        {
            
            //sounds.Add("TankMoving", Game.Content.Load<SoundEffect>(@"").Duration());
            //sounds.Add()
            base.LoadContent();
        }

        public void PlaySound(SoundType type) {
            SoundEffectInstance se = sounds[type].CreateInstance();
            se.Volume = 0.2f;
            se.Play();
        }

        public SoundEffect FindSound(SoundType type)
        {
            return sounds[type];
        }
    }
}
