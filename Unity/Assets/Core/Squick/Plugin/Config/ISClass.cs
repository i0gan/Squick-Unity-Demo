﻿//-----------------------------------------------------------------------
// <copyright file="NFILogicClass.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/SquickProtocol>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace Squick
{
    public abstract class ISClass
	{
        public abstract IPropertyManager GetPropertyManager();
        public abstract IRecordManager GetRecordManager();
        public abstract List<string> GetConfigNameList();
        public abstract List<string> GetIncludeFileList();
        public abstract bool AddConfigName(string strConfigName);
        public abstract bool AddIncludeFile(string fileName);

        public abstract string GetName();
        public abstract void SetName(string strConfigName);

        public abstract string GetPath();
        public abstract void SetPath(string strPath);

        public abstract string GetInstance();
        public abstract void SetInstance(string strInstancePath);

        public abstract void SetEncrypt(bool bEncrypt);
        public abstract bool GetEncrypt();
    }
}
