using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ILRuntime.Runtime.Intepreter;
using Uquick.Interface;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Uquick.Helper
{
    public class RegisterFunctionDelegateHelper : IRegisterHelper
    {
        private static RegisterFunctionDelegateHelper Instance;

        public static void HelperRegister(AppDomain appdomain)
        {
            if (Instance == null)
            {
                Instance = new RegisterFunctionDelegateHelper();
            }
            Instance.Register(appdomain);
        }
        
        public void Register(AppDomain appdomain)
        {
            appdomain.DelegateManager.RegisterFunctionDelegate<String, Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Object, String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ParameterInfo, String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Task<ILTypeInstance>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Object, Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ParameterInfo, Type>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Type, String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Object, Type>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Object>();
            appdomain.DelegateManager.RegisterFunctionDelegate<List<ILTypeInstance>, Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<List<ILTypeInstance>, IEnumerable<ILTypeInstance>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<float>();
            appdomain.DelegateManager.RegisterFunctionDelegate<Task>();
            appdomain.DelegateManager.RegisterFunctionDelegate<CoroutineAdapter.Adaptor, Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<CoroutineAdapter.Adaptor, String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<MonoBehaviourAdapter.Adaptor, Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance, String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<KeyValuePair<String, ILTypeInstance>, ILTypeInstance>();
            appdomain.DelegateManager.RegisterFunctionDelegate<KeyValuePair<String, ILTypeInstance>, Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<KeyValuePair<String, ILTypeInstance>, String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Type, System.Type>();
            appdomain.DelegateManager.RegisterFunctionDelegate<global::MonoBehaviourAdapter.Adaptor, System.String>();


            appdomain.DelegateManager.RegisterMethodDelegate<BestHTTP.WebSocket.WebSocket>();
            appdomain.DelegateManager.RegisterMethodDelegate<BestHTTP.WebSocket.WebSocket, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<BestHTTP.WebSocket.WebSocket, System.Exception>();
            appdomain.DelegateManager.RegisterMethodDelegate<BestHTTP.WebSocket.WebSocket, System.UInt16, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.BaseEventData>();

            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.AsyncOperation>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.String>();






        }
    }
}