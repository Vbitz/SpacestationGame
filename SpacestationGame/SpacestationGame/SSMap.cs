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

        private bool _noDraw = false;

        public bool NoDraw
        {
            get { return _noDraw; }
            set { _noDraw = value; }
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
            if (this.NoDraw)
            {
                return;
            }
            game.DrawImage(new Rectangle(x * SSMap.TileSize, y * SSMap.TileSize, SSMap.TileSize, SSMap.TileSize), this.DisplayColor);
        }

        public override string ToString()
        {
            return "Tile(" + this.DisplayName + ")";
        }

        public SSTile SetNoDraw()
        {
            this.NoDraw = true;
            return this;
        }
    }

    public class SSBaseTile
    {
        Atmos atmosTile;

        int maxTile = 0;
        Dictionary<int, SSTile> tiles = new Dictionary<int, SSTile>();

        public SSBaseTile(AtmosType atmosType, SSTile floor)
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
            //Console.WriteLine(rect.ToString() + " : " +  x + " : " + y);
            if (!rect.Intersects(new Rectangle(x * SSMap.TileSize, y * SSMap.TileSize, SSMap.TileSize, SSMap.TileSize)))
            {
                return false;
            }
            if (tiles.Count == 1)
            {
                int[] keys = new int[8];
                tiles.Keys.CopyTo(keys, 0);
                if (tiles[keys[0]].Physics)
                {
                    return true;
                }
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

    public class SSMap : Entity, ISSPhysicsProvider
    {
        public const int TileSize = 32;

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

        public List<Tileset> TileImages = new List<Tileset>();


        public SSMap(Vbitz.MainGame game)
        {
            if (BasicTiles == null)
            {
                BasicTiles = new Dictionary<SSTileTypes, SSTile>();
                BasicTiles.Add(SSTileTypes.BasicFloor, new SSTile(SSTileTypes.BasicFloor, "Basic Floor").SetColor(Color.SlateGray));
                BasicTiles.Add(SSTileTypes.BasicWall, new SSTile(SSTileTypes.BasicWall, "Basic Wall").SetColor(Color.DimGray).SetPhysics());
                BasicTiles.Add(SSTileTypes.Space, new SSTile(SSTileTypes.Space, "Space").SetNoDraw().SetPhysics());
            }

            //Generate(512, 512);

            LoadMap("Maps\\spacestationtest", game);
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
                    Map[x, y] = new SSBaseTile(AtmosType.Normal, BasicTiles[SSTileTypes.BasicWall]);
                }
            }

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    Map[x, y] = new SSBaseTile(AtmosType.Normal, BasicTiles[SSTileTypes.BasicFloor]);
                }
            }

            Random rand = new Random();

            for (int i = 0; i < 20000; i++)
            {
                Map[rand.Next(width), rand.Next(height)] = new SSBaseTile(AtmosType.Normal, BasicTiles[SSTileTypes.BasicWall]);
            }
        }

        public void LoadMap(string path, Vbitz.MainGame game)
        {
            SSMapData data = game.Content.Load<SSMapData>(path);

            Map = new SSBaseTile[data.Width, data.Height];

            this.Width = data.Width;
            this.Height = data.Height;

            this.TileImages.Add(new Tileset(game, data.TilesetImageNames[0].Filename, data.TilesetImageNames[0].Width, data.TilesetImageNames[0].Height));

            SSMapLayerData layerData = SSXMLMapLoader.UnbuildBase64Data(this.Width, this.Height, data.Layers[0]);

            //Console.WriteLine(layerData.Tiles[4].AddedData);

            int i = 0;

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.Map[x, y] = new SSBaseTile((AtmosType)layerData.Tiles[i].AtmosType, new SSGenericImageTile(layerData.Tiles[i].AddedData, this.TileImages[0]));
                    i++;
                }
            }
        }

        public override void Draw(MainGame game, EntityContainer parent)
        {
            Rectangle seenBounds = game.CameraBounds;
            for (int x = seenBounds.X / TileSize; x < (seenBounds.X + seenBounds.Width) / TileSize + 2; x++)
            {
                for (int y = seenBounds.Y / TileSize; y < (seenBounds.Y + seenBounds.Height) / TileSize + 2; y++)
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
            for (int x = seenBounds.X / TileSize; x < (seenBounds.X + seenBounds.Width) / TileSize + 2; x++)
            {
                for (int y = seenBounds.Y / TileSize; y < (seenBounds.Y + seenBounds.Height) / TileSize + 1 + 2; y++)
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
            // Physics is disabled until the map loading is finished
            return false;
            Rectangle rect = new Rectangle(-src.X, -src.Y, src.Width, src.Height);
            for (int x = (rect.X / TileSize) - 2; x < ((rect.X + rect.Width) / TileSize) + 2; x++)
            {
                for (int y = (rect.Y / TileSize) - 2; y < ((rect.Y + rect.Height) / TileSize) + 2; y++)
                {
                    if (x >= Width || x < 0 || y >= Height || y < 0)
                    {
                        continue;
                    }
                    SSBaseTile blk = Map[x, y];
                    if (blk.PhysicsTest(rect, x, y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
