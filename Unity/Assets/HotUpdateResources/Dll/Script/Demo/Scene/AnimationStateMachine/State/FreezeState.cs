using UnityEngine;
using System.Collections;
using SquickProtocol;
using Squick;

public class FreezeState : IState
{
    public FreezeState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

	}

}