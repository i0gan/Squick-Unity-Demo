using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SquickProtocol;
using Squick;
using ECM.Components;
using ECM.Controllers;
using System;
using Uquick.Core;
namespace Squick
{
	public class SceneModule : IModule
	{
		private static bool mbInitSend = false;
        private static string mTitleData;

		private IElementModule mElementModule;
		private IKernelModule mKernelModule;
		private ISEventModule mEventModule;
        
		private NetModule mNetModule;
		private HelpModule mHelpModule;
		private LoginModule mLoginModule;

        private UIModule mUIModule;

		private Dictionary<Guid, GameObject> mhtObject = new Dictionary<Guid, GameObject>();
		private int mnScene = 0;
		private bool mbLoadedScene = false;


        public enum PriorityLevel
        {
            SceneObject = 1,
            SceneSound = 2,

            SceneCamera = 700,
            SceneScenario = 800,

            SceneUI = 999,
        }

        public delegate void SceneLoadDelegation(int sceneId);
        class SceneLoadDelegationObject
        {
            public List<SceneLoadDelegation> list = new List<SceneLoadDelegation>();
        }
        private Dictionary<PriorityLevel, SceneLoadDelegationObject> mhtSceneLoadDelegation = new Dictionary<PriorityLevel, SceneLoadDelegationObject>();




        public SceneModule(IPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }

		public override void Awake() 
		{ 
			mKernelModule = mPluginManager.FindModule<IKernelModule>();
			mElementModule = mPluginManager.FindModule<IElementModule>();

			mNetModule = mPluginManager.FindModule<NetModule>();
            mEventModule = mPluginManager.FindModule<ISEventModule>();
            mHelpModule = mPluginManager.FindModule<HelpModule>();

			mLoginModule = mPluginManager.FindModule<LoginModule>();

			mUIModule = mPluginManager.FindModule<UIModule >();
        }

		public override void Init()
		{ 
		}

		public override void AfterInit() 
		{
            mKernelModule.RegisterClassCallBack(SquickProtocol.Player.ThisName, OnClassPlayerEventHandler);
            mKernelModule.RegisterClassCallBack(SquickProtocol.NPC.ThisName, OnClassNPCEventHandler);

		}

		public override void Execute()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                local = true;
            }
        }

        public override void BeforeShut()
        {
            foreach (var item in mhtObject)
            {
                GameObject.DestroyImmediate(item.Value);
            }

            mhtObject.Clear();
        }

		public override void Shut()
        {
        }

        public void AddSceneLoadCallBack(PriorityLevel priorityLevel, SceneLoadDelegation handler)
        {
            if (!mhtSceneLoadDelegation.ContainsKey(priorityLevel))
            {
                mhtSceneLoadDelegation[priorityLevel] = new SceneLoadDelegationObject();
            }

            mhtSceneLoadDelegation[priorityLevel].list.Add(handler);
        }

        public void InitPlayerComponent(Guid xID, GameObject self, bool bMainRole)
        {
            if (null == self)
            {
                return;
            }

            if (!self.GetComponent<Rigidbody>())
            {
                self.AddComponent<Rigidbody>();
            }

            if (!self.GetComponent<HeroSyncBuffer>())
            {
                self.AddComponent<HeroSyncBuffer>();
            }

            if (!self.GetComponent<HeroSync>())
            {
                self.AddComponent<HeroSync>();
            }


			HeroInput xInput = self.GetComponent<HeroInput>();
			if (!xInput)
            {
                xInput = self.AddComponent<HeroInput>();
            }

			if (bMainRole)
            {
                xInput.enabled = true;
                xInput.SetInputEnable(true);
            }
            else
            {
                xInput.enabled = false;
                xInput.SetInputEnable(false);
            }

            if (!self.GetComponent<GroundDetection>())
            {
                GroundDetection groundDetection = self.AddComponent<GroundDetection>();
                groundDetection.enabled = true;
                groundDetection.groundMask = -1;
            }

            if (!self.GetComponent<CharacterMovement>())
            {
                CharacterMovement characterMovement = self.AddComponent<CharacterMovement>();
                characterMovement.enabled = true;
            }

            if (!self.GetComponent<HeroMotor>())
            {
                HeroMotor xHeroMotor = self.AddComponent<HeroMotor>();
                xHeroMotor.enabled = true;
            }

            if (!self.GetComponent<AnimatStateController>())
            {
                AnimatStateController xHeroAnima = self.AddComponent<AnimatStateController>();
                xHeroAnima.enabled = true;
            }

            
            if (!self.GetComponent<AnimaStateMachine>())
            {
                AnimaStateMachine xHeroAnima = self.AddComponent<AnimaStateMachine>();
                xHeroAnima.enabled = true;
            }

            if (bMainRole)
            {

          


                CapsuleCollider xHeroCapsuleCollider = self.GetComponent<CapsuleCollider>();
                xHeroCapsuleCollider.isTrigger = false;
               
            }
            else
            {
                CapsuleCollider xHeroCapsuleCollider = self.GetComponent<CapsuleCollider>();
                Rigidbody rigidbody = self.GetComponent<Rigidbody>();

                string configID = mKernelModule.QueryPropertyString(xID, SquickProtocol.IObject.ConfigID);
                SquickStruct.ENPCType npcType = (SquickStruct.ENPCType)mElementModule.QueryPropertyInt(configID, SquickProtocol.NPC.NPCType);
                //SquickStruct.esub npcSubType = (SquickStruct.ENPCType)mElementModule.QueryPropertyInt(configID, SquickProtocol.NPC.NPCSubType);
                if (npcType == SquickStruct.ENPCType.TurretNpc)
                {
                    //is trigger must false if it is a building
                    // and the kinematic must true
                    xHeroCapsuleCollider.isTrigger = false;
                    //rigidbody.isKinematic = true;
                    //rigidbody.useGravity = true;
                    rigidbody.mass = 10000;
                }
                else
                {

                    xHeroCapsuleCollider.isTrigger = true;
                }

                /*
                if (mKernelModule.QueryPropertyObject(xID, SquickProtocol.NPC.MasterID) == mLoginModule.mRoleID)
                {
                    //your building or your clan member building
                    if (!self.GetComponent<FogCharacter>())
                    {
                        FogCharacter fogCharacter = self.AddComponent<FogCharacter>();
                        fogCharacter.enabled = true;
                        fogCharacter.radius = 8;
                    }
                }
                else
                {
                    if (!self.GetComponent<FogAgent>())
                    {
                        FogAgent fogAgent = self.AddComponent<FogAgent>();
                        fogAgent.enabled = true;
                    }
                }
                */
            }
        }

        private void OnClassPlayerEventHandler(Guid self, int nContainerID, int nGroupID, IObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            if (eType == IObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
            {
            }
            else if (eType == IObject.CLASS_EVENT_TYPE.OBJECT_LOADDATA)
            {
            }
            else if (eType == IObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
            {
                DestroyObject(self);
            }
            else if (eType == IObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH)
            {

                string strCnfID = mKernelModule.QueryPropertyString(self, SquickProtocol.Player.ConfigID);
                DataList.TData data = new DataList.TData(DataList.VARIANT_TYPE.VTYPE_STRING);
                if (strCnfID != "")
                {
                    data.Set(strCnfID);
                }
                else
                {
                    data.Set(strConfigIndex);
                }

				if (data.StringVal().Length > 0)
				{
					OnConfigIDChangeHandler(self, SquickProtocol.Player.ConfigID, data, data, 0);
				}

                mKernelModule.RegisterPropertyCallback(self, SquickProtocol.Player.ConfigID, OnConfigIDChangeHandler);
                mKernelModule.RegisterPropertyCallback(self, SquickProtocol.Player.HP, OnHPChangeHandler);
            }
        }

        private void OnClassNPCEventHandler(Guid self, int nContainerID, int nGroupID, IObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            if (eType == IObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
            {
                

            }
            else if (eType == IObject.CLASS_EVENT_TYPE.OBJECT_LOADDATA)
            {

            }
            else if (eType == IObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
            {
                DestroyObject(self);
            }
            else if (eType == IObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH)
            {
                string strConfigID = mKernelModule.QueryPropertyString(self, SquickProtocol.NPC.ConfigID);
                SVector3 vec3 = mKernelModule.QueryPropertyVector3(self, SquickProtocol.NPC.Position);

                Vector3 vec = new Vector3();
                vec.x = vec3.X();
                vec.y = vec3.Y();
                vec.z = vec3.Z();

                string strPrefabPath = "";
                if (strConfigID.Length <= 0)
                {
                    strPrefabPath = mElementModule.QueryPropertyString("Enemy", NPC.Prefab);
                }
                else
                {
                    strPrefabPath = mElementModule.QueryPropertyString(strConfigID, NPC.Prefab);
                }

                GameObject xNPC = CreateObject(self, strPrefabPath, vec, strClassName);
                if (xNPC == null)
                {
                    Debug.LogError("Create GameObject fail in " + strConfigID + "  " + strPrefabPath);

                    return;
                }

                xNPC.name = strConfigIndex;
                xNPC.transform.Rotate(new Vector3(0, 90, 0));

                BodyIdent xBodyIdent = xNPC.GetComponent<BodyIdent>();
                if (null != xBodyIdent)
                {//不能没有
                    xBodyIdent.enabled = true;
                    xBodyIdent.SetObjectID(self);
                    xBodyIdent.cnfID = strConfigID;
                }
                else
                {
                    Debug.LogError("No 'BodyIdent' component in " + strConfigID + "  " + strPrefabPath);
                }

                InitPlayerComponent(self, xNPC, false);
            }
        }

        private Vector3 GetRenderObjectPosition(Guid self)
        {
            if (mhtObject.ContainsKey(self))
            {
                GameObject xGameObject = (GameObject)mhtObject[self];
                return xGameObject.transform.position;
            }

            return Vector3.zero;
        }

        private void OnHPChangeHandler(Guid self, string strProperty, DataList.TData oldVar, DataList.TData newVar, Int64 reason)
        {
            if (newVar.IntVal() <= 0)
            {
                GameObject go = GetObject(self);
                if (go != null)
                {
                    AnimaStateMachine xStateMachineMng = go.GetComponent<AnimaStateMachine>();
                    if (xStateMachineMng != null)
                    {
                        xStateMachineMng.ChangeState(AnimaStateType.Dead, -1);

                        //show ui
                        //NFUIHeroDie winHeroDie = mUIModule.ShowUI<NFUIHeroDie>();
                        //winHeroDie.ShowReliveUI();
                    }
                }
            }
            else if (newVar.IntVal() > 0 && oldVar.IntVal() <= 0)
            {
                GameObject go = GetObject(self);
                if (go != null)
                {
                    AnimaStateMachine xStateMachineMng = go.GetComponent<AnimaStateMachine>();
                    if (xStateMachineMng != null)
                    {
                        xStateMachineMng.ChangeState (AnimaStateType.Idle, -1);
                    }
                }
            }
        }

        private void OnConfigIDChangeHandler(Guid self, string strProperty, DataList.TData oldVar, DataList.TData newVar, Int64 reason)
        {
            Vector3 vec = GetRenderObjectPosition(self);

            DestroyObject(self);

            if (vec.Equals(Vector3.zero))
            {
                SVector3 vec3 = mKernelModule.QueryPropertyVector3(self, NPC.Position);
                vec.x = vec3.X();
                vec.y = vec3.Y();
                vec.z = vec3.Z();
            }

			string strCnfID = newVar.StringVal();
            string strPrefabPath = mElementModule.QueryPropertyString(strCnfID, NPC.Prefab);
            if (strPrefabPath.Length <= 0)
            {
                strPrefabPath = mElementModule.QueryPropertyString("DefaultObject", NPC.Prefab);
            }
            GameObject xPlayer = CreateObject(self, strPrefabPath, vec, SquickProtocol.Player.ThisName);
            if (xPlayer)
            {
                xPlayer.name = SquickProtocol.Player.ThisName;
                xPlayer.transform.Rotate(new Vector3(0, 90, 0));

                BodyIdent xBodyIdent = xPlayer.GetComponent<BodyIdent>();
                if (null != xBodyIdent)
                {//不能没有
                    xBodyIdent.enabled = true;
                    xBodyIdent.SetObjectID(self);
                    xBodyIdent.cnfID = strCnfID;
                }
                else
                {
                    Debug.LogError("No 'BodyIdent' component in " + strPrefabPath);
                }

                if (self == mLoginModule.mRoleID)
                {
                    InitPlayerComponent(self, xPlayer, true);
                }
                else
                {
                    InitPlayerComponent(self, xPlayer, false);
                }

                
                if (self == mLoginModule.mRoleID)
                {
                    GameObject mainCamera = GameObject.Find("SceneMainCamera");
                    HeroCameraFollow xHeroCameraFollow = mainCamera.GetComponent<HeroCameraFollow>();
                    if (!xHeroCameraFollow)
                    {
                        xHeroCameraFollow = mainCamera.GetComponentInParent<HeroCameraFollow>();
                    }
                    xHeroCameraFollow.SetPlayer(xPlayer.transform);
                }

                Debug.Log("Create Object successful" + SquickProtocol.Player.ThisName + " " + vec.ToString() + " " + self.ToString());
            }
            else
            {
                Debug.LogError("Create Object failed" + SquickProtocol.Player.ThisName + " " + vec.ToString() + " " + self.ToString());
            }

        }
      
        // 创建对象
        public GameObject CreateObject(Guid ident, string strPrefabName, Vector3 vec, string strTag)
        {
            if (!mhtObject.ContainsKey(ident))
            {
                try
                {
                    Debug.Log("创建对象: " + strPrefabName);
                    GameObject xGameObject = GameObject.Instantiate(AssetMgr.Load(strPrefabName)) as GameObject;

                    mhtObject.Add(ident, xGameObject);
                    GameObject.DontDestroyOnLoad(xGameObject);

                    xGameObject.transform.position = vec;

                    return xGameObject;
                }
                catch
                {
                    Debug.LogError("Load Prefab Failed " + ident.ToString() + " Prefab:" + strPrefabName);
                }

            }

            return null;
        }

        public bool DestroyObject(Guid ident)
        {
            if (mhtObject.ContainsKey(ident))
            {
                GameObject xGameObject = (GameObject)mhtObject[ident];
                mhtObject.Remove(ident);

				UnityEngine.Object.Destroy(xGameObject);

                //找到title，一起干掉
				//mTitleModule.Destroy(ident);

                return true;
            }


            return false;
        }

        public GameObject GetObject(Guid ident)
        {
            if (mhtObject.ContainsKey(ident))
            {
                return (GameObject)mhtObject[ident];
            }

            return null;
        }

        public bool AttackObject(Guid ident, Hashtable beAttackInfo, string strStateName, Hashtable resultInfo)
        {
            if (mhtObject.ContainsKey(ident))
            {
                GameObject xGameObject = (GameObject)mhtObject[ident];
                HeroMotor motor = xGameObject.GetComponent<HeroMotor>();
                //motor.Stop();
            }

            return false;
        }


        public int GetCurSceneID()
        {
            return mnScene;
        }


        static bool local = false;
        public void LoadScene(int nSceneID, float fX, float fY, float fZ, string strData)
        {
            mbLoadedScene = true;
            mnScene = nSceneID;
			mTitleData = strData;

            mUIModule.CloseAllUI();
            UILoading xUILoading = mUIModule.ShowUI<UILoading>();
            xUILoading.LoadLevel(nSceneID, new Vector3(fX, fY, fZ));

			if (!mhtObject.ContainsKey(mLoginModule.mRoleID))
            {
                return;
            }
        }

        public void BeforeLoadSceneEnd(int nSceneID)
        {
            foreach (var v in mhtObject)
            {
                HeroMotor heroMotor = v.Value.GetComponent<HeroMotor>();
                heroMotor.ResetHeight();
            }

            Squick.IElement xElement = mElementModule.GetElement(nSceneID.ToString());
            if (null != xElement)
            {
                string strName = xElement.QueryString(SquickProtocol.Scene.SceneName);
                int sceneType = (int)xElement.QueryInt(SquickProtocol.Scene.Type);

            }
        }

   
        // 加载场景完毕
        public void LoadSceneEnd(int nSceneID)
        {
			if (!mbInitSend)
            {
                mbInitSend = true;

                //NFNetController.Instance.mxNetSender.RequireEnterGameFinish (NFNetController.Instance.xMainRoleID);
            }

            if (false == mbLoadedScene)
            {
                return;
            }

            BeforeLoadSceneEnd(nSceneID);

            mbLoadedScene = false;

            //主角贴地，出生点
            /*
            GameObject xGameObject = (GameObject)mhtObject[mLoginModule.mRoleID];
            if (null != xGameObject)
            {
                xGameObject.transform.position = mvSceneBornPos;
                //xGameObject.GetComponent<NFCStateMachineMng> ().ChangeState (AnimaStateType.Idle);
            }
            */


            SquickStruct.ESceneType nType = (SquickStruct.ESceneType)mElementModule.QueryPropertyInt(nSceneID.ToString(), SquickProtocol.Scene.Type);
            mUIModule.CloseAllUI();
            //mUIModule.ShowUI<NFUIMain>();
            //mUIModule.ShowUI<NFUIEstateBar>();
            mUIModule.ShowUI<NFUIJoystick>();

            Debug.Log("LoadSceneEnd: " + nSceneID + " " + nType);

        }

        public void SetVisibleAll(bool bVisible)
        {
            foreach (KeyValuePair<Guid, GameObject> kv in mhtObject)
            {
                GameObject go = (GameObject)kv.Value;
                go.SetActive(bVisible);
            }
        }

        public void SetVisible(Guid ident, bool bVisible)
        {
            if (mhtObject.ContainsKey(ident))
            {
                GameObject xGameObject = (GameObject)mhtObject[ident];
                xGameObject.SetActive(bVisible);
            }
        }

        public void DetroyAll()
        {
            foreach (KeyValuePair<Guid, GameObject> kv in mhtObject)
            {
                GameObject go = (GameObject)kv.Value;
                GameObject.Destroy(go);
            }

            mhtObject.Clear();
        }

        public float GetDistance(Guid xID1, Guid xID2)
        {
            GameObject go1 = GetObject(xID1);
            GameObject go2 = GetObject(xID2);
            if (go1 && go2)
            {
                return Vector3.Distance(go1.transform.position, go2.transform.position);
            }

            return 1000000f;
        }
	}
}
