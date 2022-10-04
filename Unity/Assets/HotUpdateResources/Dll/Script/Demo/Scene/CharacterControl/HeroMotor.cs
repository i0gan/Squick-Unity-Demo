using UnityEngine;
using System.Collections;
using SquickProtocol;
using Squick;
using ECM.Controllers;
using ECM.Common;
using ECM.Components;
using System;

public sealed class HeroMotor : BaseCharacterController
{
    private IKernelModule mKernelModule;
    private SceneModule mSceneModule;
    private LoginModule mLoginModule;
    private NetModule mNetModule;
    private UIModule mUIModule;

    private AnimaStateMachine mAnimaStateMachine;
    private AnimatStateController mAnima;
    private BodyIdent mBodyIdent;
    private Squick.Guid mxGUID;

    //private float mfInterval = 0.05f;
    //private float mfLastTime = 0f;
    //private Vector3 mvLastJoyDirect = new Vector3();

    public Vector3 moveToPos = Vector3.zero;

    private HeroInput mHeroInput;
    private HeroSync mHeroSync;

    public delegate bool MeetGoalCalllBack();
    MeetGoalCalllBack meetGoalCasllBack;
    //=============
    #region EDITOR EXPOSED FIELDS

    [Tooltip("The character's walk speed.")]
    [SerializeField]
    private float _walkSpeed = 3.0f;

    [Tooltip("The character's run speed.")]
    [SerializeField]
    private float _runSpeed = 6.0f;

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
    private float walkTime = 0.0f;
    private float GetTargetSpeed()
    {
        return speed;
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
    /// 帧同步玩家输入
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
        moveDirection = (vPos - this.transform.position);
    }

    public void UseSkill(AnimaStateType anim)
    {
        //Debug.Log("播放技能动画：" + anim);
        // 本地播放动画
        mAnima.PlayAnimaState(anim, -1);


    }

    // 角色移动
    public void MoveTo(Vector3 vPos, AnimaStateType anim = AnimaStateType.NONE, bool fromServer = false, MeetGoalCalllBack callBack = null)
    {
        //Debug.Log("sssss");
        meetGoalCasllBack = callBack;

        vPos.y = this.transform.position.y;
        moveToPos = vPos;
        moveDirection = (vPos - this.transform.position);
        //Debug.LogWarning("moveDirection: " + moveDirection);
        if (mLoginModule.mRoleID == mxGUID && !fromServer) // 本地移动
        {
            //mNetModule.RequireMove(mLoginModule.mRoleID, 0, moveToPos);
        }

        if (fromServer)
        {
            //Debug.Log("来自服务器的移动");
            // 播放移动动画
            if (anim == AnimaStateType.Idle || anim == AnimaStateType.NONE)
            {
                if (anim != AnimaStateType.Walk)
                {
                    walkTime = Time.time;
                    walkSpeed = 2.0f;
                    _runSpeed = 6.0f; // 跑起来的速度
                }
                walk = true;
                //walkSpeed = 1.5f;
                speed = walkSpeed + (Time.time - walkTime) * (runSpeed - walkSpeed); // 提速
            }
            else if (anim == AnimaStateType.Walk && Time.time - walkTime > 1.0f) // 2秒后开始跑
            {
                walk = false;
            }

            mAnima.PlayAnimaState(anim, -1);
        }
        else
        {
            // 播放移动动画
            if (mAnima.GetLastState() == AnimaStateType.Idle || mAnima.GetLastState() == AnimaStateType.NONE)
            {
                if (mAnima.GetLastState() != AnimaStateType.Walk)
                {
                    walkTime = Time.time;
                    walkSpeed = 2.0f;
                    _runSpeed = 6.0f; // 跑起来的速度
                }
                walk = true;
                mAnima.PlayAnimaState(AnimaStateType.Walk, -1);
                //walkSpeed = 1.5f;
                speed = walkSpeed + (Time.time - walkTime) * (runSpeed - walkSpeed); // 提速

            }
            else if (mAnima.GetLastState() == AnimaStateType.Walk && Time.time - walkTime > 1.0f) // 2秒后开始跑
            {
                walk = false;
                mAnima.PlayAnimaState(AnimaStateType.Run, -1);

            }
        }
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

        mKernelModule = SquickRoot.Instance().GetPluginManager().FindModule<IKernelModule>();

        mSceneModule = SquickRoot.Instance().GetPluginManager().FindModule<SceneModule>();
        mLoginModule = SquickRoot.Instance().GetPluginManager().FindModule<LoginModule>();
        mNetModule = SquickRoot.Instance().GetPluginManager().FindModule<NetModule>();

        mUIModule = SquickRoot.Instance().GetPluginManager().FindModule<UIModule>();

        mAnima = GetComponent<AnimatStateController>();
    }

    public override void Update()
    {
        base.Update();

        if (mLoginModule.mRoleID == mxGUID)
        {
            //DrawTool.DrawCircle(this.transform, moveToPos, 0.5f, Color.blue);
        }

        mBodyIdent.SetShadowVisible(isGrounded);

        if (!moveToPos.isZero())
        {
            if (Mathf.Abs(moveToPos.x - this.transform.position.x) < 0.1f && Mathf.Abs(moveToPos.z - this.transform.position.z) < 0.1f)
            {
                Stop();
            }
            else
            {

                moveDirection = (moveToPos - this.transform.position);
                mBodyIdent.LookAt(moveToPos);
            }
        }
    }

    void Start()
    {
        mAnima = GetComponent<AnimatStateController>();
        mBodyIdent = GetComponent<BodyIdent>();
        mAnimaStateMachine = GetComponent<AnimaStateMachine>();
        mHeroInput = GetComponent<HeroInput>();
        mHeroSync = GetComponent<HeroSync>();

        mxGUID = mBodyIdent.GetObjectID();
        moveDirection = new Vector3();

        mKernelModule.RegisterPropertyCallback(mxGUID, SquickProtocol.Player.MOVE_SPEED, PropertyEventHandler);

    }

    void PropertyEventHandler(Squick.Guid self, string strProperty, DataList.TData oldVar, DataList.TData newVar, Int64 reason)
    {

        this.runSpeed = newVar.IntVal() / 100.0f;
        Debug.Log("PropertyEventHandler: runSpeed: " + runSpeed);
    }

    void OnDestroy()
    {
        int nX = (int)transform.position.x;
        int nY = (int)transform.position.y;
    }

}
