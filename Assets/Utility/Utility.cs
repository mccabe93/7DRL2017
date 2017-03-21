using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public struct Vector2i
    {
        public int x;
        public int y;
        public static Vector2i Zero = new Vector2i(0, 0);
        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2i(Vector2 uvector)
        {
            this.x = (int)uvector.x;
            this.y = (int)uvector.y;
        }
        public static Vector2i Max(Vector2i a, Vector2i b)
        {
            if (Magnitude(a) > Magnitude(b))
                return a;
            return b;
        }
        public static int Magnitude(Vector2i a)
        {
            return (a.x * a.x) + (a.y * a.y);
        }
        public static float Distance(Vector2i a, Vector2i b)
        {
            return Mathf.Sqrt((a.x * a.x - b.x * b.x) + (a.y * a.y - b.y * b.y));
        }
    }
    public struct Vector3i
    {
        public static Vector3i Default = new Vector3i(0, 0, -1);
        public int x;
        public int y;
        public int z;
        public Vector3 UnityVector;
        public Vector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            UnityVector = new Vector3(x, y, z);
        }

        public Vector3i(float x, float y, float z)
        {
            this.x = (int)x;
            this.y = (int)y;
            this.z = (int)z;
            UnityVector = new Vector3(x, y, z);
        }

        public bool equals(Vector3i v)
        {
            if (v.x == x && v.y == y && v.z == z)
                return true;
            return false;
        }

        public static Vector3i convert(Vector3 v)
        {
            return new Vector3i(v.x, v.y, v.z);
        }

        public static Vector3i lerp(Vector3i a, Vector3i b, float dt)
        {
            return Vector3i.convert(Vector3.Lerp(a.UnityVector, b.UnityVector, dt));
        }
    }
}