﻿using UnityEngine;
using System.Collections;
using SquickProtocol;
using Squick;
using ECM.Controllers;
using ECM.Common;
using ECM.Components;
using System;

public sealed class NFHeroMotor : BaseCharacterController
{
    private IKernelModule mKernelModule;
    private NFSceneModule mSceneModule;
    private LoginModule mLoginModule;
    private NetModule mNetModule;
    private NFUIModule mUIModule;

    private NFAnimaStateMachine mAnimaStateMachine;
    private AnimatStateController mAnima;
    private BodyIdent mBodyIdent;
    private Squick.Guid mxGUID;

    //private float mfInterval = 0.05f;
    //private float mfLastTime = 0f;
    //private Vector3 mvLastJoyDirect = new Vector3();

    public Vector3 moveToPos = Vector3.zero;

    private NFHeroInput mHeroInput;
    private NFHeroSync mHeroSync;

    public delegate bool MeetGoalCalllBack();
    MeetGoalCalllBack meetGoalCasllBack;
    //=============
    #region EDITOR EXPOSED FIELDS

    [Tooltip("The character's walk speed.")]
    [SerializeField]
    private float _walkSpeed = 1.5f;

    [Tooltip("The character's run speed.")]
    [SerializeField]
    private float _runSpeed = 3.5f;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// The character's walk speed.
    /// </summary>

    public float walkSpeed
    {
        get { return _walkSpeed; }
        set { _walkSpeed = Mathf.Max(0.0f, value); }
    }

    /// <summary>
    /// The character's run speed.
    /// </summary>

    public float runSpeed
    {
        get { return _runSpeed; }
        set { _runSpeed = Mathf.Max(0.0f, value); }
    }

    /// <summary>
    /// Walk input command.
    /// </summary>

    public bool walk { get; private set; }

    #endregion

    #region METHODS

    /// <summary>
    /// Get target speed based on character state (eg: running, walking, etc).
    /// </summary>

    private float GetTargetSpeed()
    {
        return walk ? walkSpeed : runSpeed;
    }

    /// <summary>
    /// Overrides 'BaseCharacterController' CalcDesiredVelocity method to handle different speeds,
    /// eg: running, walking, etc.
    /// </summary>

    protected override Vector3 CalcDesiredVelocity()
    {
        // Set 'BaseCharacterController' speed property based on this character state

        speed = GetTargetSpeed();

        // Return desired velocity vectormovem

        return base.CalcDesiredVelocity();
    }

    /// <summary>
    /// Overrides 'BaseCharacterController' Animate method.
    /// 
    /// This shows how to handle your characters' animation states using the Animate method.
    /// The use of this method is optional, for example you can use a separate script to manage your
    /// animations completely separate of movement controller.
    /// 
    /// </summary>

    protected override void Animate()
    {
        // If no animator, return

        if (animator == null)
            return;
    }

    /// <summary>
    /// Overrides 'BaseCharacterController' HandleInput,
    /// to perform custom controller input.
    /// </summary>
    /// 
    public void ProcessInput(bool left, bool right, bool top)
    {
        if (mLoginModule.mRoleID != mxGUID)
        {
            return;
        }

        //moveDirection = v;

        //jump = top;
    }

    public Vector3 GetMoveDrictor()
    {
        return moveDirection;
    }

    public Vector3 GetMovePos()
    {
        return moveToPos;
    }

    public void Stop()
    {
        bool b = false;
        moveDirection = Vector3.zero;
        moveToPos = Vector3.zero;
        if (meetGoalCasllBack != null)
        {
            b = meetGoalCasllBack();
        }

        if (!b)
        {
            mAnima.PlayAnimaState(AnimaStateType.Idle, -1);
        }
    }

    public void MoveToAttackTarget(Vector3 vPos, Squick.Guid id)
    {
        moveToPos = vPos;
        moveDirection = (vPos - this.transform.position).normalized;
    }

    public void MoveTo(Vector3 vPos, bool fromServer = false, MeetGoalCalllBack callBack = null)
    {
        meetGoalCasllBack = callBack;

        vPos.y = this.transform.position.y;
        moveToPos = vPos;
        moveDirection = (vPos - this.transform.position).normalized;

        if (mLoginModule.mRoleID == mxGUID && !fromServer)
        {
            //mNetModule.RequireMove(mLoginModule.mRoleID, 0, moveToPos);
        }


        mAnima.PlayAnimaState(AnimaStateType.Run, -1);
    }

    public void MoveToImmune(Vector3 vPos, bool bFaceToPos = true)
    {
        if (bFaceToPos && mBodyIdent)
        {
            mBodyIdent.LookAt(vPos);
        }

        Stop();
        StopAllCoroutines();
        iTween.Stop();
        this.gameObject.transform.position = vPos;
        moveToPos = vPos;
    }

    public void MoveToImmune(Vector3 vPos, float fTime, bool bFaceToPos = true)
    {
        if (bFaceToPos && mBodyIdent)
        {
            mBodyIdent.LookAt(vPos);
        }

        iTween.Stop();
        iTween.MoveTo(this.gameObject, vPos, fTime);
        moveToPos = vPos;
    }

    protected override void HandleInput()
    {
      
    }

    #endregion

    #region MONOBEHAVIOUR

    /// <summary>
    /// Overrides 'BaseCharacterController' OnValidate method,
    /// to perform this class editor exposed fields validation.
    /// </summary>

    public void ResetHeight()
    {
        LayerMask groundMask = LayerMask.GetMask("Terrain");
        RaycastHit hitInfo;
        bool ret = Physics.Raycast(transform.position + Vector3.up * 50, Vector3.down, out hitInfo);
        if (ret)
        {
            this.transform.position = hitInfo.point;

        }
    }

    public override void OnValidate()
    {
        // Validate 'BaseCharacterController' editor exposed fields

        base.OnValidate();

        // Validate this editor exposed fields

        walkSpeed = _walkSpeed;
        runSpeed = _runSpeed;
    }

    #endregion

    public bool isOnGround
    {
        get { return movement.isOnGround; }
    }

    //=================================================================================================================o
    public override void Awake()
    {
        base.Awake();
        _walkSpeed = 1.5f;
        angularSpeed = 0f;

        mKernelModule = NFRoot.Instance().GetPluginManager().FindModule<IKernelModule>();

        mSceneModule = NFRoot.Instance().GetPluginManager().FindModule<NFSceneModule>();
        mLoginModule = NFRoot.Instance().GetPluginManager().FindModule<LoginModule>();
        mNetModule = NFRoot.Instance().GetPluginManager().FindModule<NetModule>();

        mUIModule = NFRoot.Instance().GetPluginManager().FindModule<NFUIModule>();

        mAnima = GetComponent<AnimatStateController>();
    }

    public override void Update()
    {
        base.Update();

        if (mLoginModule.mRoleID == mxGUID)
        {
            //NFDrawTool.DrawCircle(this.transform, moveToPos, 0.5f, Color.blue);
        }

        mBodyIdent.SetShadowVisible(isGrounded);

        if (!moveToPos.isZero())
        {
            if (Mathf.Abs(moveToPos.x - this.transform.position.x) < 0.1f && Mathf.Abs(moveToPos.z - this.transform.position.z) < 0.1f)
            {
                //now running
                //if (mAnimaStateMachine.CurState() == AnimaStateType.Run
                //    || mAnimaStateMachine.CurState() == AnimaStateType.Walk)
                {
                    //sometimes, the object will swap to skill animation from running animation before we swap to idle
                    Stop();
                }
            }
            else
            {
                moveDirection = (moveToPos - this.transform.position).normalized;
                mBodyIdent.LookAt(moveToPos);
            }
        }
    }

    void Start()
    {
        mAnima = GetComponent<AnimatStateController>();
        mBodyIdent = GetComponent<BodyIdent>();
        mAnimaStateMachine = GetComponent<NFAnimaStateMachine>();
        mHeroInput = GetComponent<NFHeroInput>();
        mHeroSync = GetComponent<NFHeroSync>();

        mxGUID = mBodyIdent.GetObjectID();
        moveDirection = new Vector3();

        mKernelModule.RegisterPropertyCallback(mxGUID, SquickProtocol.Player.MOVE_SPEED, PropertyEventHandler);

    }

    void PropertyEventHandler(Squick.Guid self, string strProperty, DataList.TData oldVar, DataList.TData newVar, Int64 reason)
    {
        this.runSpeed = newVar.IntVal() / 100.0f;
    }

    void OnDestroy()
	{
		int nX = (int)transform.position.x;
		int nY = (int)transform.position.y;
	}

}
