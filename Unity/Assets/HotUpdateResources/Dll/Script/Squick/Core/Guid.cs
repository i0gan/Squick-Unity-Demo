//using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Squick
{
    public class GuidConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new Guid((string)value);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(string)
                && value == typeof(Guid))
            {
                Guid id = (Guid)value;
                return id.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof(GuidConverter))]
    public class Guid : System.Object
    {
        public System.Int64 nHead64;
        public System.Int64 nData64;

        public readonly static Guid Zero = new Guid();
       

        public Guid()
        {
            nHead64 = 0;
            nData64 = 0;
        }

        public Guid(Guid id)
        {
            nHead64 = id.nHead64;
            nData64 = id.nData64;
        }

        public Guid(System.Int64 nHead, System.Int64 nData)
        {
            nHead64 = nHead;
            nData64 = nData;
        }

        public Guid(string id)
        {
            this.Parse(id);
        }

        public static bool operator == (Guid ident, Guid other)
		{
            if (((object)ident == null) && ((object)other == null))
            {
                return true;
            }

            if (((object)ident == null) || ((object)other == null))
            {
                return false;
            }

            return ident.nHead64 == other.nHead64 && ident.nData64 == other.nData64;
		}

		public static bool operator != (Guid ident, Guid other)
		{
            return !(ident == other);
		}

        public override bool Equals(object other)
        {
            return this == (Guid)other;
        }

        public bool IsNull()
        {
            return 0 == nData64 && 0 == nHead64;
        }

        string strID = null;
        public override string ToString()
        {
            if (strID == null)
            {
                strID = nHead64.ToString() + "-" + nData64.ToString();
            }

            return strID;
        }

        public void Parse(string strData)
        {
            string[] strList = strData.Split('-');
            if (strList.Count() != 2)
            {
                return;
            }

            System.Int64 nHead = 0;
            if (!System.Int64.TryParse(strList[0], out nHead))
            {
                return;
            }

            System.Int64 nData = 0;
            if (!System.Int64.TryParse(strList[1], out nData))
            {
                return;
            }

            this.nHead64 = nHead;
            this.nData64 = nData;
        }

        public override int GetHashCode()
        {
            string str =this.ToString();
            return str.GetHashCode();
        }
    }
}