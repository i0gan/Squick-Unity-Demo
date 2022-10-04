
using System.Collections;

namespace Squick
{
	public class PropertyManager : IPropertyManager
	{
		public PropertyManager(Guid self)
		{
			mSelf = self;
			mhtProperty = new Hashtable();
		}

        public override IProperty AddProperty(string strPropertyName, DataList.TData varData)
        {
            IProperty xProperty = null;
            if (!mhtProperty.ContainsKey(strPropertyName))
            {
                xProperty = new Property(mSelf, strPropertyName, varData);
                mhtProperty[strPropertyName] = xProperty;
            }

            return xProperty;
        }

		public override bool SetProperty(string strPropertyName, DataList.TData varData)
		{
			if (mhtProperty.ContainsKey(strPropertyName))
			{
				IProperty xProperty = (Property)mhtProperty[strPropertyName];
				if (null != xProperty)
				{
					xProperty.SetData(varData);
				} 
			}
			return true;
		}

		public override IProperty GetProperty(string strPropertyName)
		{
			IProperty xProperty = null;
			if (mhtProperty.ContainsKey(strPropertyName))
			{
				xProperty = (Property)mhtProperty[strPropertyName];
				return xProperty;
			}

			return xProperty;
		}

		public override void RegisterCallback(string strPropertyName, IProperty.PropertyEventHandler handler)
		{
			if (mhtProperty.ContainsKey(strPropertyName))
			{
				IProperty xProperty = (Property)mhtProperty[strPropertyName];
				xProperty.RegisterCallback(handler);
			}
		}
		
		public override DataList GetPropertyList()
		{
			DataList varData = new DataList();
			foreach( DictionaryEntry de in mhtProperty) 
			{
				varData.AddString(de.Key.ToString());				
			}
			
			return varData;
		}
		
		Guid mSelf;
		Hashtable mhtProperty;
	}
}