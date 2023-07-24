using System;
using System.Collections.Generic;

[Serializable]
public class Dialog
{
    public string npc_name;
    public List<DialogContainer> dialog = new List<DialogContainer>();
}

[Serializable]
public class DialogContainer
{
    public int side;
    public string name;
    public string message;
}