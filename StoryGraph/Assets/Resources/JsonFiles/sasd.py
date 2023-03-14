// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Attributes
    {
        public int HP { get; set; }
public int Money { get; set; }
public bool? IsAuthority { get; set; }
public int? NutritionalValue { get; set; }
public bool? IsTroublemaker { get; set; }
public int Value { get; set; }
public bool? IsPoison { get; set; }
public bool? IsRotten { get; set; }
public bool IsPrivate { get; set; }
}

public class Attributes8
    {
        public int HP { get; set; }
public int Money { get; set; }
public int Value { get; set; }
}

public class AvailableProduction
    {
        public Prod prod { get; set; }
public List<List<>> variants { get; set; }
}

public class Character
    {
        public string Id { get; set; }
public bool IsObject { get; set; }
public Attributes Attributes { get; set; }
public List<object> Characters { get; set; }
public List<Item> Items { get; set; }
public string Name { get; set; }
}

public class Character3
    {
        public string name { get; set; }
public Attributes attributes { get; set; }
public List<Item> items { get; set; }
public List<object> characters { get; set; }
public object knowledge { get; set; }
public object sound { get; set; }
}

public class Connection
    {
        public Destination Destination { get; set; }
}

public class Destination
    {
        public string Id { get; set; }
}

public class Instruction
    {
        public string Op { get; set; }
public string Nodes { get; set; }
public string To { get; set; }
public string Attribute { get; set; }
public bool? Value { get; set; }
}

public class Item
    {
        public string Id { get; set; }
public Attributes Attributes { get; set; }
public List<object> Characters { get; set; }
public List<object> Items { get; set; }
public string Name { get; set; }
}

public class Item4
    {
        public string name { get; set; }
public Attributes attributes { get; set; }
}

public class Location
    {
        public string Id { get; set; }
public Attributes Attributes { get; set; }
public List<Character> Characters { get; set; }
public List<object> Items { get; set; }
public List<Connection> Connections { get; set; }
}

public class LocationInfo
    {
        public string desc { get; set; }
public List<object> items { get; set; }
public List<Character> characters { get; set; }
public string main_location_id { get; set; }
}

public class LSide
    {
        public List<Location> Locations { get; set; }
}

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

public class Root
    {
        public List<AvailableProduction> available_productions { get; set; }
public List<World> world { get; set; }
public object changed_nodes { get; set; }
public object message { get; set; }
public string game_status { get; set; }
public LocationInfo location_info { get; set; }
public string main_character { get; set; }
}

public class RSide
    {
    }

public class World
    {
        public string Id { get; set; }
public string Name { get; set; }
public List<Character> Characters { get; set; }
public List<Connection> Connections { get; set; }
public Attributes Attributes { get; set; }
public List<Item> Items { get; set; }
}

