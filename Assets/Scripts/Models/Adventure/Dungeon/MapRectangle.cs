using UnityEngine;
using System.Collections;

public class MapRectangle
{
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }
    public string Name { get; set; }

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

    public MapRectangle ReduceOnEdge(int reduceAmount)
    {
        return new MapRectangle
        {
	        Left = Left + reduceAmount,
	        Right = Right - reduceAmount,
	        Bottom = Bottom + reduceAmount,
	        Top = Top - reduceAmount,
        };
    }

    public override string ToString()
    {
        return string.Format("[MapRectangle: Left={0}, Right={1}, Top={2}, Bottom={3}]", Left, Right, Top, Bottom);
    }

    public bool IsInside(float x, float y)
    {
        return x >= Left && x < Right && y >= Bottom && y < Top;
    }

    public bool IsIntersect(MapRectangle other)
    {
        return Left <= other.Right && Right >= other.Left
                            && Bottom <= other.Top && Top >= other.Bottom;
    }

	/// <summary>
	/// 指定した軸に沿った正の方向の端の座標を取得します。
	/// </summary>
	/// <returns>指定した軸の正の方向の端の座標。</returns>
	/// <param name="horizontal"><c>true</c> なら水平方向、<c>false</c>なら鉛直方向。</param>
	public int GetMajor(bool horizontal)
    {
        return horizontal ? Right : Top;
    }

	/// <summary>
	/// 指定した軸に沿った負の方向の端の座標を取得します。
	/// </summary>
	/// <returns>指定した軸の負の方向の端の座標。</returns>
	/// <param name="horizontal"><c>true</c> なら水平方向、<c>false</c>なら鉛直方向。</param>
	public int GetMinor(bool horizontal)
    {
        return horizontal ? Left : Bottom;
    }

	/// <summary>
	/// 指定した軸に沿った正の方向の端の座標を取得します。
	/// </summary>
	/// <param name="horizontal"><c>true</c> なら水平方向、<c>false</c>なら鉛直方向。</param>
	/// <param name="value">設定する値。</param>
	public void SetMajor(bool horizontal, int value)
    {
        if (horizontal)
        {
            Right = value;
        }
        else
        {
            Top = value;
        }
    }

	/// <summary>
	/// 指定した軸に沿った負の方向の端の座標を取得します。
	/// </summary>
	/// <param name="horizontal"><c>true</c> なら水平方向、<c>false</c>なら鉛直方向。</param>
	/// <param name="value">設定する値。</param>
	public void SetMinor(bool horizontal, int value)
    {
        if (horizontal)
        {
            Left = value;
        }
        else
        {
            Bottom = value;
        }
    }
}
