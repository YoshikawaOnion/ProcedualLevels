using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace ProcedualLevels.Common
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

        public static float GetRandomInRange(float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentException("min は max より小さい必要があります。");
            }
            return (UnityEngine.Random.value * (max - min)) + min;
        }

        /// <summary>
        /// 指定された数値の符号を取得します。指定された数値が0に限りなく近ければ0を返します。
        /// </summary>
        /// <returns>指定した数値の符号。</returns>
        /// <param name="value">符号を判定する数値。</param>
        /// <param name="threshould">数値を0に限りなく近いと判定するために用いる誤差の大きさ。</param>
        public static int Sign(float value, float threshould = float.Epsilon)
        {
            return Mathf.Abs(value) <= threshould ? 0 : (int)Mathf.Sign(value);
        }

        /// <summary>
        /// コレクションからランダムに一つの要素を返します。
        /// </summary>
        /// <returns>ランダムに選ばれた要素。</returns>
        /// <param name="source">選ぶ元となるコレクション。</param>
        public static T GetRandom<T>(this IEnumerable<T> source)
        {
            if (source.Any())
			{
				var index = Helper.GetRandomInRange(0, source.Count() - 1);
				return source.ElementAt(index);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 指定したストリームに値が発行されたとき、このストリームを一定時間停止します。
        /// </summary>
        /// <returns>新しいストリーム。</returns>
        /// <param name="source">一定時間停止するストリーム。</param>
        /// <param name="pause">値を流すことによってストリームの停止タイミングを決定するストリーム。</param>
        /// <param name="pauseTime">ストリームを停止する時間。</param>
        public static IObservable<T> PauseBy<T, U>(this IObservable<T> source,
                                                   IObservable<U> pause,
                                                   TimeSpan pauseTime)
        {
            var resume = Observable.Timer(pauseTime)
                                   .SelectMany(x => PauseBy(source, pause, pauseTime));
            return source.TakeUntil(pause)
                         .Concat(resume);
        }
    }
}