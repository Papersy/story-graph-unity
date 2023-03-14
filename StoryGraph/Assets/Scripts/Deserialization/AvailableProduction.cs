using System.Collections.Generic;

namespace Deserialization
{
    public class AvailableProduction
    {
        public Prod prod { get; set; }
        public List<List<Variant>> variants { get; set; }
    }

    public class Variant
    {
        public string LSNodeRef { get; set; }
        public string WorldNodeId { get; set; }
        public string WorldNodeName { get; set; }
        public Attributes WorldNodeAttr { get; set; }
    }
}