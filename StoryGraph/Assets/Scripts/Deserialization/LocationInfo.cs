using System.Collections.Generic;
using LocationDir;

namespace Deserialization
{
    public class LocationInfo
    {
        public string desc { get; set; }//
        public List<Item> items { get; set; }//
        public List<Character> characters { get; set; }
        public string main_location_id { get; set; }
    }
}