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
//using SquickProtocol;
using Google.Protobuf;
using Squick;

namespace Squick
{
    public partial class NetHandlerModule : IModule
    {
        public enum Event : int
        {
            SwapScene = 100,
            PlayerMove,
        };


        class ObjectDataBuff
        {
            public SquickStruct.ObjectRecordList xRecordList;
            public SquickStruct.ObjectPropertyList xPropertyList;
        };


        private IKernelModule mKernelModule;
        private IClassModule mClassModule;
        private IElementModule mElementModule;
        private ISEventModule mEventModule;
        
        private LanguageModule mLanguageModule;
        private HelpModule mHelpModule;
        private NetModule mNetModule;
        private NetEventModule mNetEventModule;
        private NFSceneModule mSceneModule;
        private LoginModule mLoginModule;
        private NFUIModule mUIModule;

        private NetHandlerModule mxNetListener = null;

        private Dictionary<Guid, ObjectDataBuff> mxObjectDataBuff = new Dictionary<Guid, ObjectDataBuff>();

        public delegate void ResultCodeDelegation(SquickStruct.EGameEventCode eventCode, String data);
        private Dictionary<SquickStruct.EGameEventCode, ResultCodeDelegation> mhtEventDelegation = new Dictionary<SquickStruct.EGameEventCode, ResultCodeDelegation>();

        public NetHandlerModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }

        public void AddMsgEventCallBack(SquickStruct.EGameEventCode code, NetHandlerModule.ResultCodeDelegation netHandler)
        {
            if (!mhtEventDelegation.ContainsKey(code))
            {
                ResultCodeDelegation myDelegationHandler = new ResultCodeDelegation(netHandler);
                mhtEventDelegation.Add(code, myDelegationHandler);
            }
            else
            {
                ResultCodeDelegation myDelegationHandler = (ResultCodeDelegation)mhtEventDelegation[code];
                myDelegationHandler += new ResultCodeDelegation(netHandler);
            }
        }

        public void DoResultCodeDelegation(SquickStruct.EGameEventCode code, String data)
        {
            if (mhtEventDelegation.ContainsKey(code))
            {
                ResultCodeDelegation myDelegationHandler = (ResultCodeDelegation)mhtEventDelegation[code];
                myDelegationHandler(code, "");
            }
        }

        // Use this for initialization
        public override void Awake()
        {
            mClassModule = mPluginManager.FindModule<IClassModule>();
            mKernelModule = mPluginManager.FindModule<IKernelModule>();
            mElementModule = mPluginManager.FindModule<IElementModule>();
            mEventModule = mPluginManager.FindModule<ISEventModule>();
            
            mLanguageModule = mPluginManager.FindModule<LanguageModule>();
            mSceneModule = mPluginManager.FindModule<NFSceneModule>();
            mNetModule = mPluginManager.FindModule<NetModule>();
            mHelpModule = mPluginManager.FindModule<HelpModule>();
            mNetEventModule = mPluginManager.FindModule<NetEventModule>();
            mUIModule = mPluginManager.FindModule<NFUIModule>();
            mLoginModule = mPluginManager.FindModule<LoginModule>();

        }

        public override void Init()
        {

            mKernelModule.RegisterClassCallBack(SquickProtocol.Player.ThisName, ClassEventHandler);
            mKernelModule.RegisterClassCallBack(SquickProtocol.NPC.ThisName, ClassEventHandler);

            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.EventResult, EGMI_EVENT_RESULT);

            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckEnterGame, EGMI_ACK_ENTER_GAME);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckSwapScene, EGMI_ACK_SWAP_SCENE);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckEnterGameFinish, EGMI_ACK_ENTER_GAME_FINISH);


            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckObjectEntry, EGMI_ACK_OBJECT_ENTRY);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckObjectLeave, EGMI_ACK_OBJECT_LEAVE);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckMove, EGMI_ACK_MOVE);

            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckPropertyInt, EGMI_ACK_PROPERTY_INT);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckPropertyFloat, EGMI_ACK_PROPERTY_FLOAT);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckPropertyString, EGMI_ACK_PROPERTY_STRING);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckPropertyObject, EGMI_ACK_PROPERTY_OBJECT);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckPropertyVector2, EGMI_ACK_PROPERTY_VECTOR2);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckPropertyVector3, EGMI_ACK_PROPERTY_VECTOR3);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckPropertyClear, EGMI_ACK_PROPERTY_CLEAR);

            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckRecordInt, EGMI_ACK_RECORD_INT);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckRecordFloat, EGMI_ACK_RECORD_FLOAT);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckRecordString, EGMI_ACK_RECORD_STRING);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckRecordObject, EGMI_ACK_RECORD_OBJECT);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckSwapRow, EGMI_ACK_SWAP_ROW);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckAddRow, EGMI_ACK_ADD_ROW);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckRemoveRow, EGMI_ACK_REMOVE_ROW);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckRecordClear, EGMI_ACK_RECORD_CLEAR);

            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckObjectRecordEntry, EGMI_ACK_OBJECT_RECORD_ENTRY);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckObjectPropertyEntry, EGMI_ACK_OBJECT_PROPERTY_ENTRY);
            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckDataFinished, EGMI_ACK_DATA_FINISHED);


            mNetModule.AddReceiveCallBack((int)SquickStruct.EGameMsgID.AckSkillObjectx, EGMI_ACK_SKILL_OBJECTX);


            ////////////////////////////////////////////////////////////////////////


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

        private void EGMI_EVENT_RESULT(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            //OnResultMsg
            SquickStruct.AckEventResult xResultCode = SquickStruct.AckEventResult.Parser.ParseFrom(xMsg.MsgData);
            SquickStruct.EGameEventCode eEvent = xResultCode.EventCode;

            DoResultCodeDelegation(eEvent, "");
        }



        private void EGMI_ACK_ENTER_GAME(int id, MemoryStream stream)
        {

            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.AckEventResult xData = SquickStruct.AckEventResult.Parser.ParseFrom(xMsg.MsgData);

            Debug.Log("EGMI_ACK_ENTER_GAME " + xData.EventCode.ToString());

            mEventModule.DoEvent((int)LoginModule.Event.EnterGameSuccess);
            //mSceneModule.LoadScene((int)xData.event_code);
            //可以播放过图动画场景
        }

        private void EGMI_ACK_SWAP_SCENE(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ReqAckSwapScene xData = SquickStruct.ReqAckSwapScene.Parser.ParseFrom(xMsg.MsgData);

            Debug.Log("SWAP_SCENE: " + xData.SceneId + " " + xData.X + "," + xData.Y + "," + xData.Z);

            mEventModule.DoEvent((int)LoginModule.Event.SwapSceneSuccess);
            /*
            SquickStruct.AckMiningTitle xTileData = null;
            if (null != xData.Data && xData.Data.Length > 0)
            {
                xTileData = SquickStruct.AckMiningTitle.Parser.ParseFrom(xData.Data);
            }
            */
            mSceneModule.LoadScene(xData.SceneId, xData.X, xData.Y, xData.Z, "");

            //重置主角坐标到出生点
        }

        private void EGMI_ACK_ENTER_GAME_FINISH(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ReqAckEnterGameSuccess xData = SquickStruct.ReqAckEnterGameSuccess.Parser.ParseFrom(xMsg.MsgData);

            Debug.Log("Enter game finished: " + xData.ToString());

            // 去掉遮场景的ui
            //主角，等怪物enable，并且充值在相应的position
            //mSceneModule.LoadScene(xData.scene_id, xData.x, xData.y, xData.z);
        }


        private void EGMI_ACK_OBJECT_ENTRY(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.AckPlayerEntryList xData = SquickStruct.AckPlayerEntryList.Parser.ParseFrom(xMsg.MsgData);

            for (int i = 0; i < xData.ObjectList.Count; ++i)
            {
                SquickStruct.PlayerEntryInfo xInfo = xData.ObjectList[i];

                SVector3 vPos = new SVector3(xInfo.X, xInfo.Y, xInfo.Z);

                DataList var = new DataList();
                var.AddString(SquickProtocol.NPC.Position);
                var.AddVector3(vPos);

                Guid xObjectID = mHelpModule.PBToNF(xInfo.ObjectGuid);
                string strClassName = xInfo.ClassId.ToStringUtf8();
                string strConfigID = xInfo.ConfigId.ToStringUtf8();

                Debug.Log("new Object enter: " + strClassName + " " + xObjectID.ToString() + " " + strConfigID  + " (" + xInfo.X + "," + xInfo.Y + "," + xInfo.Z + ")");

                ObjectDataBuff xDataBuff = new ObjectDataBuff();
                mxObjectDataBuff.Add(xObjectID, xDataBuff);
                /*
                IObject xGO = mKernelModule.CreateObject(xObjectID, xInfo.scene_id, 0, strClassName, strConfigID, var);
                if (null == xGO)
                {
                    Debug.LogError("ID conflict: " + xObjectID.ToString() + "  ConfigID: " + strClassName);
                    continue;
                }
                */
            }
        }

        private void EGMI_ACK_OBJECT_LEAVE(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.AckPlayerLeaveList xData = SquickStruct.AckPlayerLeaveList.Parser.ParseFrom(xMsg.MsgData);

            for (int i = 0; i < xData.ObjectList.Count; ++i)
            {
                mKernelModule.DestroyObject(mHelpModule.PBToNF(xData.ObjectList[i]));
            }
        }

        private void EGMI_ACK_OBJECT_RECORD_ENTRY(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.MultiObjectRecordList xData = SquickStruct.MultiObjectRecordList.Parser.ParseFrom(xMsg.MsgData);

            for (int i = 0; i < xData.MultiPlayerRecord.Count; i++)
            {
                SquickStruct.ObjectRecordList xObjectRecordList = xData.MultiPlayerRecord[i];
                Guid xObjectID = mHelpModule.PBToNF(xObjectRecordList.PlayerId);

                Debug.Log ("new record enter Object: " + xObjectID.ToString () );

                ObjectDataBuff xDataBuff;
                if (mxObjectDataBuff.TryGetValue(xObjectID, out xDataBuff))
                {
                    xDataBuff.xRecordList = xObjectRecordList;
                    if (xObjectID.IsNull())
                    {
                        AttachObjectData(xObjectID);
                    }
                }
            }
        }

        private void EGMI_ACK_OBJECT_PROPERTY_ENTRY(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.MultiObjectPropertyList xData = SquickStruct.MultiObjectPropertyList.Parser.ParseFrom(xMsg.MsgData);

            for (int i = 0; i < xData.MultiPlayerProperty.Count; i++)
            {
                SquickStruct.ObjectPropertyList xPropertyData = xData.MultiPlayerProperty[i];
                Guid xObjectID = mHelpModule.PBToNF(xPropertyData.PlayerId);

                Debug.Log("new property enter Object: " + xObjectID.ToString());

                ObjectDataBuff xDataBuff;
                if (mxObjectDataBuff.TryGetValue(xObjectID, out xDataBuff))
                {

                    xDataBuff.xPropertyList = xPropertyData;
                    if (xObjectID.IsNull())
                    {
                        AttachObjectData(xObjectID);
                    }
                }
                else
                {
                    xDataBuff = new ObjectDataBuff();
                    xDataBuff.xPropertyList = xPropertyData;
                    mxObjectDataBuff[xObjectID] = xDataBuff;
                    AttachObjectData(xObjectID);
                }
            }
        }

        private void AttachObjectData(Guid self)
        {
            Debug.Log ("AttachObjectData : " + self.ToString () );

            ObjectDataBuff xDataBuff;
            if (mxObjectDataBuff.TryGetValue(self, out xDataBuff))
            {
                ////////////////record
                if (xDataBuff.xRecordList != null)
                {
                    for (int j = 0; j < xDataBuff.xRecordList.RecordList.Count; j++)
                    {
                        SquickStruct.ObjectRecordBase xObjectRecordBase = xDataBuff.xRecordList.RecordList[j];
                        string srRecordName = xObjectRecordBase.RecordName.ToStringUtf8();

                        for (int k = 0; k < xObjectRecordBase.RowStruct.Count; ++k)
                        {
                            SquickStruct.RecordAddRowStruct xAddRowStruct = xObjectRecordBase.RowStruct[k];

                            ADD_ROW(self, xObjectRecordBase.RecordName.ToStringUtf8(), xAddRowStruct);
                        }
                    }

                    xDataBuff.xRecordList = null;
                }
                ////////////////property
                if (xDataBuff.xPropertyList != null)
                {
                    /// 
                    IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xDataBuff.xPropertyList.PlayerId));
                    IPropertyManager xPropertyManager = go.GetPropertyManager();

                    for (int j = 0; j < xDataBuff.xPropertyList.PropertyIntList.Count; j++)
                    {
                        string strPropertyName = xDataBuff.xPropertyList.PropertyIntList[j].PropertyName.ToStringUtf8();
                        IProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                        if (null == xProperty)
                        {
                            DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_INT);
                            xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                        }

                        //string className = mKernelModule.QueryPropertyString(self, SquickProtocol.IObject.ClassName);
                        //Debug.LogError (self.ToString() + " " + className + " " + strPropertyName + " : " + xDataBuff.xPropertyList.property_int_list[j].Data);

                        xProperty.SetInt(xDataBuff.xPropertyList.PropertyIntList[j].Data);
                    }

                    for (int j = 0; j < xDataBuff.xPropertyList.PropertyFloatList.Count; j++)
                    {
                        string strPropertyName = xDataBuff.xPropertyList.PropertyFloatList[j].PropertyName.ToStringUtf8();
                        IProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                        if (null == xProperty)
                        {

                            DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_FLOAT);
                            xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                        }

                        xProperty.SetFloat(xDataBuff.xPropertyList.PropertyFloatList[j].Data);
                    }

                    for (int j = 0; j < xDataBuff.xPropertyList.PropertyStringList.Count; j++)
                    {
                        string strPropertyName = xDataBuff.xPropertyList.PropertyStringList[j].PropertyName.ToStringUtf8();
                        IProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                        if (null == xProperty)
                        {
                            DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                            xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                        }

                        //string className = mKernelModule.QueryPropertyString(self, SquickProtocol.IObject.ClassName);
                        //Debug.LogError(self.ToString() + " " + className + " " + strPropertyName + " : " + xDataBuff.xPropertyList.property_string_list[j].Data.ToStringUtf8());

                        xProperty.SetString(xDataBuff.xPropertyList.PropertyStringList[j].Data.ToStringUtf8());
                    }

                    for (int j = 0; j < xDataBuff.xPropertyList.PropertyObjectList.Count; j++)
                    {
                        string strPropertyName = xDataBuff.xPropertyList.PropertyObjectList[j].PropertyName.ToStringUtf8();
                        IProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                        if (null == xProperty)
                        {
                            DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_OBJECT);
                            xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                        }

                        xProperty.SetObject(mHelpModule.PBToNF(xDataBuff.xPropertyList.PropertyObjectList[j].Data));
                    }

                    for (int j = 0; j < xDataBuff.xPropertyList.PropertyVector2List.Count; j++)
                    {
                        string strPropertyName = xDataBuff.xPropertyList.PropertyVector2List[j].PropertyName.ToStringUtf8();
                        IProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                        if (null == xProperty)
                        {
                            DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR2);

                            xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                        }

                        xProperty.SetVector2(mHelpModule.PBToNF(xDataBuff.xPropertyList.PropertyVector2List[j].Data));
                    }

                    for (int j = 0; j < xDataBuff.xPropertyList.PropertyVector3List.Count; j++)
                    {
                        string strPropertyName = xDataBuff.xPropertyList.PropertyVector3List[j].PropertyName.ToStringUtf8();
                        IProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                        if (null == xProperty)
                        {
                            DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR3);

                            xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                        }

                        xProperty.SetVector3(mHelpModule.PBToNF(xDataBuff.xPropertyList.PropertyVector3List[j].Data));
                    }


                    xDataBuff.xPropertyList = null;
                }
            }
        }

        private void ClassEventHandler(Guid self, int nContainerID, int nGroupID, Squick.IObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            switch (eType)
            {
                case Squick.IObject.CLASS_EVENT_TYPE.OBJECT_CREATE:
                    break;
                case Squick.IObject.CLASS_EVENT_TYPE.OBJECT_LOADDATA:
                    AttachObjectData(self);
                    break;
                case Squick.IObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH:
                    mxObjectDataBuff.Remove(self);
                    break;
                case Squick.IObject.CLASS_EVENT_TYPE.OBJECT_DESTROY:
                    break;
                default:
                    break;
            }
        }

        private void EGMI_ACK_DATA_FINISHED(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.AckPlayerEntryList xData = SquickStruct.AckPlayerEntryList.Parser.ParseFrom(xMsg.MsgData);

            for (int i = 0; i < xData.ObjectList.Count; ++i)
            {
                SquickStruct.PlayerEntryInfo xInfo = xData.ObjectList[i];

                SVector3 vPos = new SVector3(xInfo.X, xInfo.Y, xInfo.Z);

                DataList var = new DataList();
                var.AddString("Position");
                var.AddVector3(vPos);

                Guid xObjectID = mHelpModule.PBToNF(xInfo.ObjectGuid);
                string strClassName = xInfo.ClassId.ToStringUtf8();
                string strConfigID = xInfo.ConfigId.ToStringUtf8();

                Debug.Log("create Object: " + strClassName + " " + xObjectID.ToString() + " " + strConfigID + " (" + xInfo.X + "," + xInfo.Y + "," + xInfo.Z + ")");

                ObjectDataBuff xDataBuff;
                if (mxObjectDataBuff.TryGetValue(xObjectID, out xDataBuff))
                {
                    Squick.IObject xGO = mKernelModule.CreateObject(xObjectID, xInfo.SceneId, 0, strClassName, strConfigID, var);
                    if (null == xGO)
                    {
                        Debug.LogError("ID conflict: " + xObjectID.ToString() + "  ConfigID: " + strClassName);
                        continue;
                    }
                }
            }
        }
        /////////////////////////////////////////////////////////////////////
        private void EGMI_ACK_PROPERTY_INT(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectPropertyInt xData = SquickStruct.ObjectPropertyInt.Parser.ParseFrom(xMsg.MsgData);

            Squick.IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                return;
            }

            IPropertyManager propertyManager = go.GetPropertyManager();

            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                string name = xData.PropertyList[i].PropertyName.ToStringUtf8();
                Int64 data = xData.PropertyList[i].Data;
                Int64 reason = xData.PropertyList[i].Reason;

                IProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_INT);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetInt(data, reason);
            }
        }

        private void EGMI_ACK_PROPERTY_FLOAT(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectPropertyFloat xData = SquickStruct.ObjectPropertyFloat.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                return;
            }

            IPropertyManager propertyManager = go.GetPropertyManager();
            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                string name = xData.PropertyList[i].PropertyName.ToStringUtf8();
                float data = xData.PropertyList[i].Data;
                Int64 reason = xData.PropertyList[i].Reason;

                IProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_FLOAT);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetFloat(data, reason);
            }
        }

        private void EGMI_ACK_PROPERTY_STRING(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectPropertyString xData = SquickStruct.ObjectPropertyString.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IPropertyManager propertyManager = go.GetPropertyManager();

            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                string name = xData.PropertyList[i].PropertyName.ToStringUtf8();
                string data = xData.PropertyList[i].Data.ToStringUtf8();
                Int64 reason = xData.PropertyList[i].Reason;

                IProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetString(data, reason);
            }
        }

        private void EGMI_ACK_PROPERTY_OBJECT(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectPropertyObject xData = SquickStruct.ObjectPropertyObject.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IPropertyManager propertyManager = go.GetPropertyManager();
            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                string name = xData.PropertyList[i].PropertyName.ToStringUtf8();
                SquickStruct.Ident data = xData.PropertyList[i].Data;
                Int64 reason = xData.PropertyList[i].Reason;

                IProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_OBJECT);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetObject(mHelpModule.PBToNF(data), reason);
            }
        }

        private void EGMI_ACK_PROPERTY_VECTOR2(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectPropertyVector2 xData = SquickStruct.ObjectPropertyVector2.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IPropertyManager propertyManager = go.GetPropertyManager();

            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                string name = xData.PropertyList[i].PropertyName.ToStringUtf8();
                SquickStruct.Vector2 data = xData.PropertyList[i].Data;
                Int64 reason = xData.PropertyList[i].Reason;

                IProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR2);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetVector2(mHelpModule.PBToNF(data), reason);
            }
        }

        private void EGMI_ACK_PROPERTY_VECTOR3(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectPropertyVector3 xData = SquickStruct.ObjectPropertyVector3.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IPropertyManager propertyManager = go.GetPropertyManager();
            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                string name = xData.PropertyList[i].PropertyName.ToStringUtf8();
                SquickStruct.Vector3 data = xData.PropertyList[i].Data;
                Int64 reason = xData.PropertyList[i].Reason;

                IProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    DataList.TData var = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_VECTOR3);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetVector3(mHelpModule.PBToNF(data), reason);
            }
        }

        private void EGMI_ACK_PROPERTY_CLEAR(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectPropertyVector3 xData = SquickStruct.ObjectPropertyVector3.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IPropertyManager propertyManager = go.GetPropertyManager();
            //propertyManager.

        }

        private void EGMI_ACK_RECORD_INT(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectRecordInt xData = SquickStruct.ObjectRecordInt.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IRecordManager recordManager = go.GetRecordManager();
            IRecord record = recordManager.GetRecord(xData.RecordName.ToStringUtf8());

            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                record.SetInt(xData.PropertyList[i].Row, xData.PropertyList[i].Col, (int)xData.PropertyList[i].Data);
            }
        }

        private void EGMI_ACK_RECORD_FLOAT(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectRecordFloat xData = SquickStruct.ObjectRecordFloat.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IRecordManager recordManager = go.GetRecordManager();
            IRecord record = recordManager.GetRecord(xData.RecordName.ToStringUtf8());

            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                record.SetFloat(xData.PropertyList[i].Row, xData.PropertyList[i].Col, (float)xData.PropertyList[i].Data);
            }
        }

        private void EGMI_ACK_RECORD_STRING(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectRecordString xData = SquickStruct.ObjectRecordString.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            IRecordManager recordManager = go.GetRecordManager();
            IRecord record = recordManager.GetRecord(xData.RecordName.ToStringUtf8());

            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                record.SetString(xData.PropertyList[i].Row, xData.PropertyList[i].Col, xData.PropertyList[i].Data.ToStringUtf8());
            }
        }

        private void EGMI_ACK_RECORD_OBJECT(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectRecordObject xData = SquickStruct.ObjectRecordObject.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IRecordManager recordManager = go.GetRecordManager();
            IRecord record = recordManager.GetRecord(xData.RecordName.ToStringUtf8());

            for (int i = 0; i < xData.PropertyList.Count; i++)
            {
                record.SetObject(xData.PropertyList[i].Row, xData.PropertyList[i].Col, mHelpModule.PBToNF(xData.PropertyList[i].Data));
            }
        }

        private void EGMI_ACK_SWAP_ROW(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectRecordSwap xData = SquickStruct.ObjectRecordSwap.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IRecordManager recordManager = go.GetRecordManager();
            IRecord record = recordManager.GetRecord(xData.OriginRecordName.ToStringUtf8());

            record.SwapRow(xData.RowOrigin, xData.RowTarget);

        }

        private void ADD_ROW(Guid self, string strRecordName, SquickStruct.RecordAddRowStruct xAddStruct)
        {
            IObject go = mKernelModule.GetObject(self);
            if (go == null)
            {
                Debug.LogError("error id" + self);
                return;
            }

            IRecordManager xRecordManager = go.GetRecordManager();

            Hashtable recordVecDesc = new Hashtable();
            Hashtable recordVecData = new Hashtable();

            for (int k = 0; k < xAddStruct.RecordIntList.Count; ++k)
            {
                SquickStruct.RecordInt addIntStruct = (SquickStruct.RecordInt)xAddStruct.RecordIntList[k];

                if (addIntStruct.Col >= 0)
                {
                    recordVecDesc[addIntStruct.Col] = DataList.VARIANT_TYPE.VTYPE_INT;
                    recordVecData[addIntStruct.Col] = addIntStruct.Data;
                }
            }

            for (int k = 0; k < xAddStruct.RecordFloatList.Count; ++k)
            {
                SquickStruct.RecordFloat addFloatStruct = (SquickStruct.RecordFloat)xAddStruct.RecordFloatList[k];

                if (addFloatStruct.Col >= 0)
                {
                    recordVecDesc[addFloatStruct.Col] = DataList.VARIANT_TYPE.VTYPE_FLOAT;
                    recordVecData[addFloatStruct.Col] = addFloatStruct.Data;

                }
            }

            for (int k = 0; k < xAddStruct.RecordStringList.Count; ++k)
            {
                SquickStruct.RecordString addStringStruct = (SquickStruct.RecordString)xAddStruct.RecordStringList[k];

                if (addStringStruct.Col >= 0)
                {
                    recordVecDesc[addStringStruct.Col] = DataList.VARIANT_TYPE.VTYPE_STRING;
                    if (addStringStruct.Data != null)
                    {
                        recordVecData[addStringStruct.Col] = addStringStruct.Data.ToStringUtf8();
                    }
                    else
                    {
                        recordVecData[addStringStruct.Col] = "";
                    }
                 }
             }

             for (int k = 0; k<xAddStruct.RecordObjectList.Count; ++k)
             {
                 SquickStruct.RecordObject addObjectStruct = (SquickStruct.RecordObject)xAddStruct.RecordObjectList[k];

                 if (addObjectStruct.Col >= 0)
                 {
                     recordVecDesc[addObjectStruct.Col] = DataList.VARIANT_TYPE.VTYPE_OBJECT;
                     recordVecData[addObjectStruct.Col] = mHelpModule.PBToNF(addObjectStruct.Data);

                 }
             }

             for (int k = 0; k<xAddStruct.RecordVector2List.Count; ++k)
             {
                 SquickStruct.RecordVector2 addObjectStruct = (SquickStruct.RecordVector2)xAddStruct.RecordVector2List[k];

                 if (addObjectStruct.Col >= 0)
                 {
                     recordVecDesc[addObjectStruct.Col] = DataList.VARIANT_TYPE.VTYPE_VECTOR2;
                     recordVecData[addObjectStruct.Col] = mHelpModule.PBToNF(addObjectStruct.Data);

                 }
             }

             for (int k = 0; k<xAddStruct.RecordVector3List.Count; ++k)
             {
                 SquickStruct.RecordVector3 addObjectStruct = (SquickStruct.RecordVector3)xAddStruct.RecordVector3List[k];

                 if (addObjectStruct.Col >= 0)
                 {
                     recordVecDesc[addObjectStruct.Col] = DataList.VARIANT_TYPE.VTYPE_VECTOR3;
                     recordVecData[addObjectStruct.Col] = mHelpModule.PBToNF(addObjectStruct.Data);

                 }
             }

             DataList varListDesc = new DataList();
             DataList varListData = new DataList();
             for (int m = 0; m < recordVecDesc.Count; m++)
             {
                 if (recordVecDesc.ContainsKey(m) && recordVecData.ContainsKey(m))
                 {
                     DataList.VARIANT_TYPE nType = (DataList.VARIANT_TYPE)recordVecDesc[m];
                     switch (nType)
                     {
                         case DataList.VARIANT_TYPE.VTYPE_INT:
                         {
                             varListDesc.AddInt(0);
                             varListData.AddInt((Int64) recordVecData[m]);
                         }

                         break;
                         case DataList.VARIANT_TYPE.VTYPE_FLOAT:
                         {
                             varListDesc.AddFloat(0.0f);
                             varListData.AddFloat((float) recordVecData[m]);
                         }
                         break;
                         case DataList.VARIANT_TYPE.VTYPE_STRING:
                         {
                             varListDesc.AddString("");
                             varListData.AddString((string) recordVecData[m]);
                         }
                         break;
                         case DataList.VARIANT_TYPE.VTYPE_OBJECT:
                         {
                             varListDesc.AddObject(new Guid());
                             varListData.AddObject((Guid) recordVecData[m]);
                         }
                         break;
                         case DataList.VARIANT_TYPE.VTYPE_VECTOR2:
                             {
                                 varListDesc.AddVector2(new SVector2());
                                 varListData.AddVector2((SVector2) recordVecData[m]);
                             }
                             break;
                         case DataList.VARIANT_TYPE.VTYPE_VECTOR3:
                             {
                                 varListDesc.AddVector3(new SVector3());
                                 varListData.AddVector3((SVector3) recordVecData[m]);
                             }
                             break;
                         default:
                         break;

                     }
                 }
                 else
                 {
                     //����
                     //Debug.LogException(i);
                 }
             }

             IRecord xRecord = xRecordManager.GetRecord(strRecordName);
             if (null == xRecord)
             {
                Debug.Log("Empty record:" + strRecordName);
                 string strClassName = mKernelModule.QueryPropertyString(self, SquickProtocol.IObject.ClassName);
                 ISClass xLogicClass = mClassModule.GetElement(strClassName);
                 IRecord xStaticRecord = xLogicClass.GetRecordManager().GetRecord(strRecordName);

                 xRecord = xRecordManager.AddRecord(strRecordName, 512, varListDesc, xStaticRecord.GetTagData());
             }

             if (self.IsNull())
             {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < varListData.Count(); ++i)
                {
                    stringBuilder.Append(varListData.GetData(i).ToString());
                    stringBuilder.Append(";");
                }

                Debug.Log(strRecordName + " add row:" + stringBuilder.ToString());
             }

             xRecord.AddRow(xAddStruct.Row, varListData);
         }

        private void EGMI_ACK_ADD_ROW(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectRecordAddRow xData = SquickStruct.ObjectRecordAddRow.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }

            IRecordManager recordManager = go.GetRecordManager();

            for (int i = 0; i < xData.RowData.Count; i++)
            {
                ADD_ROW(mHelpModule.PBToNF(xData.PlayerId), xData.RecordName.ToStringUtf8(), xData.RowData[i]);
            }
        }

        private void EGMI_ACK_REMOVE_ROW(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ObjectRecordRemove xData = SquickStruct.ObjectRecordRemove.Parser.ParseFrom(xMsg.MsgData);

            IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.PlayerId));
            if (go == null)
            {
                Debug.LogError("error id" + xData.PlayerId);
                return;
            }
            IRecordManager recordManager = go.GetRecordManager();
            IRecord record = recordManager.GetRecord(xData.RecordName.ToStringUtf8());

            for (int i = 0; i < xData.RemoveRow.Count; i++)
            {
                record.Remove(xData.RemoveRow[i]);
            }
        }

        private void EGMI_ACK_RECORD_CLEAR(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.MultiObjectRecordList xData = SquickStruct.MultiObjectRecordList.Parser.ParseFrom(xMsg.MsgData);
            for (int i = 0; i < xData.MultiPlayerRecord.Count; ++i)
            {
                SquickStruct.ObjectRecordList objectRecordList = xData.MultiPlayerRecord[i];
                for (int j = 0; j < objectRecordList.RecordList.Count; ++j)
                {
                    IObject go = mKernelModule.GetObject(mHelpModule.PBToNF(objectRecordList.PlayerId));
                    if (go == null)
                    {
                        Debug.LogError("error id" + objectRecordList.PlayerId);
                        return;
                    }

                    SquickStruct.ObjectRecordBase objectRecordBase = objectRecordList.RecordList[j];
                    string recordName = objectRecordBase.RecordName.ToStringUtf8();
                    IRecordManager recordManager = go.GetRecordManager();

                    if (recordManager != null)
                    {
                        IRecord record = recordManager.GetRecord(recordName);
                        if (record != null)
                        {
                            record.Clear();
                        }
                    }
                }
            }
        }

        //////////////////////////////////
        /// 
        private void EGMI_ACK_MOVE(int id, MemoryStream stream)
        {
     
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ReqAckPlayerPosSync xData = SquickStruct.ReqAckPlayerPosSync.Parser.ParseFrom(xMsg.MsgData);

            if (xData.SyncUnit.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < xData.SyncUnit.Count; ++i)
            {

                SquickStruct.PosSyncUnit syncUnit = xData.SyncUnit[i];

                Guid xMover = mHelpModule.PBToNF(syncUnit.Mover);

                if (xMover.IsNull())
                {
                    Debug.LogError("xMover " + Time.time);
                    return;
                }

                if (xMover == mLoginModule.mRoleID && syncUnit.Type == PosSyncUnit.Types.EMoveType.EmtWalk)
                {
                    //平常自己行走不需要同步
                    return;
                }

                GameObject xGameObject = mSceneModule.GetObject(xMover);
                if (!xGameObject)
                {
                    Debug.LogError("xGameObject " + Time.time);
                    return;
                }

                NFHeroMotor xHeroMotor = xGameObject.GetComponent<NFHeroMotor>();
                if (!xHeroMotor)
                {
                    Debug.LogError("xHeroSync " + Time.time);
                    return;
                }

                NFHeroSync xHeroSync = xGameObject.GetComponent<NFHeroSync>();
                if (!xHeroSync)
                {
                    Debug.LogError("xHeroSync " + Time.time);
                    return;
                }

                UnityEngine.Vector3 v = new UnityEngine.Vector3();
                v.x = syncUnit.Pos.X;
                v.y = syncUnit.Pos.Y;
                v.z = syncUnit.Pos.Z;

                Debug.Log("Move " + v);

                if (syncUnit.Type == PosSyncUnit.Types.EMoveType.EmtWalk)
                {
                    xHeroSync.AddSyncData(xData.Sequence, syncUnit);
                }
                else if (syncUnit.Type == PosSyncUnit.Types.EMoveType.EetSpeedy)
                {
                    xHeroMotor.MoveToImmune(v, 0.1f);
                }
                else if (syncUnit.Type == PosSyncUnit.Types.EMoveType.EetTeleport)
                {
                    xHeroMotor.MoveToImmune(v);
                }
            }
        }

        private void EGMI_ACK_SKILL_OBJECTX(int id, MemoryStream stream)
        {
            SquickStruct.MsgBase xMsg = SquickStruct.MsgBase.Parser.ParseFrom(stream);

            SquickStruct.ReqAckUseSkill xData = SquickStruct.ReqAckUseSkill.Parser.ParseFrom(xMsg.MsgData);

            Guid xUser = mHelpModule.PBToNF(xData.User);
            GameObject xGameObject = mSceneModule.GetObject(xUser);
            if (xGameObject)
            {
                NFHeroSync xHeroSync = xGameObject.GetComponent<NFHeroSync>();
                if (!xHeroSync)
                {
                    Debug.LogError("xHeroSync " + Time.time);
                    return;
                }

                xHeroSync.Clear();

                //NFHeroSkill xHeroSkill = xGameObject.GetComponent<NFHeroSkill>();
                //xHeroSkill.AckSkill(xUser, xData.ClientIndex, xData.ServerIndex, xData.SkillId.ToStringUtf8(), xData.EffectData.ToList<SquickStruct.EffectData>());
            }
        }

        public override void AfterInit()
        {
        }
    }
}