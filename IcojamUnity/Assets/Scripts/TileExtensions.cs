using UnityEngine.Tilemaps;

/// <summary>
/// Extends TileBase for more fun methods
/// </summary>
public static class TileExtensions
{
    public static bool TestMethod(this TileBase i)
    {
        return i.name == "bob";
    }
}
