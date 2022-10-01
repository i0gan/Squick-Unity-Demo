using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SquickProtocol;
using Squick;

public class AnimaStateMachine : MonoBehaviour
{
    private IKernelModule mKernelModule;
    private IElementModule mElementModule;
    private LoginModule mLoginModule;

    private Dictionary<AnimaStateType, IState> mStateDictionary = new Dictionary<AnimaStateType, IState>();

    private float mfHeartBeatTime;
    private AnimatStateController mAnimatStateController;
    private HeroMotor xHeroMotor;
    private BodyIdent xBodyIdent;

    private AnimaStateType mCurrentState = AnimaStateType.NONE;
    private AnimaStateType mLastState = AnimaStateType.NONE;


    public string curState;
    public string lastState;
  

    public void Awake()
    {
		IPluginManager pluginManager = SquickRoot.Instance().GetPluginManager();

        xBodyIdent = GetComponent<BodyIdent>();

        mKernelModule = pluginManager.FindModule<IKernelModule>();
		mElementModule = pluginManager.FindModule<IElementModule>();
		mLoginModule = pluginManager.FindModule<LoginModule>();

        AddState(AnimaStateType.Idle, new IdleState(this.gameObject, AnimaStateType.Idle, this, 1f, 0f, true));
        AddState(AnimaStateType.Idle1, new IdleState(this.gameObject, AnimaStateType.Idle, this, 1f, 0f, true));
        AddState(AnimaStateType.Idle2, new IdleState(this.gameObject, AnimaStateType.Idle, this, 1f, 0f, true));

        AddState(AnimaStateType.Run, new RunState(this.gameObject, AnimaStateType.Run, this, 1f, 0f, true));
        AddState(AnimaStateType.Walk, new WalkState(this.gameObject, AnimaStateType.Walk, this, 1f, 0f, true));
        AddState(AnimaStateType.Dizzy, new DizzyState(this.gameObject, AnimaStateType.Dizzy, this, 1f, 0f));
        AddState(AnimaStateType.Freeze, new FreezeState(this.gameObject, AnimaStateType.Freeze, this, 1f, 0f));
        AddState(AnimaStateType.Block, new BlockState(this.gameObject, AnimaStateType.Block, this, 1f, 0f));
        AddState(AnimaStateType.Fall, new FallState(this.gameObject, AnimaStateType.Fall, this, 1f, 0f));
        AddState(AnimaStateType.Dead, new DeadState(this.gameObject, AnimaStateType.Dead, this, 1f, 0f));
        AddState(AnimaStateType.JumpStart, new NFJumpStartState(this.gameObject, AnimaStateType.JumpStart, this, 1f, 0f));
        AddState(AnimaStateType.Jumping, new JumpingState(this.gameObject, AnimaStateType.Jumping, this, 1f, 0f));
        AddState(AnimaStateType.JumpLand, new JumpLandState(this.gameObject, AnimaStateType.JumpLand, this, 0.1f, 0.4f));
        AddState(AnimaStateType.BeHit1, new BeHitState(this.gameObject, AnimaStateType.BeHit1, this, 1f, 0f));
        AddState(AnimaStateType.BeHit2, new BeHitState(this.gameObject, AnimaStateType.BeHit2, this, 1f, 0f));
        AddState(AnimaStateType.HitFly, new NFHitFlyState(this.gameObject, AnimaStateType.HitFly, this, 1f, 0f));
        AddState(AnimaStateType.Stun, new NFHitFlyState(this.gameObject, AnimaStateType.Stun, this, 1f, 0f));

        AddState(AnimaStateType.DashForward, new DashForwardState(this.gameObject, AnimaStateType.DashForward, this, 1f, 0f));
        AddState(AnimaStateType.DashJump, new DashForwardState(this.gameObject, AnimaStateType.DashJump, this, 1f, 0f));

        AddState(AnimaStateType.Buff1, new Buff1(this.gameObject, AnimaStateType.Buff1, this, 1f, 0f));

        AddState(AnimaStateType.NormalSkill1, new NormalSkill1(this.gameObject, AnimaStateType.NormalSkill1, this, 1f, 0f));
        AddState(AnimaStateType.NormalSkill2, new NormalSkill2(this.gameObject, AnimaStateType.NormalSkill2, this, 1f, 0f));
        AddState(AnimaStateType.NormalSkill3, new NormalSkill3(this.gameObject, AnimaStateType.NormalSkill3, this, 1f, 0f));
        AddState(AnimaStateType.NormalSkill4, new NormalSkill4(this.gameObject, AnimaStateType.NormalSkill4, this, 1f, 0f));
        AddState(AnimaStateType.NormalSkill5, new NFNormalSkill5(this.gameObject, AnimaStateType.NormalSkill5, this, 1f, 0f));

        AddState(AnimaStateType.SpecialSkill1, new NFSpecialSkill1(this.gameObject, AnimaStateType.SpecialSkill1, this, 1f, 0f));
        AddState(AnimaStateType.SpecialSkill2, new SpecialSkill2(this.gameObject, AnimaStateType.SpecialSkill2, this, 1f, 0f));
        AddState(AnimaStateType.SkillThump, new SkillThump(this.gameObject, AnimaStateType.SkillThump, this, 1f, 0f));

    }

    void Start()
    {

  
        mAnimatStateController = GetComponent<AnimatStateController>();
        xHeroMotor = GetComponent<HeroMotor>();

        mAnimatStateController.GetAnimationEvent().AddOnDamageDelegation(OnDamageDelegation);
        mAnimatStateController.GetAnimationEvent().AddOnEndAnimaDelegation(OnEndAnimaDelegation);
        mAnimatStateController.GetAnimationEvent().AddOnStartAnimaDelegation(OnStartAnimaDelegation);
        mAnimatStateController.GetAnimationEvent().AddBulletTouchPosDelegation(OnBulletTouchPositionDelegation);
        mAnimatStateController.GetAnimationEvent().AddBulletTouchTargetDelegation(OnBulletTouchTargetDelegation);

    }

    public void OnStartAnimaDelegation(GameObject self, AnimaStateType eAnimaType, int index)
    {
        ChangeState(eAnimaType, index);
    }

    public void OnEndAnimaDelegation(GameObject self, AnimaStateType eAnimaType, int index)
    {
        
    }

    public void OnDamageDelegation(GameObject self, GameObject target, AnimaStateType eAnimaType, int index)
    {
        //float damage = Random.Range(900000f, 1100000f);
        //NFPrefabManager.Instance.textManager.Add(damage.ToStringScientific(), target.transform);
        Debug.Log("show damage --- " + target.ToString() + " " + eAnimaType.ToString() + " " + index.ToString());
    }

    public void OnBulletTouchPositionDelegation(GameObject self, Vector3 position, AnimaStateType eAnimaType, int index)
    {
        //show damage
        Debug.Log("show damage --- " + position.ToString() + " " + eAnimaType.ToString() + " " + index.ToString());

    }

    public void OnBulletTouchTargetDelegation(GameObject self, GameObject target, AnimaStateType eAnimaType, int index)
    {
        //show damage
        Debug.Log("show damage --- " + target.name + " " + eAnimaType.ToString() + " " + index.ToString());

    }

	public void OnStart()
    {
        
    }

    public void Update()
    {
        if (mCurrentState != AnimaStateType.NONE)
        {
            mStateDictionary[mCurrentState].Execute(this.gameObject);
            mStateDictionary[mCurrentState].OnCheckInput(this.gameObject);
        }
        else
        {
            ChangeState(AnimaStateType.Idle, -1);
        }


        if (Application.isEditor)
        {
            curState = mCurrentState.ToString();
            lastState = mLastState.ToString();
        }
    }

    public void AddState(AnimaStateType eState, IState xState)
    {
        mStateDictionary[eState] = xState;
    }

    public IState GetState(AnimaStateType eState)
    {
        return mStateDictionary[eState];
    }

    public AnimaStateType LastState()
    {
        return mLastState;
    }

    public AnimaStateType CurState()
    {
        return mCurrentState;
    }

    public void ChangeState(AnimaStateType eState, int index, NFStateData data = null)
    {
        if (mCurrentState == eState)
        {
            return;
        }
        
        if (mCurrentState != AnimaStateType.NONE && mStateDictionary.ContainsKey(mCurrentState))
        {
            mStateDictionary[mCurrentState].Exit(this.gameObject);
        }
  
        if (mStateDictionary.ContainsKey(eState))
        {
            mLastState = mCurrentState;
            mCurrentState = eState;

            mStateDictionary[mCurrentState].xStateData = data;
            mStateDictionary[mCurrentState].Enter(this.gameObject, index);
        }
        else
        {
            Debug.LogError("ChangeState to " + mCurrentState + " from " + mLastState);
        }
    }


    public Guid GetGUID()
    {
		return xBodyIdent.GetObjectID();
    }

    public bool IsMainRole()
    {
		if (GetGUID() == mLoginModule.mRoleID)
        {
            return true;
        }

        return false;
    }


    public void OutputStateData(AnimaStateType eNewState, float fSpeed, Vector3 vNowPos, Vector3 vTargetPos, Vector3 vMoveDirection)
    {
    }

    public void InputStateData(AnimaStateType eNewState, AnimaStateType eLastState, float fSpeed, int nTime, Vector3 vNowPos, Vector3 vTargetPos, Vector3 vMoveDirection)
    {
        if (IsMainRole())
        {
            return;
        }

        //Debug.Log ("In SyncData: " + id.ToString() + eNewState.ToString() + " TargetPos: " + vTargetPos.x + "," + vTargetPos.y+ "," + vTargetPos.z );

        NFStateData data = new NFStateData();
        data.vTargetPos = vTargetPos;
        data.fSpeed = fSpeed;
        data.xMoveDirection = vMoveDirection;

    }
}
