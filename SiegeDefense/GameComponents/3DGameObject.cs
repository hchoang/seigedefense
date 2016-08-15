using Microsoft.Xna.Framework;
using SiegeDefense.GameComponents.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameComponents {
    public abstract class _3DGameObject : GameObject {
        public virtual Matrix TranslationMatrix { get; set; } = Matrix.Identity;
        public virtual Matrix RotationMatrix { get; set; } = Matrix.Identity;
        public virtual Matrix ScaleMatrix { get; set; } = Matrix.Identity;
        public virtual Matrix WorldMatrix {
            get {
                return ScaleMatrix * RotationMatrix * TranslationMatrix;
            }
            set {
                TranslationMatrix = Matrix.CreateTranslation(value.Translation);
                RotationMatrix = Matrix.CreateFromQuaternion(value.Rotation);
                ScaleMatrix = Matrix.CreateScale(value.Scale);
            }
        }
        public virtual Vector3 Position {
            get {
                return TranslationMatrix.Translation;
            }
            set {
                TranslationMatrix = Matrix.CreateTranslation(value);
            }
        }
        public virtual Vector3 Up {
            get {
                return WorldMatrix.Up;
            }
            set {
                WorldMatrix = Matrix.CreateWorld(Position, Forward, value);
            }
        }
        public virtual Vector3 Forward {
            get {
                return Vector3.Normalize(WorldMatrix.Forward);
            }
            set {
                WorldMatrix = Matrix.CreateWorld(Position, value, Up);
            }
        }
        public virtual Vector3 Left {
            get {
                return Vector3.Normalize(WorldMatrix.Left);
            }
        }
    }
}
