using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
    public class SVector3
    {
        static SVector3 zero = new SVector3();

        private float x;
        private float y;
        private float z;

        public SVector3()
        {
            this.x = 0.0f;
            this.y = 0.0f;
            this.z = 0.0f;
        }
        public SVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float X()
        {
            return this.x;
        }
        public float Y()
        {
            return this.y;

        }
        public float Z()
        {
            return this.z;
        }
        public void SetX(float x)
        {
            this.x = x;
        }
        public void SetY(float y)
        {
            this.y = y;
        }
        public void SetZ(float z)
        {
            this.z = z;
        }

        public bool IsZero()
        {
            return x < 0.001f && y < 0.001f && z < 0.001f;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public float SquaredLength()
        {
            return x * x + y * y + z * z;
        }

        public SVector3 Direction()
        {
            if (this.IsZero())
            {
                return new SVector3();
            }
            float length = this.Length();
            return new SVector3(x / length, y / length, z / length);
        }

        public bool FromString(string value)
        {
            string[] values = value.Split(',');
            if (values.Length != 3)
            {
                return false;
            }
            float.TryParse(values[0], out x);
            float.TryParse(values[1], out y);
            float.TryParse(values[2], out z);
            return true;
        }

        public override string ToString()
        {
            return x + "," + y + "," + z;
        }

        public static SVector3 add(SVector3 va, SVector3 vb)
        {
            SVector3 vector = new SVector3();
            vector.SetX(va.X() + vb.X());
            vector.SetY(va.Y() + vb.Y());
            vector.SetZ(va.Z() + vb.Z());
            return vector;
        }

        public static SVector3 sub(SVector3 va, SVector3 vb)
        {
            SVector3 vector = new SVector3();
            vector.SetX(va.X() - vb.X());
            vector.SetY(va.Y() - vb.Y());
            vector.SetZ(va.Z() - vb.Z());
            return vector;
        }

        public static float Distance(SVector3 va, SVector3 vb)
        {
            SVector3 vector = SVector3.sub(va, vb);
            return vector.Length();
        }

        public static SVector3 Zero()
        {
            return zero;
        }
    }
}
