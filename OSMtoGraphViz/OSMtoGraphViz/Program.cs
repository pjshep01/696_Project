using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OSMtoGraphViz
{
    class Program
    {
        static void Main(string[] args)
        {
            double size = 4096;
            string path = ".\\louisville.dot";
            string osm_path = "C:\\Users\\pjshep01\\Downloads\\louisville.osm";
            List<string> store = new List<string>();
            
            path = Functions.Check_Filename(path, 0);
            OSM_Structs.Min_Max MM = Functions.GetMinMax(osm_path);

            if(!MM.status)
            {
                Console.WriteLine("The file does {0} not exist.", osm_path);
                return;
            }

            Functions.Build_Dot_File(store, osm_path, MM, size,path);
            File.AppendAllLines(path, store);
            File.AppendAllText(path, "}");
        }
    }
    
}