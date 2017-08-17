using System;
using UnityEngine;

namespace ProcedualLevels.Common
{
    public static class Vector3Extensions
    {
        public static Vector3 CreateFromArray(float[] array)
        {
            if (array.Length == 3)
            {
                return new Vector3(array[0], array[1], array[2]);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static float[] ToArray(this Vector3 vector)
        {
            return new[] { vector.x, vector.y, vector.z };
        }

        static public Vector3 MergeX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        static public Vector3 MergeY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        static public Vector3 MergeZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        static public Vector3 MergeX(this Vector3 v, Vector3 v2)
        {
            return new Vector3(v2.x, v.y, v.z);
        }

        static public Vector3 MergeY(this Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x, v2.y, v.z);
        }

        static public Vector3 MergeZ(this Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x, v.y, v2.z);
        }

        static public Vector3 AddX(this Vector3 v, float x)
        {
            return new Vector3(v.x + x, v.y, v.z);
        }

        static public Vector3 AddY(this Vector3 v, float y)
        {
            return new Vector3(v.x, v.y + y, v.z);
        }

        static public Vector3 AddZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, v.z + z);
        }

        static public Vector3 AddX(this Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x + v2.x, v.y, v.z);
        }

        static public Vector3 AddY(this Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x, v.y + v2.y, v.z);
        }

        static public Vector3 AddZ(this Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x, v.y, v.z + v2.z);
        }

        static public Vector3 Add(this Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x + v2.x, v.y + v2.y, v.z + v2.z);
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        /// <summary>
        /// ベクトルの各要素を掛け合わせます。
        /// </summary>
        /// <returns>The mul.</returns>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        public static Vector3 Mul(this Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        public static Vector3 Div(this Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }

        public static bool IsNaN(this Vector3 v)
        {
            if (float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z))
            {
                return true;
            }
            return false;
        }
    }

    public static class Vector2Extensions
    {

        public static Vector2 InvertX(this Vector2 v)
        {
            return new Vector2(-v.x, v.y);
        }

        public static Vector2 InvertY(this Vector2 v)
        {
            return new Vector2(v.x, -v.y);
        }

        public static Vector2 MergeX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        public static Vector2 MergeY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        public static Vector3 ToVector3(this Vector2 v, float z = 0)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector2 FromAngleLength(float angle, float length)
        {
            var x = Mathf.Sin(angle * Mathf.Deg2Rad);
            var y = Mathf.Cos(angle * Mathf.Deg2Rad);
            return new Vector2(x, y) * length;
        }

        public static bool IsNaN(this Vector2 v)
        {
            if (float.IsNaN(v.x) || float.IsNaN(v.y))
            {
                return true;
            }
            return false;
        }
    }
}