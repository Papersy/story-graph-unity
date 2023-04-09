using System.Collections.Generic;
using LocationDir;

namespace Deserialization
{
    public class World
    {
        public string Id { get; set; }//
        public string Name { get; set; }//
        public List<Character> Characters { get; set; }//Person NPCes//
        public List<Connection> Connections { get; set; }//
        public Attributes Attributes { get; set; }//
        public List<Item> Items { get; set; }//
    }
}