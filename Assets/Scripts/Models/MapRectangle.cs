using UnityEngine;
using System.Collections;

public class MapRectangle
{
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }

    public Vector2 Position
    {
        get { return new Vector2(Left, Bottom); }
    }

    public Vector2 Size
    {
        get { return new Vector2(Width, Height); }
    }

    public int Width
    {
        get { return Right - Left; }
    }
    public int Height
    {
        get { return Top - Bottom; }
    }

    public MapRectangle()
        : this(0, 0, 0, 0)
    {
    }

    public MapRectangle(int left, int right, int bottom, int top)
    {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
    }

    public MapRectangle Clone()
    {
        return new MapRectangle(Left, Right, Bottom, Top);
    }
}
