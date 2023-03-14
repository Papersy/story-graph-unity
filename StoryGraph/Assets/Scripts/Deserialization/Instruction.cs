namespace Deserialization
{
    public class Instruction
    {
        public string Op { get; set; }
        public string Nodes { get; set; }
        public string To { get; set; }
        public string Attribute { get; set; }
        public bool? Value { get; set; }
    }
}