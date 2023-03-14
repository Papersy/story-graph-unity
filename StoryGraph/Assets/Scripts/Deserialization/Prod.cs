using System.Collections.Generic;

namespace Deserialization
{
    public class Prod
    {
        public string Title { get; set; }
        public string TitleGeneric { get; set; }
        public string Description { get; set; }
        public int Override { get; set; }
        public LSide LSide { get; set; }
        public RSide RSide { get; set; }
        public List<Instruction> Instructions { get; set; }
        public List<object> Preconditions { get; set; }
    }
}