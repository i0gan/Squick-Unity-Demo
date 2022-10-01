using UnityEngine;
using System.Collections;

using SquickProtocol;
using Squick;

public class NFDizzyState : NFIState
{

    public NFDizzyState(GameObject gameObject, AnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

	}
	

}