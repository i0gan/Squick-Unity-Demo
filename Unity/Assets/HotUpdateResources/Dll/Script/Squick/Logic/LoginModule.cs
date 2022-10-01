using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SquickStruct;
using UnityEngine;
using Squick;
using Google.Protobuf;

namespace Squick
{
	public class LoginModule : IModule
    {    
		public enum Event : int
        {
			StartToConnect = 0,
			Connected,
            Disconnected,
            ConnectionRefused,

            RoleList = 10,
			LoginSuccess,
            LoginFailure,
            WorldList,
            ServerList,
            SelectServerSuccess,
            VerifyKeySuccess,
            EnterGameSuccess,
            SwapSceneSuccess,
        };


        public string mAccount;
        public string mKey;
        public int mServerID;
        public ArrayList mWorldServerList = new ArrayList();
        public ArrayList mGameServerList = new ArrayList();


        public static int autoReconnectGameID = 0;
        public Guid mRoleID = new Guid();
        public string mRoleName;
        public ArrayList mRoleList = new ArrayList();

		private MemoryStream mxBody = new MemoryStream();

        private NetModule mNetModule;
        private UIModule mUIModule;
        private ISEventModule mEventModule;
        private IKernelModule mKernelModule;
        private HelpModule mHelpModule;

        public LoginModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
		}

        public override void Awake()
		{
            mNetModule = mPluginManager.FindModule<NetModule>();
            mUIModule = mPluginManager.FindModule<UIModule>();
            mEventModule = mPluginManager.FindModule<ISEventModule>();
            mKernelModule = mPluginManager.FindModule<IKernelModule>();
            mHelpModule = mPluginManager.FindModule<HelpModule>();
        }

        public override void Init()
		{
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckLogin, OnLoginProcess);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckWorldList, OnWorldList);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckConnectWorld, OnConnectWorld);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckConnectKey, OnConnectKey);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckSelectServer, OnSelectServer);

            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckRoleList, EGMI_ACK_ROLE_LIST);


            mEventModule.RegisterCallback((int)LoginModule.Event.Connected, OnConnected);
			mEventModule.RegisterCallback((int)LoginModule.Event.Disconnected, OnDisconnected);

            mEventModule.RegisterCallback((int)LoginModule.Event.LoginSuccess, OnLoginSuccess);
            mEventModule.RegisterCallback((int)LoginModule.Event.WorldList, OnWorldList);
            mEventModule.RegisterCallback((int)LoginModule.Event.ServerList, OnServerList);
            mEventModule.RegisterCallback((int)LoginModule.Event.SelectServerSuccess, OnSelectServer);
            mEventModule.RegisterCallback((int)LoginModule.Event.RoleList, OnRoleList);
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

		public void OnConnected(int eventId, DataList valueList)
        {
			if (mKey != null && mKey.Length > 0)
			{
				//verify token
                RequireVerifyWorldKey(mAccount, mKey);
			}
        }

		public void OnDisconnected(int eventId, DataList valueList)
        {
            Debug.Log("断开连接");
			if (mKey != null)
            {
                //reconnect
                mAccount = "";
                mKey = "";
                mServerID = 0;
                mWorldServerList.Clear();
                mGameServerList.Clear();
                mRoleID = new Guid();
                mRoleName = "";
                mRoleList.Clear();

                //Clear all players and UI objects
                DataList xDataList = mKernelModule.GetObjectList();
                for (int i = 0; i < xDataList.Count(); ++i)
                {
                    mKernelModule.DestroyObject(xDataList.ObjectVal(i));
                }

                //mUIModule.DestroyAllUI();
                //mUIModule.ShowUI<UILogin>();
            }
        }
        
        // 请求消息
	    public void LoginPB(string strAccount, string strPwd, string strKey)
        {
            SquickStruct.ReqAccountLogin xData = new SquickStruct.ReqAccountLogin();
            xData.Account = ByteString.CopyFromUtf8(strAccount);
            xData.Password = ByteString.CopyFromUtf8(strPwd);
            xData.SecurityCode = ByteString.CopyFromUtf8(strKey);
            xData.SignBuff = ByteString.CopyFromUtf8("");
            xData.ClientVersion = 1;
            xData.LoginMode = SquickStruct.ELoginMode.ElmAutoRegisterLogin;
            xData.ClientIP = 0;
            xData.ClientMAC = 0;
            xData.DeviceInfo = ByteString.CopyFromUtf8("");
            xData.ExtraInfo = ByteString.CopyFromUtf8("");

            mAccount = strAccount;
            /*
            MemoryStream stream = new MemoryStream();
            xData.WriteTo(stream);
            mNetModule.SendMsg(SquickStruct.EGameMsgID.EGMI_REQ_LOGIN, stream);
*/
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            mNetModule.SendMsg((int)SquickStruct.EGameMsgID.ReqLogin, mxBody);
        }

	    public void RequireWorldList()
        {
            // 获取世界列表
            Debug.Log("正在获取世界列表");
            SquickStruct.ReqServerList xData = new SquickStruct.ReqServerList();
            xData.Type = SquickStruct.ReqServerListType.RsltWorldServer;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)SquickStruct.EGameMsgID.ReqWorldList, mxBody);
        }

	    public void RequireConnectWorld(int nWorldID)
        {
            SquickStruct.ReqConnectWorld xData = new SquickStruct.ReqConnectWorld();
            xData.WorldId = nWorldID;
            xData.LoginId = 0;
            xData.Account = ByteString.CopyFromUtf8("");
            xData.Sender = new Ident();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)SquickStruct.EGameMsgID.ReqConnectWorld, mxBody);
        }

	    public void RequireVerifyWorldKey(string strAccount, string strKey)
        {
            Debug.Log("获取key中: acc: " + strAccount + " pas: " + strKey);
            SquickStruct.ReqAccountLogin xData = new SquickStruct.ReqAccountLogin();
            xData.Account = ByteString.CopyFromUtf8(strAccount);
            xData.Password = ByteString.CopyFromUtf8("");
            xData.SecurityCode = ByteString.CopyFromUtf8(strKey);
            xData.SignBuff = ByteString.CopyFromUtf8("");
            xData.ClientVersion = 1;
            xData.LoginMode = 0;
            xData.ClientIP = 0;
            xData.ClientMAC = 0;
            xData.DeviceInfo = ByteString.CopyFromUtf8("");
            xData.ExtraInfo = ByteString.CopyFromUtf8("");

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            mAccount = strAccount;
            mKey = strKey;


            mNetModule.SendMsg((int)SquickStruct.EGameMsgID.ReqConnectKey, mxBody);
        }

	    public void RequireServerList()
        {
            Debug.Log("请求服务器列表中");
            SquickStruct.ReqServerList xData = new SquickStruct.ReqServerList();
            xData.Type = SquickStruct.ReqServerListType.RsltGamesErver;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)SquickStruct.EGameMsgID.ReqWorldList, mxBody);
        }

	    public void RequireSelectServer(int nServerID)
        {
            SquickStruct.ReqSelectServer xData = new SquickStruct.ReqSelectServer();
            xData.WorldId = nServerID;
            mServerID = nServerID;
                        
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)SquickStruct.EGameMsgID.ReqSelectServer, mxBody);
        }

        // 接收消息
		private void OnLoginProcess(int id, MemoryStream stream)
        {
			SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);
            SquickStruct.AckEventResult xData = SquickStruct.AckEventResult.Parser.ParseFrom(xMsg.MsgData);

            if (EGameEventCode.AccountLoginSuccess == xData.EventCode)
            {
				Debug.Log("Login  SUCCESS");
				mEventModule.DoEvent((int)LoginModule.Event.LoginSuccess);
            }
            else
            {
                Debug.Log("Login Faild,Code: " + xData.EventCode);
                DataList varList = new DataList();
                varList.AddInt((Int64)xData.EventCode);
				mEventModule.DoEvent((int)LoginModule.Event.LoginFailure);
            }
        }

        private void OnWorldList(int id, MemoryStream stream)
        {
            
	        SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);
            SquickStruct.AckServerList xData = SquickStruct.AckServerList.Parser.ParseFrom(xMsg.MsgData);

            if (ReqServerListType.RsltWorldServer == xData.Type)
            {
                for (int i = 0; i < xData.Info.Count; ++i)
                {
                    ServerInfo info = xData.Info[i];
                    Debug.Log("WorldList  ServerId: " + info.ServerId + " Name: " + info.Name.ToStringUtf8() + " Status: " + info.Status);
                    mWorldServerList.Add(info);
                }

				mEventModule.DoEvent((int)LoginModule.Event.WorldList);
            }
            else if (ReqServerListType.RsltGamesErver == xData.Type)
            {
                for (int i = 0; i < xData.Info.Count; ++i)
                {
                    ServerInfo info = xData.Info[i];
                    Debug.Log("GameList  Server Id: " + info.ServerId + " Name: " + info.Name.ToStringUtf8() + " Status: " + info.Status);
                    mGameServerList.Add(info);
                }
				mEventModule.DoEvent((int)LoginModule.Event.ServerList);
            }
        }

        private void OnConnectWorld(int id, MemoryStream stream)
        {
	        SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);
            SquickStruct.AckConnectWorldResult xData = SquickStruct.AckConnectWorldResult.Parser.ParseFrom(xMsg.MsgData);

            mKey = xData.WorldKey.ToStringUtf8();
            
			mNetModule.BeforeShut();
			mNetModule.Shut();

			String strIP = xData.WorldIp.ToStringUtf8();
			if (strIP == "127.0.0.1")
			{
				strIP = mNetModule.FirstIP();
			}
			mNetModule.StartConnect(strIP, xData.WorldPort);

        }



        private void OnConnectKey(int id, MemoryStream stream)
        {
	        SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream); // 解析获取Msg
            SquickStruct.AckEventResult xData = SquickStruct.AckEventResult.Parser.ParseFrom(xMsg.MsgData);

            if (xData.EventCode == EGameEventCode.VerifyKeySuccess)
            {
                Debug.Log("验证 Key 成功");
                RequireServerList();
            }
            else
            {
                Debug.Log("验证 Key 失败");
            }
        }

        private void OnSelectServer(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream); 

            SquickStruct.AckEventResult xData = SquickStruct.AckEventResult.Parser.ParseFrom(xMsg.MsgData);

            if (xData.EventCode == EGameEventCode.SelectserverSuccess)
            {
                Debug.Log("选择服务器成功 ");
				mEventModule.DoEvent((int)LoginModule.Event.SelectServerSuccess); // 调用选择服务器事件成功
            }
            else
            {
                Debug.Log("选择服务器失败 ");
            }
        }


        private void EGMI_ACK_ROLE_LIST(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.AckRoleLiteInfoList xData = SquickStruct.AckRoleLiteInfoList.Parser.ParseFrom(xMsg.MsgData);

			Debug.Log("QueryRoleList  SUCCESS : " + xData.CharData.Count);

			mRoleList.Clear();

            for (int i = 0; i < xData.CharData.Count; ++i)
            {
                SquickStruct.RoleLiteInfo info = xData.CharData[i];

                Debug.Log("QueryRoleList  SUCCESS : " + info.NoobName.ToStringUtf8());

				mRoleList.Add(info);
            }


			mEventModule.DoEvent((int)LoginModule.Event.RoleList);

            //////////////////
            /*
			if (mRoleList.Count > 0)
            {
                //NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_ROLEHALL);

				SquickStruct.RoleLiteInfo xLiteInfo = (SquickStruct.RoleLiteInfo)mRoleList[0];
                Guid xEnterID = new Guid();
                xEnterID.nData64 = xLiteInfo.id.index;
                xEnterID.nHead64 = xLiteInfo.id.svrid;

				mNetModule.RequireEnterGameServer(xEnterID, mAccount, xLiteInfo.noob_name.ToStringUtf8(), mServerID);

                //mxNetController.mPlayerState = NFNetController.PLAYER_STATE.E_PLAYER_WAITING_TO_GAME;

                Debug.Log("Selected role :" + xLiteInfo.noob_name.ToStringUtf8());
            }
            else
            {
                //NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_CREATEHALL);
				RequireCreateRole( mAccount, 0, 0, mServerID);
                Debug.Log("No Role!, require to create a new role! ");
            }
            */
        }

        //申请角色列表
        public void RequireRoleList()
        {
            SquickStruct.ReqRoleList xData = new SquickStruct.ReqRoleList();
			xData.GameId = mServerID;
			xData.Account = ByteString.CopyFromUtf8(mAccount);
            
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)SquickStruct.EGameMsgID.ReqRoleList, mxBody);
        }

        public void RequireCreateRole(string strRoleName, int byCareer, int bySex)
        {
            if (strRoleName.Length >= 20 || strRoleName.Length < 1)
            {
                return;
            }

            SquickStruct.ReqCreateRole xData = new SquickStruct.ReqCreateRole();
            xData.Career = byCareer;
            xData.Sex = bySex;
            xData.NoobName = ByteString.CopyFromUtf8(strRoleName);
			xData.Account = ByteString.CopyFromUtf8(mAccount);
            xData.Race = 0;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)SquickStruct.EGameMsgID.ReqCreateRole, mxBody);
        }


        public void RequireDelRole(string strRoleName)
        {
            SquickStruct.ReqDeleteRole xData = new SquickStruct.ReqDeleteRole();
            xData.Name = ByteString.CopyFromUtf8(strRoleName);
            xData.Account = ByteString.CopyFromUtf8(mAccount);
			xData.GameId = mServerID;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg((int)SquickStruct.EGameMsgID.ReqDeleteRole, mxBody);


            Debug.Log("RequireDelRole:" + strRoleName);
        }


        // Logic Event
        public void OnLoginSuccess(int eventId, DataList valueList)
        {
            //mUIModule.ShowUI<NFUISelectServer>();

            RequireWorldList();
        }

        public void OnWorldList(int eventId, DataList valueList)
        {
            Debug.Log("OnWorldList" + mWorldServerList.Count);

            foreach (SquickStruct.ServerInfo info in mWorldServerList)
            {

                RequireConnectWorld(info.ServerId);
                break;
            }
        }

        public void OnSelectServer(int eventId, DataList valueList)
        {
            RequireRoleList();
        }

        public void OnServerList(int eventId, DataList valueList)
        {
            ArrayList serverList = mGameServerList;

            Debug.Log("OnServerList:" + serverList.Count);

            if (autoReconnectGameID > 0)
            {
                RequireSelectServer(autoReconnectGameID);
            }
            else
            {
                System.Random rd = new System.Random();
                int index = rd.Next(0, serverList.Count);
                SquickStruct.ServerInfo info = (ServerInfo)serverList[index];

                RequireSelectServer(0);
            }

            autoReconnectGameID = 0;
        }

        // Logic Event
        public void OnRoleList(int eventId, DataList valueList)
        {
            ArrayList roleList = mRoleList;

            foreach (SquickStruct.RoleLiteInfo info in roleList)
            {
                OnRoleClick(0);
                return;
            }

            OnCreateRoleClick();
        }

        private void OnRoleClick(int nIndex)
        {
            ArrayList roleList = mRoleList;
            SquickStruct.RoleLiteInfo info = (SquickStruct.RoleLiteInfo)roleList[nIndex];

            mRoleID = mHelpModule.PBToNF(info.Id);
            mRoleName = info.NoobName.ToStringUtf8();

            mNetModule.RequireEnterGameServer();
        }

        private void OnCreateRoleClick()
        {

            string strRoleName = mAccount + "_Role";
            //string strRoleName = mLoginModule.mAccount + "_Role" + UnityEngine.Random.Range(1000, 10000);
            RequireCreateRole(strRoleName, 1, 1);
        }
    };
}