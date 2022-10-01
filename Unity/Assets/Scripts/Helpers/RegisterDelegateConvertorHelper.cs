using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using ILRuntime.Runtime.Intepreter;
using LitJson;
using UnityEngine.Events;
using WebSocketSharp;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using Object = System.Object;

namespace Uquick.Helper
{
    public class RegisterDelegateConvertorHelper
    {
        private static RegisterDelegateConvertorHelper Instance;

        public static void HelperRegister(AppDomain appdomain)
        {
            if (Instance == null)
            {
                Instance = new RegisterDelegateConvertorHelper();
            }
            Instance.Register(appdomain);
        }

        public void Register(AppDomain appdomain)
        {

            appdomain.DelegateManager.RegisterDelegateConvertor<Predicate<String>>(act =>
            {
                return new Predicate<String>(obj =>
                {
                    return ((Func<String, Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<ParameterizedThreadStart>(act =>
            {
                return new ParameterizedThreadStart(obj =>
                {
                    ((Action<Object>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<EventHandler<MessageEventArgs>>(act =>
            {
                return new EventHandler<MessageEventArgs>((sender, e) =>
                {
                    ((Action<Object, MessageEventArgs>)act)(sender, e);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction<String>>(act =>
            {
                return new UnityAction<String>(arg0 =>
                {
                    ((Action<String>) act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction<Boolean>>(act =>
            {
                return new UnityAction<Boolean>(arg0 =>
                {
                    ((Action<Boolean>) act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<WaitCallback>(act =>
            {
                return new WaitCallback(state => { ((Action<Object>) act)(state); });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction>(act =>
            {
                return new UnityAction(() => { ((Action) act)(); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction<Single>>(act =>
            {
                return new UnityAction<Single>(arg0 => { ((Action<Single>) act)(arg0); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnhandledExceptionEventHandler>(act =>
            {
                return new UnhandledExceptionEventHandler((sender, e) =>
                {
                    ((Action<Object, UnhandledExceptionEventArgs>) act)(sender, e);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Predicate<UnityEngine.Object>>(act =>
            {
                return new Predicate<UnityEngine.Object>(obj => { return ((Func<UnityEngine.Object, Boolean>) act)(obj); });
            });
            appdomain.DelegateManager
                .RegisterDelegateConvertor<Predicate<ILTypeInstance>>(act =>
                {
                    return new Predicate<ILTypeInstance>(obj =>
                    {
                        return ((Func<ILTypeInstance, Boolean>) act)(obj);
                    });
                });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction<Int32>>(act =>
            {
                return new UnityAction<Int32>(arg0 => { ((Action<Int32>) act)(arg0); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Action<JsonData>>(action =>
            {
                return new Action<JsonData>(a => { ((Action<JsonData>) action)(a); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityAction>(act =>
            {
                return new UnityAction(async () => { ((Action) act)(); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<ThreadStart>(act =>
            {
                return new ThreadStart(() => { ((Action) act)(); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Predicate<CoroutineAdapter.Adaptor>>(
                act =>
                {
                    return new Predicate<CoroutineAdapter.Adaptor>(obj =>
                    {
                        return ((Func<CoroutineAdapter.Adaptor, Boolean>) act)(obj);
                    });
                });
            appdomain.DelegateManager.RegisterDelegateConvertor<Predicate<MonoBehaviourAdapter.Adaptor>>(act =>
            {
                return new Predicate<MonoBehaviourAdapter.Adaptor>(obj =>
                {
                    return ((Func<MonoBehaviourAdapter.Adaptor, Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<ElapsedEventHandler>(act =>
            {
                return new ElapsedEventHandler((sender, e) =>
                {
                    ((Action<Object, ElapsedEventArgs>)act)(sender, e);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<Predicate<KeyValuePair<String, ILTypeInstance>>>(act =>
            {
                return new Predicate<KeyValuePair<String, ILTypeInstance>>(obj =>
                {
                    return ((Func<KeyValuePair<String, ILTypeInstance>, Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.EventHandler<System.Threading.Tasks.UnobservedTaskExceptionEventArgs>>((act) =>
            {
                return new System.EventHandler<System.Threading.Tasks.UnobservedTaskExceptionEventArgs>((sender, e) =>
                {
                    ((Action<System.Object, System.Threading.Tasks.UnobservedTaskExceptionEventArgs>)act)(sender, e);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<BestHTTP.WebSocket.OnWebSocketOpenDelegate>((act) =>
            {
                return new BestHTTP.WebSocket.OnWebSocketOpenDelegate((webSocket) =>
                {
                    ((Action<BestHTTP.WebSocket.WebSocket>)act)(webSocket);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<BestHTTP.WebSocket.OnWebSocketMessageDelegate>((act) =>
            {
                return new BestHTTP.WebSocket.OnWebSocketMessageDelegate((webSocket, message) =>
                {
                    ((Action<BestHTTP.WebSocket.WebSocket, System.String>)act)(webSocket, message);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<BestHTTP.WebSocket.OnWebSocketErrorDelegate>((act) =>
            {
                return new BestHTTP.WebSocket.OnWebSocketErrorDelegate((webSocket, ex) =>
                {
                    ((Action<BestHTTP.WebSocket.WebSocket, System.Exception>)act)(webSocket, ex);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<BestHTTP.WebSocket.OnWebSocketClosedDelegate>((act) =>
            {
                return new BestHTTP.WebSocket.OnWebSocketClosedDelegate((webSocket, code, message) =>
                {
                    ((Action<BestHTTP.WebSocket.WebSocket, System.UInt16, System.String>)act)(webSocket, code, message);
                });
            });
            

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>((arg0) =>
                {
                    ((Action<UnityEngine.EventSystems.BaseEventData>)act)(arg0);
                });
            });




        }
    }
}