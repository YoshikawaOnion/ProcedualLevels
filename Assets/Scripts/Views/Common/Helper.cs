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
        public static float RandomInRange(float min, float max)
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
				var index = Models.Helper.GetRandomInRange(0, source.Count() - 1);
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