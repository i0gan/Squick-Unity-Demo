
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Squick
{
	public abstract class IClassModule : IModule
    {
        public abstract void SetDataPath(string strDataPath);
        public abstract string GetDataPath();
        public abstract bool ExistElement(string strClassName);
        public abstract bool AddElement(string strClassName);

        public abstract ISClass GetElement(string strClassName);
        public abstract Dictionary<string, ISClass> GetElementList();

    }
}