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
    public class SSPlayer : SSLiving
    {
        private IPhysicsProvider Physics;

        public SSPlayer(IPhysicsProvider prov)
        {
            Physics = prov;
            this.LocationF = new Vector2(-(MainGame.WindowWidth / 2 - 8), -(MainGame.WindowHeight / 2 - 8));
        }

        public override void Update(MainGame game, EntityContainer parent, GameTime time)
        {
            if (game.IsKeyDown(Keys.W) || game.IsKeyDown(Keys.Up))
            {
                DoMove(game, 0, (float)time.ElapsedGameTime.Milliseconds / 5);
            }

            if (game.IsKeyDown(Keys.S) || game.IsKeyDown(Keys.Down))
            {
                DoMove(game, 0, -(float)time.ElapsedGameTime.Milliseconds / 5);
            }

            if (game.IsKeyDown(Keys.A) || game.IsKeyDown(Keys.Left))
            {
                DoMove(game, (float)time.ElapsedGameTime.Milliseconds / 5, 0);
            }

            if (game.IsKeyDown(Keys.D) || game.IsKeyDown(Keys.Right))
            {
                DoMove(game, -(float)time.ElapsedGameTime.Milliseconds / 5, 0);
            } 
        }

        private void DoMove(MainGame game, float x, float y)
        {
            if (Physics.Colides(MoveSimulate(x,y), this))
            {
                return;
            }
            Move(x, y);
            game.MoveCamera(x, y);
        }

        public override void Draw(MainGame game, EntityContainer parent)
        {
            game.DrawImage(new Rectangle(MainGame.WindowWidth / 2 - 8, MainGame.WindowHeight / 2 - 8, LivingEntSize, LivingEntSize), Color.RoyalBlue, true);
        }

        public override string ToString()
        {
            return "Player (" + LocationF.ToString() + ")";
        }
    }
}
