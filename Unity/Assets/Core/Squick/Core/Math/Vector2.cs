using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
    public class SVector2
    {
        private static SVector2 zero = new SVector2();
        private float x;
        private float y;

        public SVector2()
        {
            this.x = 0.0f;
            this.y = 0.0f;
        }
        public SVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float X()
        {
            return this.x;
        }
        public float Y()
        {
            return this.y;

        }
        public void SetX(float x)
        {
            this.x = x;
        }
        public void SetY(float y)
        {
            this.y = y;
        }

        public bool IsZero()
        {
            return x < 0.001f && y < 0.001f;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public float SquaredLength()
        {
            return x * x + y * y;
        }

        public SVector2 Direction()
        {
            if (this.IsZero())
            {
                return new SVector2();
            }
            float length = this.Length();
            return new SVector2(x / length, y / length);
        }

        public bool FromString(string value)
        {
            string[] values = value.Split(',');
            if (values.Length != 2)
            {
                return false;
            }
            float.TryParse(values[0], out x);
            float.TryParse(values[1], out y);
            return true;
        }

        public override string ToString()
        {
            return x + "," + y;
        }

        public static SVector2 add(SVector2 va, SVector2 vb)
        {
            SVector2 vector = new SVector2();
            vector.SetX(va.X() + vb.X());
            vector.SetY(va.Y() + vb.Y());
            return vector;
        }

        public static SVector2 sub(SVector2 va, SVector2 vb)
        {
            SVector2 vector = new SVector2();
            vector.SetX(va.X() - vb.X());
            vector.SetY(va.Y() - vb.Y());
            return vector;
        }

        public static float Distance(SVector2 va, SVector2 vb)
        {
            SVector2 vector = SVector2.sub(va, vb);
            return vector.Length();
        }

        public static SVector2 Zero()
        {
            return zero;
        }
    }
}
