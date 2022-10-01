//-----------------------------------------------------------------------
// <copyright file="IElement.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/SquickProtocol>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squick
{
	public abstract class IElement
	{
        public abstract IPropertyManager GetPropertyManager();

        public abstract Int64 QueryInt(string strName);
        public abstract double QueryFloat(string strName);
        public abstract string QueryString(string strName);
        public abstract Guid QueryObject(string strName);

	}
}
