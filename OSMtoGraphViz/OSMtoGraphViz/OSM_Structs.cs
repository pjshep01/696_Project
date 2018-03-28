namespace OSM_Structs
{
    public class lat_long
    {
        public string Lat { get; set; }
        public string Lon { get; set; }
        public lat_long(string lat, string lon)
        {
            Lat = lat;
            Lon = lon;
        }
    }
    public class Min_Max
    {
        public float min_x { get; set; }
        public float max_x { get; set; }
        public float min_y { get; set; }
        public float max_y { get; set; }
        public bool status { get; set; }
        public Min_Max(float minx, float maxx, float miny, float maxy)
        {
            min_x = minx;
            max_x = maxx;
            min_y = miny;
            max_y = min_y;
        }
    }
}