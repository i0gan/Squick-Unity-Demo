using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using UnityEngine;
using Squick;
using SquickStruct;

namespace Squick
{
	public partial class NetModule : IModule
    {
		private IKernelModule mKernelModule;
		private HelpModule mHelpModule;
		private LoginModule mLoginModule;

		private NetListener mNetListener;
		private NetClient mNetClient;

		private string strFirstIP;
		public string strGameServerIP;

        //sender
        private SquickStruct.MsgBase mxData = new SquickStruct.MsgBase();
		private MemoryStream mxBody = new MemoryStream();
		private MsgHead mxHead = new MsgHead();
        private byte[] sendBytes = new byte[ConstDefine.NF_PACKET_BUFF_SIZE];

        public NetModule(IPluginManager pluginManager)
        {
            mNetListener = new NetListener();
            mPluginManager = pluginManager;
        }
        
        public override void Awake()
        {
        }

		public override void Init()
		{
		}

        public override void Execute()
        {
			if (null != mNetClient)
			{
				mNetClient.Execute();
			}
        }

        public override void BeforeShut()
        {
			if (null != mNetClient)
            {
                mNetClient.Disconnect();
            }
        }

        public override void Shut()
        {
			mNetClient = null;
		}

		public override void AfterInit()
		{
			mHelpModule = mPluginManager.FindModule<HelpModule>();
			mKernelModule = mPluginManager.FindModule<IKernelModule>();
			mLoginModule = mPluginManager.FindModule<LoginModule>();
		}

		public String FirstIP()
		{
			return strFirstIP;
		}

        public void StartConnect(string strIP, int nPort)
        {
            Debug.Log(Time.realtimeSinceStartup.ToString() + " StartConnect " + strIP + " " + nPort.ToString());

			mNetClient = new NetClient(mNetListener);

            mNetClient.Connect(strIP, nPort);

            if (strFirstIP == null)
            {
                strFirstIP = strIP;
            }
            else if(strGameServerIP == null)
            {
                strGameServerIP = strIP;
            }
        }

        public void DisconnectFromServer()
        {
            mNetClient.Disconnect();
        }

        public NetState GetState()
        {
            return mNetClient.GetState();
        }

		public void AddReceiveCallBack(int eMsg, Squick.NetListener.MsgDelegation netHandler)
        {
			mNetListener.RegisteredDelegation(eMsg, netHandler);
        }
  
		public void AddNetEventCallBack(Squick.NetListener.EventDelegation netHandler)
        {
			mNetListener.RegisteredNetEventHandler(netHandler);
        }

        public void SendMsg(int unMsgID)
        {

            if (mNetClient != null)
            {
                //SquickStruct.MsgBase
                mxData.PlayerId = mHelpModule.NFToPB(mLoginModule.mRoleID);

                mxBody.SetLength(0);
                mxData.WriteTo(mxBody);
                // 网络字节序，待整改
                mxHead.unMsgID = (UInt16)unMsgID;
                mxHead.unDataLen = (UInt32)mxBody.Length + (UInt32)ConstDefine.NF_PACKET_HEAD_SIZE;

                byte[] bodyByte = mxBody.ToArray();
                byte[] headByte = mxHead.EnCode();

                Array.Clear(sendBytes, 0, ConstDefine.NF_PACKET_BUFF_SIZE);
                headByte.CopyTo(sendBytes, 0);
                bodyByte.CopyTo(sendBytes, headByte.Length);

                mNetClient.SendBytes(sendBytes, bodyByte.Length + headByte.Length);
            }
        }

        public void SendMsg(int unMsgID, MemoryStream stream)
        {
            //Debug.Log("send message:" + unMsgID);

            if (mNetClient != null)
            {
                //SquickStruct.MsgBase
                mxData.PlayerId = mHelpModule.NFToPB(mLoginModule.mRoleID);
                mxData.MsgData = ByteString.CopyFrom(stream.ToArray());

                mxBody.SetLength(0);
                mxData.WriteTo(mxBody);

                mxHead.unMsgID = (UInt16)unMsgID;
                mxHead.unDataLen = (UInt32)mxBody.Length + (UInt32)ConstDefine.NF_PACKET_HEAD_SIZE;

                byte[] bodyByte = mxBody.ToArray();
                byte[] headByte = mxHead.EnCode();

                Array.Clear(sendBytes, 0, ConstDefine.NF_PACKET_BUFF_SIZE);
                headByte.CopyTo(sendBytes, 0);
                bodyByte.CopyTo(sendBytes, headByte.Length);

                mNetClient.SendBytes(sendBytes, bodyByte.Length + headByte.Length);
            }

            /////////////////////////////////////////////////////////////////
        }
      
        ////////////////////////////////////修改自身属性
        public void RequirePropertyInt(Guid objectID, string strPropertyName, Int64 newVar)
        {
            SquickStruct.ObjectPropertyInt xData = new SquickStruct.ObjectPropertyInt();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            SquickStruct.PropertyInt xPropertyInt = new SquickStruct.PropertyInt();
            xPropertyInt.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xPropertyInt.Data = newVar;
            xData.PropertyList.Add(xPropertyInt);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload int");
            SendMsg((int)SquickStruct.EGameMsgID.AckPropertyInt, mxBody);
        }

        public void RequirePropertyFloat(Guid objectID, string strPropertyName, double newVar)
        {
            SquickStruct.ObjectPropertyFloat xData = new SquickStruct.ObjectPropertyFloat();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            SquickStruct.PropertyFloat xPropertyFloat = new SquickStruct.PropertyFloat();
            xPropertyFloat.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xPropertyFloat.Data = (float)newVar;
            xData.PropertyList.Add(xPropertyFloat);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload Float");
            SendMsg((int)SquickStruct.EGameMsgID.AckPropertyFloat, mxBody);
        }

        public void RequirePropertyString(Guid objectID, string strPropertyName, string newVar)
        {
            SquickStruct.ObjectPropertyString xData = new SquickStruct.ObjectPropertyString();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            SquickStruct.PropertyString xPropertyString = new SquickStruct.PropertyString();
            xPropertyString.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xPropertyString.Data = ByteString.CopyFromUtf8(newVar);
            xData.PropertyList.Add(xPropertyString);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload String");
            SendMsg((int)SquickStruct.EGameMsgID.AckPropertyString, mxBody);
        }

        public void RequirePropertyObject(Guid objectID, string strPropertyName, Guid newVar)
        {
            SquickStruct.ObjectPropertyObject xData = new SquickStruct.ObjectPropertyObject();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            SquickStruct.PropertyObject xPropertyObject = new SquickStruct.PropertyObject();
            xPropertyObject.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xPropertyObject.Data = mHelpModule.NFToPB(newVar);
            xData.PropertyList.Add(xPropertyObject);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload Object");
            SendMsg((int)SquickStruct.EGameMsgID.AckPropertyObject, mxBody);
        }

        public void RequirePropertyVector2(Guid objectID, string strPropertyName, SVector2 newVar)
        {
            SquickStruct.ObjectPropertyVector2 xData = new SquickStruct.ObjectPropertyVector2();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            SquickStruct.PropertyVector2 xProperty = new SquickStruct.PropertyVector2();
            xProperty.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xProperty.Data = mHelpModule.NFToPB(newVar);
            xData.PropertyList.Add(xProperty);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)SquickStruct.EGameMsgID.AckPropertyVector2, mxBody);
        }

        public void RequirePropertyVector3(Guid objectID, string strPropertyName, SVector3 newVar)
        {
            SquickStruct.ObjectPropertyVector3 xData = new SquickStruct.ObjectPropertyVector3();
            xData.PlayerId = mHelpModule.NFToPB(objectID);

            SquickStruct.PropertyVector3 xProperty = new SquickStruct.PropertyVector3();
            xProperty.PropertyName = ByteString.CopyFromUtf8(strPropertyName);
            xProperty.Data = mHelpModule.NFToPB(newVar);
            xData.PropertyList.Add(xProperty);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)SquickStruct.EGameMsgID.AckPropertyVector3, mxBody);
        }

		public void RequireAddRow(Guid objectID, string strRecordName, int nRow)
        {
            SquickStruct.ObjectRecordAddRow xData = new SquickStruct.ObjectRecordAddRow();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            SquickStruct.RecordAddRowStruct xRecordAddRowStruct = new SquickStruct.RecordAddRowStruct();
            xData.RowData.Add(xRecordAddRowStruct);
            xRecordAddRowStruct.Row = nRow;

			IObject xObject = mKernelModule.GetObject(objectID);
            IRecord xRecord = xObject.GetRecordManager().GetRecord(strRecordName);
            DataList xRowData = xRecord.QueryRow(nRow);
            for (int i = 0; i < xRowData.Count(); i++)
            {
                switch (xRowData.GetType(i))
                {
                    case DataList.VARIANT_TYPE.VTYPE_INT:
                        {
                            SquickStruct.RecordInt xRecordInt = new SquickStruct.RecordInt();
                            xRecordInt.Row = nRow;
                            xRecordInt.Col = i;
                            xRecordInt.Data = xRowData.IntVal(i);
                            xRecordAddRowStruct.RecordIntList.Add(xRecordInt);
                        }
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_FLOAT:
                        {
                            SquickStruct.RecordFloat xRecordFloat = new SquickStruct.RecordFloat();
                            xRecordFloat.Row = nRow;
                            xRecordFloat.Col = i;
                            xRecordFloat.Data = (float)xRowData.FloatVal(i);
                            xRecordAddRowStruct.RecordFloatList.Add(xRecordFloat);
                        }
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_STRING:
                        {
                            SquickStruct.RecordString xRecordString = new SquickStruct.RecordString();
                            xRecordString.Row = nRow;
                            xRecordString.Col = i;
                            xRecordString.Data = ByteString.CopyFromUtf8(xRowData.StringVal(i));
                            xRecordAddRowStruct.RecordStringList.Add(xRecordString);
                        }
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_OBJECT:
                        {
                            SquickStruct.RecordObject xRecordObject = new SquickStruct.RecordObject();
                            xRecordObject.Row = nRow;
                            xRecordObject.Col = i;
                            xRecordObject.Data = mHelpModule.NFToPB(xRowData.ObjectVal(i));
                            xRecordAddRowStruct.RecordObjectList.Add(xRecordObject);
                        }
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_VECTOR2:
                        {
                            SquickStruct.RecordVector2 xRecordVector = new SquickStruct.RecordVector2();
                            xRecordVector.Row = nRow;
                            xRecordVector.Col = i;
                            xRecordVector.Data = mHelpModule.NFToPB(xRowData.Vector2Val(i));
                            xRecordAddRowStruct.RecordVector2List.Add(xRecordVector);
                        }
                        break;
                    case DataList.VARIANT_TYPE.VTYPE_VECTOR3:
                        {
                            SquickStruct.RecordVector3 xRecordVector = new SquickStruct.RecordVector3();
                            xRecordVector.Row = nRow;
                            xRecordVector.Col = i;
                            xRecordVector.Data = mHelpModule.NFToPB(xRowData.Vector3Val(i));
                            xRecordAddRowStruct.RecordVector3List.Add(xRecordVector);
                        }
                        break;

                }
            }

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record addRow");
            SendMsg((int)SquickStruct.EGameMsgID.AckAddRow, mxBody);
        }

		public void RequireRemoveRow(Guid objectID, string strRecordName, int nRow)
        {
            SquickStruct.ObjectRecordRemove xData = new SquickStruct.ObjectRecordRemove();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);
            xData.RemoveRow.Add(nRow);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record removeRow");
            SendMsg((int)SquickStruct.EGameMsgID.AckRemoveRow, mxBody);
        }

		public void RequireSwapRow(Guid objectID, string strRecordName, int nOriginRow, int nTargetRow)
        {
            SquickStruct.ObjectRecordSwap xData = new SquickStruct.ObjectRecordSwap();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.OriginRecordName = ByteString.CopyFromUtf8(strRecordName);
            xData.TargetRecordName = ByteString.CopyFromUtf8(strRecordName);
            xData.RowOrigin = nOriginRow;
            xData.RowTarget = nTargetRow;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record swapRow");
            SendMsg((int)SquickStruct.EGameMsgID.AckSwapRow, mxBody);
        }

		public void RequireRecordInt(Guid objectID, string strRecordName, int nRow, int nCol, DataList.TData newVar)
        {
            SquickStruct.ObjectRecordInt xData = new SquickStruct.ObjectRecordInt();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            SquickStruct.RecordInt xRecordInt = new SquickStruct.RecordInt();
            xData.PropertyList.Add(xRecordInt);
            xRecordInt.Row = nRow;
            xRecordInt.Col = nCol;
            xRecordInt.Data = newVar.IntVal();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record int");
            SendMsg((int)SquickStruct.EGameMsgID.AckRecordInt, mxBody);
        }

		public void RequireRecordFloat(Guid objectID, string strRecordName, int nRow, int nCol, DataList.TData newVar)
        {
            SquickStruct.ObjectRecordFloat xData = new SquickStruct.ObjectRecordFloat();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            SquickStruct.RecordFloat xRecordFloat = new SquickStruct.RecordFloat();
            xData.PropertyList.Add(xRecordFloat);
            xRecordFloat.Row = nRow;
            xRecordFloat.Col = nCol;
            xRecordFloat.Data = (float)newVar.FloatVal();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record float");
            SendMsg((int)SquickStruct.EGameMsgID.AckRecordFloat, mxBody);
        }

		public void RequireRecordString(Guid objectID, string strRecordName, int nRow, int nCol, DataList.TData newVar)
        {
            SquickStruct.ObjectRecordString xData = new SquickStruct.ObjectRecordString();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            SquickStruct.RecordString xRecordString = new SquickStruct.RecordString();
            xData.PropertyList.Add(xRecordString);
            xRecordString.Row = nRow;
            xRecordString.Col = nCol;
            xRecordString.Data = ByteString.CopyFromUtf8(newVar.StringVal());

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record string");
            SendMsg((int)SquickStruct.EGameMsgID.AckRecordString, mxBody);
        }

		public void RequireRecordObject(Guid objectID, string strRecordName, int nRow, int nCol, DataList.TData newVar)
        {
            SquickStruct.ObjectRecordObject xData = new SquickStruct.ObjectRecordObject();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            SquickStruct.RecordObject xRecordObject = new SquickStruct.RecordObject();
            xData.PropertyList.Add(xRecordObject);
            xRecordObject.Row = nRow;
            xRecordObject.Col = nCol;
            xRecordObject.Data = mHelpModule.NFToPB(newVar.ObjectVal());

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            Debug.Log("send upload record object");
            SendMsg((int)SquickStruct.EGameMsgID.AckRecordObject, mxBody);
        }

		public void RequireRecordVector2(Guid objectID, string strRecordName, int nRow, int nCol, DataList.TData newVar)
        {
            SquickStruct.ObjectRecordVector2 xData = new SquickStruct.ObjectRecordVector2();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            SquickStruct.RecordVector2 xRecordVector = new SquickStruct.RecordVector2();
            xRecordVector.Row = nRow;
            xRecordVector.Col = nCol;
            xRecordVector.Data = mHelpModule.NFToPB(newVar.Vector2Val());

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)SquickStruct.EGameMsgID.AckRecordVector2, mxBody);
        }

		public void RequireRecordVector3(Guid objectID, string strRecordName, int nRow, int nCol, DataList.TData newVar)
        {
            SquickStruct.ObjectRecordVector3 xData = new SquickStruct.ObjectRecordVector3();
			xData.PlayerId = mHelpModule.NFToPB(objectID);
            xData.RecordName = ByteString.CopyFromUtf8(strRecordName);

            SquickStruct.RecordVector3 xRecordVector = new SquickStruct.RecordVector3();
            xRecordVector.Row = nRow;
            xRecordVector.Col = nCol;
            xRecordVector.Data = mHelpModule.NFToPB(newVar.Vector3Val());

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)SquickStruct.EGameMsgID.AckRecordVector3, mxBody);
        }

        ////////////////////////////////////修改自身属性end
        /////////////////////////////////////////////////////////////////

        public void RequireEnterGameServer()
        {
            SquickStruct.ReqEnterGameServer xData = new SquickStruct.ReqEnterGameServer();
			xData.Name = ByteString.CopyFromUtf8(mLoginModule.mRoleName);
			xData.Account = ByteString.CopyFromUtf8(mLoginModule.mAccount);
			xData.GameId = 0;
			xData.Id = mHelpModule.NFToPB(mLoginModule.mRoleID); 
			
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)SquickStruct.EGameMsgID.ReqEnterGame, mxBody);
        }

        public void RequireEnterGameFinish()
        {
            //only use in the first time when player enter game world
            SquickStruct.ReqAckEnterGameSuccess xData = new SquickStruct.ReqAckEnterGameSuccess();
            xData.Arg = 1;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)SquickStruct.EGameMsgID.ReqEnterGameFinish, mxBody);
        }

        //发送心跳
        public void RequireHeartBeat()
        {
            SquickStruct.ReqHeartBeat xData = new SquickStruct.ReqHeartBeat();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)SquickStruct.EGameMsgID.StsHeartBeat, mxBody);
        }

        //发送心跳
        public void RequireLagTest(int index)
        {
            SquickStruct.ReqAckLagTest xData = new SquickStruct.ReqAckLagTest();
            xData.Index = index;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)SquickStruct.EGameMsgID.ReqLagTest, mxBody);
        }

        //WSAD移动
        public void RequireMove(Guid objectID, int nType, UnityEngine.Vector3 vPos)
        {
            SquickStruct.ReqAckPlayerPosSync xData = new SquickStruct.ReqAckPlayerPosSync();

            SquickStruct.TransformSyncUnit posSyncUnit = new TransformSyncUnit();
            posSyncUnit.Mover = mHelpModule.NFToPB(objectID);
            posSyncUnit.Position = new SquickStruct.Vector3();
            posSyncUnit.Position.X = vPos.x;
            posSyncUnit.Position.Y = vPos.y;
            posSyncUnit.Position.Z = vPos.z;
            posSyncUnit.Status = nType;
            xData.SyncUnit.Add(posSyncUnit);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg((int)SquickStruct.EGameMsgID.ReqMove, mxBody);

            //为了表现，客户端先走，后续同步
        }

        public void RequireMoveImmune(Guid objectID, UnityEngine.Vector3 vPos)
        {
            SquickStruct.ReqAckPlayerPosSync xData = new SquickStruct.ReqAckPlayerPosSync();

            SquickStruct.TransformSyncUnit posSyncUnit = new TransformSyncUnit();
            posSyncUnit.Mover = mHelpModule.NFToPB(objectID);
            posSyncUnit.Position = new SquickStruct.Vector3();
            posSyncUnit.Position.X = vPos.x;
            posSyncUnit.Position.Y = vPos.y;
            posSyncUnit.Position.Z = vPos.z;
            posSyncUnit.Type = (int)SquickStruct.TransformSyncUnit.Types.EMoveType.EetTeleport;
            xData.SyncUnit.Add(posSyncUnit);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)SquickStruct.EGameMsgID.ReqMove, mxBody);
        }

        //有可能是他副本的NPC移动,因此增加64对象ID
        public void RequireUseSkill(Guid objectID, string strKillID, Int32 index, List<Guid> nTargetIDList)
        {
            SquickStruct.ReqAckUseSkill xData = new SquickStruct.ReqAckUseSkill();
            xData.User = mHelpModule.NFToPB(objectID);
            xData.SkillId = ByteString.CopyFromUtf8(strKillID);
            xData.ClientIndex = index;

            if (nTargetIDList != null)
            {
                foreach (Guid id in nTargetIDList)
                {
                    SquickStruct.EffectData xEffData = new SquickStruct.EffectData();
                    xEffData.EffectIdent = (mHelpModule.NFToPB(id));
                    xEffData.EffectValue = 0;
                    xEffData.EffectRlt = 0;

                    xData.EffectData.Add(xEffData);
                }
            }

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);
            SendMsg((int)SquickStruct.EGameMsgID.ReqSkillObjectx, mxBody);
        }

        public void RequireSwapScene(int nTransferType, int nSceneID, int nLineIndex)
        {
            SquickStruct.ReqAckSwapScene xData = new SquickStruct.ReqAckSwapScene();
            xData.TransferType = nTransferType;
            xData.SceneId = nSceneID;
            xData.LineId = nLineIndex;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)SquickStruct.EGameMsgID.ReqSwapScene, mxBody);
        }

        public void RequireSwapScene(int nSceneID)
        {
            SquickStruct.ReqAckSwapScene xData = new SquickStruct.ReqAckSwapScene();
            xData.TransferType = 0;
            xData.SceneId = nSceneID;
            xData.LineId = -1;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)SquickStruct.EGameMsgID.ReqSwapScene, mxBody);
        }

        public void RequireSwapScene(int nTransferType, int nSceneID)
        {
            SquickStruct.ReqAckSwapScene xData = new SquickStruct.ReqAckSwapScene();
            xData.TransferType = nTransferType;
            xData.SceneId = nSceneID;
            xData.LineId = -1;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg((int)SquickStruct.EGameMsgID.ReqSwapScene, mxBody);
        }

        /////////////////////////////////////////////////////////////////
    }
}