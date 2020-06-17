#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
using UnityEngine;

public struct Player
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
{
    public int id;
    public uint hexColor;
    public string name;

    public PlayerClasses playerClass;
    public Vector2Int pos;


    public static bool operator !=(Player a, Player b)
    {
        return !(a == b);
    }

    public static bool operator ==(Player a, Player b)
    {
        return ((a.id == b.id) && (a.hexColor == b.hexColor) && (a.name == b.name));
    }
}

public enum PlayerClasses
{
    Wizard = 0,
    Knight,
    Archer,
    Rogue
}
