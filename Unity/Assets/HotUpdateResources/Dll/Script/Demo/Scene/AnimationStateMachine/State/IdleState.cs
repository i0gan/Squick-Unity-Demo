﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SquickProtocol;
using Squick;

public class IdleState : IState
{
    private BodyIdent xBodyIdent;
	private HeroMotor xHeroMotor;
    private HeroSync xHeroSync;

    private Vector3 vector3 = new Vector3();


    private IKernelModule mKernelModule;

    private LoginModule mLoginModule;
    private SceneModule mSceneModule;

    public IdleState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        IPluginManager pluginManager = SquickRoot.Instance().GetPluginManager();

        mKernelModule = pluginManager.FindModule<IKernelModule>();
        mLoginModule = SquickRoot.Instance().GetPluginManager().FindModule<LoginModule>();
        mSceneModule = SquickRoot.Instance().GetPluginManager().FindModule<SceneModule>();
    }

    private bool Fall()
    {
        return false;
    }

    public override void Enter(GameObject gameObject, int index)
    {
        xBodyIdent = gameObject.GetComponent<BodyIdent>();
        xHeroMotor = gameObject.GetComponent<HeroMotor>();
        xHeroSync = gameObject.GetComponent<HeroSync>();

        base.Enter(gameObject, index);

		if (mStateMachine.IsMainRole())
		{
			//xHeroSync.SendSyncMessage();
		}
        //看是否还按住移动选项，如果按住，则继续walk

		if (!xHeroMotor.isOnGround)
        {
            mAnimatStateController.PlayAnimaState(AnimaStateType.Fall, -1); // 播放掉落动画
		}
    }

    public override void Execute(GameObject gameObject)
    {
        base.Execute(gameObject);
        if (gameObject.transform.position.y < -10)
        {
            GameObject go = mSceneModule.GetObject(mLoginModule.mRoleID);
            if (go != null)
            {
                if (Vector3.Distance(go.transform.position, gameObject.transform.position) < 50)
                {
                    vector3.x = gameObject.transform.position.x;
                    vector3.y = 22;
                    vector3.z = gameObject.transform.position.z;
                    gameObject.transform.position = vector3;
                }
            }
        }
    }

	public override void OnCheckInput(GameObject gameObject)
    {
        if (mStateMachine.IsMainRole())
        {
            //hp
            if (mKernelModule.QueryPropertyInt(xBodyIdent.GetObjectID(), SquickProtocol.NPC.HP) <= 0)
            {
                return;
            }

            if (SquickRoot.Instance().GetGameMode() == GAME_MODE.GAME_MODE_2D)
            {
                bool left = false;
                bool right = false;
                bool top = false;

                if (xHeroMotor.GetMoveDrictor().z > 0)
                {
                    top = true;
                }

                if (xHeroMotor.GetMoveDrictor().x < 0)
                {
                    left = true;
                    xHeroMotor.ProcessInput(left, right, top);
                    if (top)
                    {
                        mAnimatStateController.PlayAnimaState(AnimaStateType.JumpStart, -1);
                    }
                    else
                    {
                        mAnimatStateController.PlayAnimaState(AnimaStateType.Run, -1);
                    }
                }
                else if (xHeroMotor.GetMoveDrictor().x > 0)
                {
                    right = true;
                    xHeroMotor.ProcessInput(left, right, top);
                    if (top)
                    {
                        mAnimatStateController.PlayAnimaState(AnimaStateType.JumpStart, -1);
                    }
                    else
                    {
                        mAnimatStateController.PlayAnimaState(AnimaStateType.Run, -1);
                    }
                }
                else
                {
                    if (top)
                    {
                        xHeroMotor.ProcessInput(left, right, top);
                        mAnimatStateController.PlayAnimaState(AnimaStateType.JumpStart, -1);
                    }
                }
            }
            else
            {
                if (xHeroMotor.GetMoveDrictor() != Vector3.zero)
                {
                
                }
            }
        }
	}
}