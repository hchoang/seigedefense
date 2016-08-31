using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace SiegeDefense.GameComponents.Models
{
    public class UserControlledTank : Tank
    {
        public int point { get; private set; }

        public UserControlledTank(Model model) : base(model)
        {
        }

        public void earnPoint(int point)
        {
            this.point += point;
        }
    }
}
