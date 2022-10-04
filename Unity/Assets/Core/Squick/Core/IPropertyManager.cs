
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
    public abstract class IPropertyManager
    {
        public abstract IProperty AddProperty(string strPropertyName, DataList.TData varData);

        public abstract bool SetProperty(string strPropertyName, DataList.TData varData);

        public abstract IProperty GetProperty(string strPropertyName);
		
		public abstract DataList GetPropertyList();
		
		public abstract void RegisterCallback(string strPropertyName, IProperty.PropertyEventHandler handler);
    }
}