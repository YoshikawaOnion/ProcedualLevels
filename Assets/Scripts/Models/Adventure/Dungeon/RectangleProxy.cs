using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Common;
using UnityEngine;

namespace ProcedualLevels.Models
{
    public static class AxisHelper
    {
        public static void GetAligned(bool isHorizontal,
                               int horizontal,
                               int vertical,
                               out int primal,
                               out int second)
        {
            primal = isHorizontal ? horizontal : vertical;
            second = isHorizontal ? vertical : horizontal;
        }
    }

    /// <summary>
    /// MapRectangle を、注目している軸に対して操作するプロキシ。
    /// </summary>
    public struct RectangleProxy
    {
        public MapRectangle Original { get; set; }
        public bool IsHorizontal { get; set; }

        /// <summary>
        /// 軸に平行な正の方向の値を取得または設定します。
        /// </summary>
        public int PrimalMajor
        {
            get { return GetMajor(IsHorizontal); }
            set { SetMajor(IsHorizontal, value); }
        }
        /// <summary>
        /// 軸に平行な負の方向の値を取得または設定します。
        /// </summary>
        public int PrimalMinor
        {
            get { return GetMinor(IsHorizontal); }
            set { SetMinor(IsHorizontal, value); }
        }
        /// <summary>
        /// 軸に垂直な正の方向の値を取得または設定します。
        /// </summary>
        public int SecondMajor
        {
            get { return GetMajor(!IsHorizontal); }
            set { SetMajor(!IsHorizontal, value); }
        }
        /// <summary>
        /// 軸に垂直な負の方向の値を取得または設定します。
        /// </summary>
        public int SecondMinor
        {
            get { return GetMinor(!IsHorizontal); }
            set { SetMinor(!IsHorizontal, value); }
        }
        /// <summary>
        /// 軸に平行な方向の長さを取得します。
        /// </summary>
        public int PrimalLength
        {
            get { return PrimalMajor - PrimalMinor; }
        }
        /// <summary>
        /// 軸に垂直な方向の長さを取得します。
        /// </summary>
        public int SecondLength
        {
            get { return SecondMajor - SecondMinor; }
        }

        public RectangleProxy(MapRectangle original, bool horizontal)
            :this()
        {
            Original = original;
            IsHorizontal = horizontal;
        }

        private int GetMajor(bool horizontal)
        {
            return horizontal ? Original.Right : Original.Top;
        }

        private int GetMinor(bool horizontal)
        {
            return horizontal ? Original.Left : Original.Bottom;
        }

        private void SetMajor(bool horizontal, int value)
        {
            if (horizontal)
            {
                Original.Right = value;
            }
            else
            {
                Original.Top = value;
            }
        }

        private void SetMinor(bool horizontal, int value)
        {
            if (horizontal)
            {
                Original.Left = value;
            }
            else
            {
                Original.Bottom = value;
            }
        }
    }
}