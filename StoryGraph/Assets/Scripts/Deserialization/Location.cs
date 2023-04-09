using System;
using System.Collections.Generic;
using LocationDir;

public class Location
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Characters> Characters { get; set; }
    public List<Connection> Connections { get; set; }
    public Attributes Attributes { get; set; }
    public List<Item> Items { get; set; }
}