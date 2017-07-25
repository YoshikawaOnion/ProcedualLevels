using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class Helper
{
    public static T Clamp<T>(T value, T min, T max)
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0)
        {
            return min;
        }
        else if(value.CompareTo(max) > 0)
        {
            return max;
        }
        else
        {
            return value;
        }
    }

    public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T value)
    {
        foreach (var item in source)
        {
            yield return item;
        }
        yield return value;
    }
}
