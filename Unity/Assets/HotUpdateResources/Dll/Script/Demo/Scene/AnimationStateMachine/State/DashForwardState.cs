using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SquickProtocol;
using Squick;

public class DashForwardState : IState
{
    private HeroMotor xHeroMotor;

    public DashForwardState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        xHeroMotor = gameObject.GetComponent<HeroMotor>();
    }

    public override void Enter(GameObject gameObject, int index)
    {
        base.Enter(gameObject, index);

        if (xStateData != null)
        {
            iTween.MoveTo(gameObject, xStateData.vTargetPos, xStateData.fSpeed);
        }
    }

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);

    }

}