using System.Collections.Generic;
using Deserialization;

public class Root
{
    // public List<AvailableProduction> available_productions { get; set; }
    public List<World> world { get; set; }//
    public string changed_nodes { get; set; }//
    public string message { get; set; }//
    public string game_status { get; set; }//
    // public LocationInfo location_info { get; set; }
    public string main_character { get; set; }
}