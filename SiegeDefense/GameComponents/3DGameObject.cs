using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiegeDefense.GameComponents {
    public abstract class _3DGameObject : GameObject {
        public Matrix TranslationMatrix { get; set; } = Matrix.Identity;
        public Matrix RotationMatrix { get; set; } = Matrix.Identity;
        public Matrix ScaleMatrix { get; set; } = Matrix.Identity;
        public Matrix WorldMatrix {
            get {
                return ScaleMatrix * RotationMatrix * TranslationMatrix;
            }
        }
    }
}
