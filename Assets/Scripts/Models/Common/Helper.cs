using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcedualLevels.Models
{
    public static class Helper
    {
        public static T Clamp<T>(T value, T min, T max)
            where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            else if (value.CompareTo(max) > 0)
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

        /// <summary>
        /// 指定した範囲内の乱数を返します。
        /// </summary>
        /// <returns>範囲内の乱数。</returns>
        /// <param name="min">乱数の最小値。</param>
        /// <param name="max">乱数の最大値。</param>
        public static int GetRandomInRange(int min, int max)
        {
            if (min > max)
            {
                Debug.LogWarning("min:" + min + " is greater than max:" + max + ".");
            }
            return (int)(UnityEngine.Random.value * (max - min)) + min;
        }

        /// <summary>
        /// 部屋の中からランダムな一点を返します。
        /// </summary>
        /// <returns>部屋の中のランダムな点。</returns>
        /// <param name="room">点が含まれる部屋。</param>
        /// <param name="margin">部屋の外周からの最小距離。</param>
        public static Vector2 GetRandomLocation(MapRectangle room, int margin)
        {
            var x = GetRandomInRange(room.Left + margin, room.Right - margin);
            var y = GetRandomInRange(room.Bottom + margin, room.Top - margin);
            return new Vector2(x + 0.5f, y + 0.5f);
        }

        public static T MinItem<T, U>(this IEnumerable<T> source, Func<T, U> selector)
            where U : IComparable<U>
        {
            var min = source.Min(selector);
            return source.First(x => selector(x).CompareTo(min) == 0);
        }

        public static T MaxItem<T, U>(this IEnumerable<T> source, Func<T, U> selector)
            where U : IComparable<U>
        {
            var max = source.Max(selector);
            return source.First(x => selector(x).CompareTo(max) == 0);
        }
    }
}