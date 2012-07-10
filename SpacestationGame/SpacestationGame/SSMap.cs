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
    public enum SSTileTypes
    {
        Atmos,
        BasicFloor
    }

    public class SSTile
    {
        private SSTileTypes _name;

        public SSTileTypes Name
        {
            get { return _name; }
        }

        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            private set { _displayName = value; }
        }

        private Color _displayColor;

        public Color DisplayColor
        {
            get { return _displayColor; }
            private set { _displayColor = value; }
        }

        public SSTile(SSTileTypes type, string displayName)
        {
            this._name = type;
            this._displayName = displayName;
        }

        public SSTile SetColor(Color col)
        {
            this.DisplayColor = col;
            return this;
        }

        public virtual void OnPlayerTick(SSPlayer ply)
        {

        }

        public virtual void Update(MainGame game, GameTime time)
        {
            
        }

        public virtual void Draw(MainGame game, int x, int y)
        {
            game.DrawImage(new Rectangle(x * 16, y * 16, 16, 16), this.DisplayColor);
        }

        public override string ToString()
        {
            return "Tile(" + this.DisplayName + ")";
        }
    }

    public class SSBaseTile
    {
        Atmos atmosTile;

        int maxTile = 0;
        Dictionary<int, SSTile> tiles = new Dictionary<int, SSTile>();

        public SSBaseTile(Atmos.AtmosType atmosType, SSTile floor)
        {
            atmosTile = new Atmos(atmosType);
            AddTile(0, floor);
        }

        public void AddTile(int level, SSTile tile)
        {
            tiles.Add(level, tile);
            if (level >= maxTile)
            {
                maxTile = level + 1;
            }
        }

        public override string ToString()
        {
            string ret = "";
            ret += atmosTile.ToString() + "\n";
            for (int i = 0; i < maxTile; i++)
            {
                if (tiles.ContainsKey(i))
                {
                    ret += "    " + tiles[i].ToString() + "\n";
                }
            }
            return ret;
        }

        public void Update(MainGame game, GameTime time)
        {
            atmosTile.Update(game, time);
        }

        public void Draw(MainGame game, int x, int y)
        {
            atmosTile.Draw(game, x, y);

            for (int i = 0; i < maxTile; i++)
            {
                if (tiles.ContainsKey(i))
                {
                    tiles[i].Draw(game, x, y);
                }
            }
        }
    }

    public class SSMap : Entity
    {
        public static Dictionary<SSTileTypes, SSTile> BasicTiles = null;

        SSBaseTile[,] Map;

        private int _width;

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }


        public SSMap()
        {
            if (BasicTiles == null)
            {
                BasicTiles = new Dictionary<SSTileTypes, SSTile>();
                BasicTiles.Add(SSTileTypes.BasicFloor, new SSTile(SSTileTypes.BasicFloor, "Basic Floor").SetColor(Color.SlateGray));
            }

            Generate(32, 32);
        }

        public void Generate(int width, int height)
        {
            Map = new SSBaseTile[width, height];
            this.Width = width;
            this.Height = height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Map[x, y] = new SSBaseTile(Atmos.AtmosType.Normal, BasicTiles[SSTileTypes.BasicFloor]);
                }
            }
        }

        public override void Draw(MainGame game, EntityContainer parent)
        {
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Map[x, y].Draw(game, x, y);
                }
            }
        }

        public override void Update(MainGame game, EntityContainer parent, GameTime time)
        {
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Map[x, y].Update(game, time);
                }
            }
        }
    }
}
