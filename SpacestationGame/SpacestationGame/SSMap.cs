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
        BasicFloor,
        BasicWall
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

        private bool _physics;

        public bool Physics
        {
            get { return _physics; }
            private set { _physics = value; }
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

        public SSTile SetPhysics()
        {
            this.Physics = true;
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

        public bool PhysicsTest(Rectangle rect, int x, int y)
        {
            if (!rect.Contains(new Rectangle(x * 16, y * 16, 16, 16)))
            {
                return false;
            }
            for (int i = maxTile; i > 0; i--)
            {
                if (tiles.ContainsKey(i))
                {
                    if (tiles[i].Physics)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class SSMap : Entity, IPhysicsProvider
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
                BasicTiles.Add(SSTileTypes.BasicWall, new SSTile(SSTileTypes.BasicWall, "Basic Wall").SetColor(Color.DimGray).SetPhysics());
            }

            Generate(512, 512);
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
                    Map[x, y] = new SSBaseTile(Atmos.AtmosType.Normal, BasicTiles[SSTileTypes.BasicWall]);
                }
            }

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    Map[x, y] = new SSBaseTile(Atmos.AtmosType.Normal, BasicTiles[SSTileTypes.BasicFloor]);
                }
            }

            Random rand = new Random();

            for (int i = 0; i < 5000; i++)
            {
                Map[rand.Next(width), rand.Next(height)] = new SSBaseTile(Atmos.AtmosType.Normal, BasicTiles[SSTileTypes.BasicWall]);
            }
        }

        public override void Draw(MainGame game, EntityContainer parent)
        {
            Rectangle seenBounds = game.CameraBounds;
            for (int x = seenBounds.X / 16; x < (seenBounds.X + seenBounds.Width) / 16 + 2; x++)
            {
                for (int y = seenBounds.Y / 16; y < (seenBounds.Y + seenBounds.Width) / 16 + 2; y++)
                {
                    if (x >= Width || x < 0 || y >= Height || y < 0)
                    {
                        continue;
                    }
                    Map[x, y].Draw(game, x, y);
                }
            }
        }

        public override void Update(MainGame game, EntityContainer parent, GameTime time)
        {
            Rectangle seenBounds = game.CameraBounds;
            for (int x = seenBounds.X / 16; x < (seenBounds.X + seenBounds.Width) / 16 + 2; x++)
            {
                for (int y = seenBounds.Y / 16; y < (seenBounds.Y + seenBounds.Width) / 16 + 1 + 2; y++)
                {
                    if (x >= Width || x < 0 || y >= Height || y < 0)
                    {
                        continue;
                    }
                    Map[x, y].Update(game, time);
                }
            }
        }

        public bool Colides(Rectangle src, SSLiving ent)
        {
            for (int x = (src.X / 16); x < ((src.X + src.Width) / 16); x++)
            {
                for (int y = (src.Y / 16); y < ((src.X + src.Width) / 16); y++)
                {
                    //Console.WriteLine(-x + " : " + -y);
                    if (-x >= Width || -x < 0 || -y >= Height || -y < 0)
                    {
                        return true;
                    }
                    SSBaseTile blk = Map[-x, -y];
                    if (blk.PhysicsTest(src, x, y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
