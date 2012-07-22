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

namespace Vbitz
{
    /// <summary>
    /// Tileset class, the tiles are indexed left to right top to bottom
    /// </summary>
    public class Tileset
    {
        private Texture2D _tilesetImage;

        private int _width;

        public int Width
        {
            get { return _width; }
            private set { _width = value; }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            private set { _height = value; }
        }

        private int _tileWidth;

        public int TileWidth
        {
            get { return _tileWidth; }
            private set { _tileWidth = value; }
        }

        private int _tileHeight;

        public int TileHeight
        {
            get { return _tileHeight; }
            private set { _tileHeight = value; }
        }


        public Tileset(MainGame game, string imageName, int tileWidth, int tileHeight)
        {
            this._tilesetImage = game.GetTexture(imageName);
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
            this.Width = this._tilesetImage.Width / tileWidth;
            this.Height = this._tilesetImage.Height / tileHeight;
        }

        public void DrawTile(MainGame game, int x, int y, int index)
        {
            if (index == 0)
            {
                return;
            }
            Rectangle rect = new Rectangle(x * this.TileWidth, y * this.TileHeight, this.TileWidth, this.TileHeight);
            game.DrawImage(_tilesetImage, rect, GetTileRect(index), Color.White);
        }

        public Rectangle GetTileRect(int index_r)
        {
            int index = index_r - 1;
            int y = index / this.Width;
            int x = index - (y * this.Width);
            //return new Rectangle(32, 0, 32, 32);
            return new Rectangle(x * this.TileWidth, y * this.TileHeight, this.TileWidth, this.TileHeight);
        }
    }
}
