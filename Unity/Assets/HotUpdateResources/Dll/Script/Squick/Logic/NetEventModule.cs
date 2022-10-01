using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;
using SquickStruct;
using Google.Protobuf;
using Squick;

namespace Squick
{
	public class NetEventModule : IModule
	{
		private IKernelModule mKernelModule;
		private ISEventModule mEventModule;
        private HelpModule mHelpModule;
		private NetModule mNetModule;
		private LogModule mLogModule;

		public NetEventModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }

		public override void Awake()
        {
            mNetModule = mPluginManager.FindModule<NetModule>();
            mHelpModule = mPluginManager.FindModule<HelpModule>();
			mKernelModule = mPluginManager.FindModule<IKernelModule>();
			mLogModule = mPluginManager.FindModule<LogModule>();
			mEventModule = mPluginManager.FindModule<ISEventModule>();
        }

        public override void Init()
        {
			mNetModule.AddNetEventCallBack(NetEventDelegation);

			//mNetModule.RegisteredResultCodeDelegation(SquickStruct.EGameEventCode.EGEC_UNKOWN_ERROR, EGEC_UNKOWN_ERROR);
			//mNetModule.RegisteredResultCodeDelegation(SquickStruct.EGameEventCode.EGEC_ACCOUNT_SUCCESS, EGEC_ACCOUNT_SUCCESS);
        }

		public override void AfterInit()
        {
        }

        public override void Execute()
        {
        }
        public override void BeforeShut()
        {
        }

        public override void Shut()
        {
        }

		private void NetEventDelegation(NetEventType eventType)
		{
            Debug.Log(Time.realtimeSinceStartup.ToString() + " 服务器连接成功" + eventType.ToString());

			switch (eventType)
			{
				case NetEventType.Connected:
					mEventModule.DoEvent((int)LoginModule.Event.Connected);
					break;
				case NetEventType.Disconnected:
					mEventModule.DoEvent((int)LoginModule.Event.Disconnected);
                    break;
				case NetEventType.ConnectionRefused:
					mEventModule.DoEvent((int)LoginModule.Event.ConnectionRefused);
                    break;
				default:
					break;
			}
		}

	    private void EGEC_UNKOWN_ERROR(SquickStruct.EGameEventCode eCode)
	    {

	    }

		private void EGEC_ACCOUNT_SUCCESS(SquickStruct.EGameEventCode eCode)
		{
		}
	}
}