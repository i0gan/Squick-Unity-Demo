﻿//
// AddOnDemo.cs
//
// Author:
//       JasonXuDeveloper（傑） <jasonxudeveloper@gmail.com>
//
// Copyright (c) 2022 Uquick
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Uquick.Core;
using UnityEngine;

namespace Uquick.Examples
{
    public class AddOnDemo
    {
        public async void Awake()
        {
            var packageName = "AddOn1";
            var package = await Updater.CheckPackage(packageName);
            Debug.Log(StringifyHelper.JSONSerliaze(package));
            Updater.UpdatePackage("AddOn1", package: package, nextScene: BM.BPath.Assets_HotUpdateResources_AddOns_AddOn1_Scenes_test__unity, onLoadSceneFinished: () =>
            {
                Debug.Log("进入分包场景");
                Debug.Log(((TextAsset)AssetMgr.Load(BM.BPath.Assets_HotUpdateResources_AddOns_AddOn1_Others_test__txt, "AddOn1")).text);
            });
        }
    }
}
