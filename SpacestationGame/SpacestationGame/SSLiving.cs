using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Vbitz;

namespace SpacestationGame
{
    public class SSLiving : Entity
    {
        private float _OxygenLevel = 1.0f;

        public float OxygenLevel
        {
            get { return _OxygenLevel; }
            set { _OxygenLevel = value; }
        }

        private float _Health = 100.0f;

        public float Health
        {
            get { return _Health; }
            set { _Health = value; }
        }
    }
}
