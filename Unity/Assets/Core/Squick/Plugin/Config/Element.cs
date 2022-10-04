using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
	class Element : IElement
	{
        public Element()
        {
            mxPropertyManager = new PropertyManager(new Guid());
        }

        public override IPropertyManager GetPropertyManager()
        {
            return mxPropertyManager;
        }


        public override Int64 QueryInt(string strName)
        {
            IProperty xProperty = GetPropertyManager().GetProperty(strName);
            if (null != xProperty)
            {
                return xProperty.QueryInt();
            }

            return 0;
        }

        public override double QueryFloat(string strName)
        {
            IProperty xProperty = GetPropertyManager().GetProperty(strName);
            if (null != xProperty)
            {
                return xProperty.QueryFloat();
            }

            return 0.0;
        }

        public override string QueryString(string strName)
        {
            IProperty xProperty = GetPropertyManager().GetProperty(strName);
            if (null != xProperty)
            {
                return xProperty.QueryString();
            }

            return DataList.NULL_STRING;
        }

        public override Guid QueryObject(string strName)
        {
            IProperty xProperty = GetPropertyManager().GetProperty(strName);
            if (null != xProperty)
            {
                return xProperty.QueryObject();
            }

            return DataList.NULL_OBJECT;
        }

        private IPropertyManager mxPropertyManager;
	}
}
