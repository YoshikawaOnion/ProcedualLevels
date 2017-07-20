using UnityEngine;
using System.Collections;
using System;

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
    }
}