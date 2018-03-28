using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OSRMRouteCall
{
    class csv_Layout
    {
        public string start_lat { get; set; }
        public string start_long { get; set; }
        public string end_lat { get; set; }
        public string end_long { get; set; }
        public string csv_dist { get; set; }
        public double calc_dist { get; set; }
        public csv_Layout(string slat, string slong, string elat, string elong, string tdist, double cdist)
        {
            start_lat = slat;
            start_long = slong;
            end_lat = elat;
            end_long = elong;
            csv_dist = tdist;
            calc_dist = cdist;
        }
        public string Join()
        {
            return string.Join(",", start_lat, start_long, end_lat, end_long, csv_dist, calc_dist);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<csv_Layout> store = new List<csv_Layout>();
            File.WriteAllText(".\\out.csv", "start_lat, start_long, end_lat, end_long, csv_distance, calculated_distance\r\n");
            using (var reader = new StreamReader(@"C:\Users\admin\Downloads\yellow_tripdata_2009-01.csv"))
            {
                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    //Console.WriteLine(values + " " + line);
                    //Console.ReadLine();
                    if (values.Count() < 7)
                    {
                        continue;
                    }
                    csv_Layout current_line = new csv_Layout(values[6], values[5], values[10], values[9], values[4],0);

                    string URL = @"http://127.0.0.1:5000/route/v1/driving/" + values[5] + "," + values[6] + ";" + values[9] + "," + values[10] + "?steps=true";
                    //Console.WriteLine(URL);
                    //client.BaseAddress = new Uri(URL);
                    if (values[6] == "Start_Lat")
                        continue;

                    HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(URL);
                    webrequest.Method = "GET";
                    webrequest.ContentType = "application/x-www-form-urlencoded";
                    HttpWebResponse webresponse;
                    try
                    {
                        webresponse = (HttpWebResponse)webrequest.GetResponse();
                    }
                    catch
                    {
                        continue;
                    }
                    Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                    string result = string.Empty;
                    result = responseStream.ReadToEnd();
                    //Console.Write(result);
                    //Console.ReadLine();
                    webresponse.Close();
                    try
                    {
                        dynamic json = JsonConvert.DeserializeObject(result);
                        current_line.calc_dist = (((double)json.routes[0].distance * .00062137));
                    }
                    catch
                    {
                        current_line.calc_dist = 0;
                    }
                    
                    store.Add(current_line);
                    //return result;
                    if (store.Count > 100000)
                    {
                        StringBuilder to_file = new StringBuilder();
                        foreach(var entry in store)
                        {
                            to_file.AppendLine(entry.Join());
                        }
                        File.AppendAllText(".\\out.csv", to_file.ToString());
                        store.Clear();
                    }
                }
            }
            foreach(var entry in store)
            {
                StringBuilder to_file = new StringBuilder();
                foreach (var e in store)
                {
                    to_file.AppendLine(e.Join());
                }
                File.AppendAllText(".\\out.csv", to_file.ToString());
                store.Clear();
            }

        }
    }
}
