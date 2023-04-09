using System.Collections.Generic;
using LocationDir;

namespace Deserialization
{
    public class Character //Person NPC
    {
        public string Id { get; set; }//
        public string Name { get; set; }//
        public Attributes Attributes { get; set; }//
        public List<Item> Items { get; set; }//
        public List<string> characters { get; set; }//??? not same register
        public string knowledge { get; set; }
        public string sound { get; set; }
    }
}