using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SquickProtocol;
using Squick;
using ECM.Components;
using ECM.Controllers;

public class HeroSync : MonoBehaviour 
{
	private HeroSyncBuffer mxSyncBuffer;
	private HeroMotor mxHeroMotor;

	private BodyIdent mxBodyIdent;
    private NFAnimaStateMachine mAnimaStateMachine;
    private AnimatStateController mAnimatStateController;

    private NetModule mNetModule;
	private LoginModule mLoginModule;
    private HelpModule mHelpModule;
    private IKernelModule mKernelModule;

    private float SYNC_TIME = 0.05f;

    void Awake () 
	{

	}

    private void Start()
    {
        mxHeroMotor = GetComponent<HeroMotor>();
        mxSyncBuffer = GetComponent<HeroSyncBuffer>();
        mxBodyIdent = GetComponent<BodyIdent>();
        mAnimaStateMachine = GetComponent<NFAnimaStateMachine>();
        mAnimatStateController = GetComponent<AnimatStateController>();

        mNetModule = SquickRoot.Instance().GetPluginManager().FindModule<NetModule>();
        mLoginModule = SquickRoot.Instance().GetPluginManager().FindModule<LoginModule>();
        mHelpModule = SquickRoot.Instance().GetPluginManager().FindModule<HelpModule>();
        mKernelModule = SquickRoot.Instance().GetPluginManager().FindModule<IKernelModule>();
    }

    bool CheckState()
    {
		return true;
	}

    private bool MeetGoalCallBack()
    {
        if (mxSyncBuffer.Size() > 0)
        {
            FixedUpdate();
            return true;
        }

        return false;
    }

    float moveSpeed = 2.0f;
    int lastInterpolationTime = 0;
    private void FixedUpdate()
    {
        ReportPos();

        if (mxBodyIdent && mxBodyIdent.GetObjectID() != mLoginModule.mRoleID)
		{
            HeroSyncBuffer.Keyframe keyframe;
            if (mxSyncBuffer.Size() > 1)
            {
                keyframe = mxSyncBuffer.LastKeyframe();
            }
            else
            {
                keyframe = mxSyncBuffer.NextKeyframe();
            }

            if (keyframe != null)
            {
                mxHeroMotor.walkSpeed = moveSpeed;
                mxHeroMotor.runSpeed = moveSpeed;

                float dis = Vector3.Distance(keyframe.Position, mxHeroMotor.transform.position);
                if (dis > 1f)
                {
                    mxHeroMotor.walkSpeed = dis / SYNC_TIME;
                    mxHeroMotor.runSpeed = dis / SYNC_TIME;
                }

                lastInterpolationTime = keyframe.InterpolationTime;


                AnimaStateType stateType = (SquickProtocol.AnimaStateType)keyframe.status;
                switch (stateType)
                {
                    case AnimaStateType.Run:
                        if (keyframe.Position != Vector3.zero)
                        {
                            mxHeroMotor.MoveTo(keyframe.Position, true, MeetGoalCallBack);
                        }
                        break;
                    case AnimaStateType.Idle:
                        if (UnityEngine.Vector3.Distance(keyframe.Position , mxHeroMotor.transform.position) > 0.1f)
                        {
                            mxHeroMotor.MoveTo(keyframe.Position, true, MeetGoalCallBack);
                        }
                        else
                        {
                            mxHeroMotor.Stop();
                        }
                        break;
                    case AnimaStateType.Stun:
                        mAnimatStateController.PlayAnimaState(AnimaStateType.Stun, 0);
                        break;
                    case AnimaStateType.NONE:
                        mxHeroMotor.transform.position = keyframe.Position;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    Vector3 lastPos = Vector3.zero;
    float lastReportTime = 0f;
    bool canFixFrame = true;
    void ReportPos()
    {
        if (lastReportTime <= 0f)
        {
            mNetModule.RequireMove(mLoginModule.mRoleID, (int)AnimaStateType.NONE, mxHeroMotor.transform.position);
        }

        if (Time.time > (SYNC_TIME + lastReportTime))
        {
            lastReportTime = Time.time;

            if (mLoginModule.mRoleID == mxBodyIdent.GetObjectID())
            {
                if (lastPos != mxHeroMotor.transform.position)
                {
                    if (mxHeroMotor.moveToPos != Vector3.zero)
                    {
                        //是玩家自己移动
                        lastPos = mxHeroMotor.moveToPos;
                        canFixFrame = false;
                    }
                    else
                    {
                        //是其他技能导致的唯一，比如屠夫的钩子那种
                        lastPos = mxHeroMotor.transform.position;
                        canFixFrame = false;
                    }

                    mNetModule.RequireMove(mLoginModule.mRoleID, (int)mAnimaStateMachine.CurState(), lastPos);
                }
                else
                {
                    //fix last pos
                    if (canFixFrame)
                    {
                        canFixFrame = false;
                        mNetModule.RequireMove(mLoginModule.mRoleID, (int)mAnimaStateMachine.CurState(), lastPos);
                    }
                }
            }
        }
    }

    public void AddSyncData(int sequence, SquickStruct.PosSyncUnit syncUnit)
    {
        Clear();

        Vector3 pos = new Vector3();
        Vector3 dir = new Vector3();

        if (syncUnit.Pos != null)
        {
            pos.x = syncUnit.Pos.X;
            pos.y = syncUnit.Pos.Y;
            pos.z = syncUnit.Pos.Z;
        }

        if (syncUnit.Orientation != null)
        {
            dir.x = syncUnit.Orientation.X;
            dir.y = syncUnit.Orientation.Y;
            dir.z = syncUnit.Orientation.Z;
        }

        var keyframe = new HeroSyncBuffer.Keyframe();
        keyframe.Position = pos;
        keyframe.Director = dir;
        keyframe.status = syncUnit.Status;
        keyframe.InterpolationTime = sequence;

        if (mxSyncBuffer)
        {
            Debug.Log(keyframe.InterpolationTime + " move " + this.transform.position.ToString() + " TO " + keyframe.Position.ToString());

            mxSyncBuffer.AddKeyframe(keyframe);
        }
    }

    public void Clear()
    {
        if (mxSyncBuffer)
        {
            mxSyncBuffer.Clear();
        }
    }
}
