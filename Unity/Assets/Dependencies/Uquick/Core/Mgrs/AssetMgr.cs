using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BM;
using ET;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Uquick.Core
{
    public static class AssetMgr
    {
        public static bool RuntimeMode => AssetComponentConfig.AssetLoadMode != AssetLoadMode.Develop;

        public static Object Load(string path)
        {
            return Load(path, null, null);
        }

        public static Object Load(string path, string package)
        {
            return Load(path, package, null);
        }
        
        [Obsolete]
        public static Object Load(string path, Type type)
        {
            return Load(path, null, type);
        }

        private static Object Load(string path, string package, Type type)
        {
            
            var ret = AssetComponent.Load(out var handler, path, package);
            AddCache(handler);
            return ret;
        }

        public static T Load<T>(string path)
            where T : Object
        {
            return Load<T>(path, null, null);
        }

        public static T Load<T>(string path, string package)
            where T : Object
        {
            return Load<T>(path, package, null);
        }
        
        [Obsolete]
        public static T Load<T>(string path, Type type)
            where T : Object
        {
            return Load<T>(path, null, type);
        }

        private static T Load<T>(string path, string package, Type type)
            where T : Object
        {
            var ret = AssetComponent.Load<T>(out var handler, path, package);
            AddCache(handler);
            return ret;
        }

        public static async ETTask<Object> LoadAsync(string path)
        {
            return await LoadAsync(path, null, null);
        }

        public static async ETTask<Object> LoadAsync(string path, string package)
        {
            return await LoadAsync(path, package, null);
        }

        [Obsolete]
        public static async ETTask<Object> LoadAsync(string path, Type type)
        {
            return await LoadAsync(path, null, type);
        }

        private static async ETTask<Object> LoadAsync(string path, string package, Type type)
        {
            var ret = await AssetComponent.LoadAsync(out var handler, path, package);
            AddCache(handler);
            return ret;
        }

        public static async ETTask<T> LoadAsync<T>(string path)
            where T : Object
        {
            return await LoadAsync<T>(path, null, null);
        }

        public static async ETTask<T> LoadAsync<T>(string path, string package)
            where T : Object
        {
            return await LoadAsync<T>(path, package, null);
        }
        
        [Obsolete]
        public static async ETTask<T> LoadAsync<T>(string path, Type type)
            where T : Object
        {
            return await LoadAsync<T>(path, null, type);
        }

        private static async ETTask<T> LoadAsync<T>(string path, string package = null, Type type = null)
            where T : Object
        {
            var ret = await AssetComponent.LoadAsync<T>(out var handler, path, package);
            AddCache(handler);
            return ret;
        }

        /// <summary>
        /// 强制卸载所有该路径资源的引用
        /// </summary>
        /// <param name="path"></param>
        /// <param name="package"></param>
        public static void Unload(string path, string package = null)
        {
            package = string.IsNullOrEmpty(package) ? AssetComponentConfig.DefaultBundlePackageName : package;
            AssetComponent.UnLoadByPath(path, package);
        }

        public static void LoadScene(string path, bool additive = false, string package = null)
        {
            AssetComponent.LoadScene(path, package);
            if (additive)
                SceneManager.LoadScene(path, LoadSceneMode.Additive);
            else
                SceneManager.LoadScene(path);
            RemoveUnusedAssets();
        }

        public static async void LoadSceneAsync(string path, bool additive = false, string package = null,
            Action<float> loadingCallback = null,
            Action<AsyncOperation> finishedCallback = null)
        {
            await AssetComponent.LoadSceneAsync(path, package);
            AsyncOperation operation = additive
                ? SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive)
                : SceneManager.LoadSceneAsync(path);
            operation.allowSceneActivation = false;
            while (!operation.isDone && operation.progress < 0.9f)
            {
                loadingCallback?.Invoke(operation.progress);
                await Task.Delay(1);
            }

            loadingCallback?.Invoke(1);
            operation.allowSceneActivation = true;
            operation.completed += asyncOperation =>
            {
                RemoveUnusedAssets();
                finishedCallback?.Invoke(asyncOperation);
            };
        }

        private static List<LoadHandler> _cacheAssets = new List<LoadHandler>();

        private static void AddCache(LoadHandler handler)
        {
            _cacheAssets.Add(handler);
        }

        private static void RemoveCache(LoadHandler handler)
        {
            _cacheAssets.Remove(handler);
        }

        public static void RemoveUnusedAssets()
        {
            for (int i = 0, cnt = _cacheAssets.Count; i < cnt; i++)
            {
                _cacheAssets[i].UnLoad();
                _cacheAssets.RemoveAt(i);
                i--;
                cnt--;
            }
        }
    }
}