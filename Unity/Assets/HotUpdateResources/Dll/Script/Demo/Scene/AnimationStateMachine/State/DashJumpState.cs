using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SquickProtocol;
using Squick;

public class DashJumpState : IState
{
    public DashJumpState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

    }

    public override void Enter(GameObject gameObject, int index)
    {
        base.Enter(gameObject, index);

		HeroMotor xHeroMotor = gameObject.GetComponent<HeroMotor>();
    }

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);
    }
}