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
        protected Vector2 LocationF = new Vector2(-300, -300);

        protected void Move(float x, float y)
        {
            LocationF = Vector2.Add(LocationF, new Vector2(x, y));
        }

        protected Rectangle MoveSimulate(float x, float y)
        {
            return new Rectangle((int)(LocationF.X + x), (int)(LocationF.Y + y), 32, 32);
        }

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
