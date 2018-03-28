using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OSMtoGraphViz
{
    class Functions
    {
        public static string Check_Filename(string path, int ver)
        {
            //Console.WriteLine("the path: " +path);
            string newpath;
            var basepath = path.TrimStart('.').Split('.');
            var version = "." + basepath[0] +ver+ "." + basepath[1];
            if (!File.Exists(version))
            {
                // Create a file to write to.
                string createText = "digraph G{\r\nnode[fontsize=.5,height =.1,width=.1,fixedsize=true];\r\nedge[arrowsize=.1];\r\n";
                File.WriteAllText(version, createText);
                newpath = version;
            }
            else
            {

                var split = path.TrimStart('.').Split('.');
                var cs = "." + split[0] + "." + split[1];
                newpath = Check_Filename(cs, ver + 1);
            }
            return newpath;
        }
        public static OSM_Structs.Min_Max GetMinMax(string path)
        {
            OSM_Structs.Min_Max mm = new OSM_Structs.Min_Max(-40.0f,-160.0f,69.0f,20.0f);
            if(!File.Exists(path))
            {
                mm.status = false;
                return mm;
            }
            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("node"))
                            {
                                if (reader.HasAttributes)
                                {
                                    String lt = reader.GetAttribute("lat");
                                    String lg = reader.GetAttribute("lon");
                                    float t_lt;
                                    float t_lg;
                                    if (float.TryParse(lt, out t_lt) && float.TryParse(lg, out t_lg))
                                    {
                                        if (t_lg > mm.max_x)
                                        {
                                            mm.max_x = t_lg;
                                        }
                                        if (t_lg < mm.min_x)
                                        {
                                            mm.min_x = t_lg;
                                        }
                                        if (t_lt > mm.max_y)
                                        {
                                            mm.max_y = t_lt;
                                        }
                                        if (t_lt < mm.min_y)
                                        {
                                            mm.min_y = t_lt;
                                        }
                                    }

                                }
                            }
                            else if (reader.Name.Equals("way"))
                            {
                                break;
                            }
                            break;

                    }
                }
            }
            mm.status = true;
            return mm;
        }
        public static void Build_Dot_File(
            List<string> store,
            string path,
            OSM_Structs.Min_Max mm,
            double size,
            string out_path
        )
        {
            string first_point = null;
            string second_point = null;
            float Z = mm.min_x - mm.max_x;
            float C = mm.min_y - mm.max_y;
            using (XmlReader reader = XmlReader.Create(path))
            {
                while (reader.Read())
                {
                    if(store.Count > 500000)
                    {
                        Console.WriteLine("Here");
                        File.AppendAllLines(out_path, store);
                        store.Clear();
                    }
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("node"))
                            {
                                if (reader.HasAttributes)
                                {
                                    String lt = reader.GetAttribute("lat");
                                    String lg = reader.GetAttribute("lon");
                                    float flt;
                                    float flg;
                                    float.TryParse(lt, out flt);
                                    float.TryParse(lg, out flg);
                                    float x = ((mm.max_x - flg) / (Z)) * (float)size;
                                    float y = ((mm.max_y - flt) / (C)) * (float)size;
                                    //Console.WriteLine(x + " " + y);
                                    store.Add(reader.GetAttribute("id") + " [pos = \"" + x + "," + y + "!\"];");
                                }
                            }
                            else if (reader.Name.Equals("nd"))
                            {
                                if (first_point == null)
                                {
                                    first_point = reader.GetAttribute("ref");
                                }
                                else
                                {
                                    second_point = reader.GetAttribute("ref");
                                    store.Add(first_point + " -> " + second_point + " [];");
                                    first_point = second_point;
                                    second_point = null;
                                }
                            }
                            break;

                        case XmlNodeType.EndElement:
                            if (reader.Name.Equals("way"))
                            {
                                first_point = null;
                                second_point = null;
                            }
                            break;
                    }
                }
            }
        }
    }
}