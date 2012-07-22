using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpacestationGame;

using System.IO;

namespace XMLMapLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string item in Directory.GetFiles("../raw_res/Maps"))
            {
                Console.WriteLine("Generating Map Resource: " + item);
                SSXMLMapLoader.ParseMapXml(item);
            }
            //Console.ReadKey();
        }
    }
}
