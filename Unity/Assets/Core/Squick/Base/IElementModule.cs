
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
	public abstract class IElementModule : IModule
    {
        public abstract bool Load();
        public abstract bool Clear();

        public abstract bool ExistElement(string strConfigName);
        public abstract bool AddElement(string strName, IElement xElement);
        public abstract IElement GetElement(string strConfigName);

        public abstract Int64 QueryPropertyInt(string strConfigName, string strPropertyName);
        public abstract double QueryPropertyFloat(string strConfigName, string strPropertyName);
        public abstract string QueryPropertyString(string strConfigName, string strPropertyName);
    }
}