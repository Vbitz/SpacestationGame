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
    class SSGenericImageTile : SSTile
    {
        private int tileIndex;
        private Tileset tileSet;

        public SSGenericImageTile(int tileIndex, Tileset ts) 
            : base(SSTileTypes.GenericImage, "TODO : Replace Me")
        {
            //Console.WriteLine(tileIndex);
            this.tileIndex = tileIndex;
            this.tileSet = ts;
        }

        public override void Draw(MainGame game, int x, int y)
        {
            tileSet.DrawTile(game, x, y, tileIndex);
        }
    }
}