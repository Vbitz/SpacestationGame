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
    class SSGame : MainGame
    {
        SSMap GameMap;
        public SSPlayer LocalPlayer;

        protected override void OnInit()
        {
            this.DrawColor = new Color(20, 20, 20);
            this.UseCameraInput = false;

            GameMap = new SSMap(this);
            LocalPlayer = new SSPlayer(GameMap);
            Container.Add(GameMap);
            Container.Add(LocalPlayer);
        }

        protected override void OnUpdate(GameTime gameTime)
        {

        }

        protected override void OnDraw(GameTime gameTime)
        {

            this.DrawString(LocalPlayer.ToString(), new Vector2(20, 70), Color.White, true);
        }
    }
}
