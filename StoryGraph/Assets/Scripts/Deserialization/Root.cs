using System.Collections.Generic;

public class Root
{
    public string Title { get; set; }
    public string TitleGeneric { get; set; }
    public string Description { get; set; }
    public int Override { get; set; }
    public LSide LSide  { get; set; }
    public RSide RSide { get; set; }
    public List<object> Instructions { get; set; }
}