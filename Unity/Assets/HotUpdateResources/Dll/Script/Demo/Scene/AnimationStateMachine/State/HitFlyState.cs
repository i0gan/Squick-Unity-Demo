using UnityEngine;
using System.Collections;
using SquickProtocol;
using Squick;

public class NFHitFlyState : IState
{
    public NFHitFlyState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

	}

}