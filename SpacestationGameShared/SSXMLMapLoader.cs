using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

using System.Xml;
using System.IO;
using System.IO.Compression;

namespace SpacestationGame
{
    public class SSMapData
    {
        public int Width;
        public int Height;
        public List<string> Layers = new List<string>();

        public List<SSTileSetImage> TilesetImageNames = new List<SSTileSetImage>();

        public SSMapData()
        {
            this.Width = 0;
            this.Height = 0;
        }

        public SSMapData(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
    }

    public class SSTileSetImage
    {
        public int Width;
        public int Height;
        public string Filename;

        public SSTileSetImage()
        {
            Width = 0;
            Height = 0;
            Filename = "";
        }

        public SSTileSetImage(int w, int h, string filename)
        {
            Width = w;
            Height = h;
            Filename = filename;
        }
    }

    public class SSMapLayerData
    {
        public SSTileData[] Tiles;

        public SSMapLayerData(int w, int h, SSTileData bse)
        {
            Tiles = new SSTileData[w * h];
            int i = 0;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Tiles[i++] = bse;
                }
            }
        }
    }

    public struct SSTileData
    {
        public byte TType;
        public byte AtmosType;
        public byte AddedData;

        public SSTileData(SSTileTypes type, AtmosType atmos, byte add)
        {
            TType = (byte)type;
            AtmosType = (byte)atmos;
            AddedData = add;
        }
    }

    public class SSXMLMapLoader
    {

        public static string BuildBase64Data(SSMapLayerData data)
        {
            byte[] mapData = new byte[data.Tiles.Length * 3];
            for (int i = 0; i < data.Tiles.Length; i += 3)
            {
                mapData[i] = data.Tiles[i / 3].TType;
                mapData[i + 1] = data.Tiles[i / 3].AtmosType;
                mapData[i + 2] = data.Tiles[i / 3].AddedData;
            }
            MemoryStream output = new MemoryStream();
            GZipStream str = new GZipStream(output, CompressionMode.Compress);
            str.Write(mapData, 0, mapData.Length);
            str.Close();
            return Convert.ToBase64String(output.ToArray());
        }

        public static SSMapLayerData UnbuildBase64Data(int w, int h, string data)
        {
            MemoryStream input = new MemoryStream(Convert.FromBase64String(data));
            SSMapLayerData ret = new SSMapLayerData(w, h, new SSTileData(SSTileTypes.Space, AtmosType.Space, 0x00));
            GZipStream str = new GZipStream(input, CompressionMode.Decompress);
            MemoryStream output = new MemoryStream();

            str.Flush();
            str.CopyTo(output);
            str.Close();

            byte[] decompressedData = output.ToArray();

            int x = 0;

            for (int i = 0; i < decompressedData.Length; i += 3)
            {
                ret.Tiles[x].TType = decompressedData[i];
                ret.Tiles[x].AtmosType = decompressedData[i + 1];
                ret.Tiles[x].AddedData = decompressedData[i + 2];
                x++;
            }

            return ret;
        }

        public static void ParseMapXml(string filename)
        {
            FileInfo info = new FileInfo(filename);
            Console.WriteLine(info.Name);

            string fname = info.Name.Split('.')[0] + ".xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            SSMapData data = new SSMapData(0, 0);

            XmlReader reader = XmlReader.Create(filename);

            int tileWidth = 0;
            int tileHeight = 0;

            while (reader.Read())
            {
                if (!reader.IsStartElement())
                {
                    continue;
                }
                switch (reader.Name)
                {
                    case "map":
                        data = new SSMapData(Int32.Parse(reader.GetAttribute("width")), Int32.Parse(reader.GetAttribute("height")));
                        break;
                    case "tileset":
                        tileWidth = Int32.Parse(reader.GetAttribute("tilewidth"));
                        tileHeight = Int32.Parse(reader.GetAttribute("tileheight"));
                        break;
                    case "image":   
                        FileInfo imageName = new FileInfo(reader.GetAttribute("source"));
                        data.TilesetImageNames.Add(new SSTileSetImage(tileWidth, tileHeight, imageName.Name.Split('.')[0]));
                        break;
                    case "data":
                        byte[] tiledata = new byte[data.Width * data.Height * 4];
                        string[] lines = reader.ReadElementContentAsString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        SSMapLayerData data2 = new SSMapLayerData(data.Width, data.Height, new SSTileData(SSTileTypes.Space, AtmosType.Space, 0x00));

                        int i = 0;
                        for (int y = 0; y < data.Height; y++)
                        {
                            string[] tokens = lines[y].Split(',');
                            for (int x = 0; x < data.Width; x++)
                            {
                                data2.Tiles[i] = new SSTileData(SSTileTypes.GenericImage, AtmosType.Normal, (byte)Int32.Parse(tokens[x]));
                                i++;
                            }
                        }
                        data.Layers.Add(BuildBase64Data(data2));
                        break;
                    default:
                        break;
                }
            }

            reader.Close();

            XmlWriter writer = XmlWriter.Create(@"..\SpacestationGame\SpacestationGameContent\Maps\" + fname, settings);
            IntermediateSerializer.Serialize(writer, data, null);
            writer.Close();
        }
    }
}
