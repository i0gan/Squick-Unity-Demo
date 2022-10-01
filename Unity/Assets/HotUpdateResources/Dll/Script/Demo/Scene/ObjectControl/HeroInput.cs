using UnityEngine;
using System.Collections;
using SquickProtocol;
using Squick;
using ECM.Controllers;
using ECM.Common;
using ECM.Components;
using System;

public class HeroInput : MonoBehaviour
{
    private UIModule mUIModule;
    private LoginModule mLoginModule;
    private IKernelModule mKernelModule;

    private NFAnimaStateMachine mStateMachineMng;
    private AnimatStateController mAnimatStateController;
    private HeroMotor mHeroMotor;
    private BodyIdent mBodyIdent;

    private NFUIJoystick mJoystick;


    public bool mbInputEnable = false;


    public void SetInputEnable(bool bEnable)
    {
        mbInputEnable = bEnable;
    }

    void Start()
    {
        mStateMachineMng = GetComponent<NFAnimaStateMachine>();
        mAnimatStateController = GetComponent<AnimatStateController>();
        mBodyIdent = GetComponent<BodyIdent>();
        mHeroMotor = GetComponent<HeroMotor>();

        mUIModule = SquickRoot.Instance().GetPluginManager().FindModule<UIModule>();
        mLoginModule = SquickRoot.Instance().GetPluginManager().FindModule<LoginModule>();

        mKernelModule = SquickRoot.Instance().GetPluginManager().FindModule<IKernelModule>();

        mKernelModule.RegisterPropertyCallback(mBodyIdent.GetObjectID(), SquickProtocol.Player.MOVE_SPEED, PropertyMoveSpeedHandler);
        mKernelModule.RegisterPropertyCallback(mBodyIdent.GetObjectID(), SquickProtocol.Player.ATK_SPEED, PropertyAttackSpeedHandler);

        mHeroMotor.angularSpeed = 0f;
    }

    public void PropertyMoveSpeedHandler(Squick.Guid self, string strProperty, DataList.TData oldVar, DataList.TData newVar, Int64 reason)
    {
        //set the animations's speed
        //run
        //walk
    }

    public void PropertyAttackSpeedHandler(Squick.Guid self, string strProperty, DataList.TData oldVar, DataList.TData newVar, Int64 reason)
    {
        //set the animations's speed
        //normally attack1
        //normally attack2
        //normally attack3
        //normally attack4
        //normally attack5
    }

    bool CheckMove()
    {
        //idle
        //jumpland
        //run
        if (mStateMachineMng.CurState() != AnimaStateType.Idle
            && mStateMachineMng.CurState() != AnimaStateType.Idle1
            && mStateMachineMng.CurState() != AnimaStateType.Idle2
            && mStateMachineMng.CurState() != AnimaStateType.Run
            && mStateMachineMng.CurState() != AnimaStateType.Walk)
        {
            return false;
        }


        return true;
    }

    void MoveEvent(Vector3 direction)
    {
        if (mLoginModule.mRoleID == mBodyIdent.GetObjectID())
        {
            // Handle your custom input here...
            //手动的时候，如果AI在追逐中，是不需要输入的
            {
                if (mKernelModule.QueryPropertyInt(mBodyIdent.GetObjectID(), SquickProtocol.Player.HP) > 0)
                {

                    //人工ui输入的，需要和摄像机进行校正才是世界坐标
                    // Transform moveDirection vector to be relative to camera view direction
                    if (Camera.main)
                    {
                        if (direction != Vector3.zero)
                        {
                            if (CheckMove())
                            {
                                Vector3 vDirection = direction.relativeTo(Camera.main.transform);
                                mHeroMotor.MoveTo(this.transform.position + vDirection.normalized * 0.5f);
                            }
                        }
                    }
                }
            }
        }   
    }

    void JoyOnPointerDownHandler(Vector3 direction)
    {
        MoveEvent(direction);

        fLastEventTime = Time.time;
        fLastEventdirection = direction;
    }

    void JoyOnPointerUpHandler(Vector3 direction)
    {
        fLastEventTime = 0f;
        fLastEventdirection = direction;
    }

    void JoyOnPointerDragHandler(Vector3 direction)
    {
        MoveEvent(direction);

        fLastEventTime = Time.time;
        fLastEventdirection = direction;
    }

    float fLastEventTime = 0f;
    Vector3 fLastEventdirection;
    public void FixedUpdate()
    {
        if (mJoystick == null)
        {
            mJoystick = mUIModule.GetUI<NFUIJoystick>();

            if (mJoystick)
            {
                mJoystick.SetPointerDownHandler(JoyOnPointerDownHandler);
                mJoystick.SetPointerDragHandler(JoyOnPointerDragHandler);
                mJoystick.SetPointerUpHandler(JoyOnPointerUpHandler);
            }
        }

        if (fLastEventTime > 0f && Time.time > (fLastEventTime + 0.1f))
        {
            fLastEventTime = Time.time;

            MoveEvent(fLastEventdirection);
        }
    }

    void OnDestroy()
    {
    }
}