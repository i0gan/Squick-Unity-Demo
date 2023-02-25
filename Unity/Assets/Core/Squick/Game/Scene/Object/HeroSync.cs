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
    private AnimaStateMachine mAnimaStateMachine;
    private AnimatStateController mAnimatStateController;

    private NetModule mNetModule;
    private LoginModule mLoginModule;
    private HelpModule mHelpModule;
    private IKernelModule mKernelModule;

    private float SYNC_TIME = 0.05f; // 多久同步一次 20 fps

    void Awake()
    {

    }

    private void Start()
    {
        mxHeroMotor = GetComponent<HeroMotor>();
        mxSyncBuffer = GetComponent<HeroSyncBuffer>();
        mxBodyIdent = GetComponent<BodyIdent>();
        mAnimaStateMachine = GetComponent<AnimaStateMachine>();
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


        #region 位置同步

        ReportPos(); // 自己同步位置信息
        // 对方玩家同步信息
        if (mxBodyIdent && mxBodyIdent.GetObjectID() != mLoginModule.mRoleID)
        {
            //
            HeroSyncBuffer.Keyframe keyframe; // 获取到远程缓存的帧序列
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
                lastInterpolationTime = keyframe.InterpolationTime;

                Debug.Log("同步中 : " + (SquickProtocol.AnimaStateType)keyframe.status + "   " + keyframe.Position);
                AnimaStateType stateType = (SquickProtocol.AnimaStateType)keyframe.status;
                switch (stateType)
                {
                    case AnimaStateType.Walk:
                        if (keyframe.Position != Vector3.zero)
                        {
                            mxHeroMotor.MoveTo(keyframe.Position, (AnimaStateType)keyframe.status, true, MeetGoalCallBack);
                            
                        }
                        break;
                    case AnimaStateType.Run:
                        if (keyframe.Position != Vector3.zero)
                        {
                            mxHeroMotor.MoveTo(keyframe.Position, (AnimaStateType)keyframe.status, true, MeetGoalCallBack);

                        }
                        break;
                    case AnimaStateType.Idle:
                        if (UnityEngine.Vector3.Distance(keyframe.Position, mxHeroMotor.transform.position) > 0.1f)
                        {
                            mxHeroMotor.MoveTo(keyframe.Position, (AnimaStateType)keyframe.status, true, MeetGoalCallBack);

                        }
                        else
                        {
                            mxHeroMotor.Stop();

                        }
                        break;
                    case AnimaStateType.NONE:
                        mxHeroMotor.transform.position = keyframe.Position;
                        break;
                       
                    default:
                        mxHeroMotor.UseSkill(stateType, true);
                        break;
                }
            }
        }
        #endregion


        // 动画同步

    }

    Vector3 lastPos = Vector3.zero;
    Vector3 reqPos = Vector3.zero;
    float lastReportTime = 0f;
    bool canFixFrame = true;
    void ReportPos()
    {
        if (lastReportTime <= 0f)
        {
            mNetModule.RequireMove(mLoginModule.mRoleID, (int)AnimaStateType.NONE, mxHeroMotor.transform.position);
        }

        if (Time.time > (SYNC_TIME + lastReportTime)) // 至少20fps进行同步
        {
            lastReportTime = Time.time;

            if (mLoginModule.mRoleID == mxBodyIdent.GetObjectID())
            {
                //Debug.Log("ooookkkkk");
                if (reqPos != mxHeroMotor.transform.position)
                {
                    if (mxHeroMotor.moveToPos != Vector3.zero)
                    {
                        //是玩家自己移动
                        reqPos = mxHeroMotor.moveToPos;
                        canFixFrame = false;
                    }
                    else
                    {
                        //是其他技能导致的唯一，比如屠夫的钩子那种
                        reqPos = mxHeroMotor.transform.position;
                        canFixFrame = false;
                    }

                    if (reqPos == lastPos) // 同一个位置请求，就不用同步到服务器
                    {
                        return;
                    }
                    // 请求移动
                    lastPos = reqPos;
                    //Debug.Log("请求移动：" + mxHeroMotor.transform.position + " to " + reqPos);
                    mNetModule.RequireMove(mLoginModule.mRoleID, (int)mAnimaStateMachine.CurState(), reqPos);
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

    public void ReportSkill(AnimaStateType anim)
    {

        mNetModule.RequireMove(mLoginModule.mRoleID, (int)anim, lastPos);
        //mNetModule.RequireUseSkill(mLoginModule.mRoleID, anim.ToString(), 0, null);
    }


    public void AddSyncData(int sequence, SquickStruct.TransformSyncUnit syncUnit)
    {
        Clear();

        Vector3 pos = new Vector3();
        Vector3 dir = new Vector3();

        if (syncUnit.Position != null)
        {
            pos.x = syncUnit.Position.X;
            pos.y = syncUnit.Position.Y;
            pos.z = syncUnit.Position.Z;
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
            //Debug.Log(keyframe.InterpolationTime + " move " + this.transform.position.ToString() + " TO " + keyframe.Position.ToString() + " Sequence: " + sequence) ;

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
