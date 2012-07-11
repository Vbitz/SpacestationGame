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

            GameMap = new SSMap();
            LocalPlayer = new SSPlayer(GameMap);
            Container.Add(GameMap);
            Container.Add(LocalPlayer);
        }

        protected override void OnUpdate(GameTime gameTime)
        {

        }

        protected override void OnDraw(GameTime gameTime)
        {
            /*
            Rectangle seenBounds = CameraBounds;
            for (int x = seenBounds.X / 64; x < (seenBounds.X + seenBounds.Width) / 64 + 2; x++)
            {
                for (int y = seenBounds.Y / 64; y < (seenBounds.Y + seenBounds.Height) / 64 + 2; y++)
                {
                    if (x >= GameMap.Width || x < 0 || y >= GameMap.Height || y < 0)
                    {
                        continue;
                    }
                    if (GameMap.Colides(new Rectangle(x * 64, y * 64, 64, 64), null))
                    {
                        DrawImage(new Rectangle(x * 64, y * 64, 64, 64), Color.Green);
                    }
                }
            }
            Checks which tiles have physics*/

            this.DrawString(LocalPlayer.ToString(), new Vector2(20, 70), Color.White, true);
        }
    }
}
