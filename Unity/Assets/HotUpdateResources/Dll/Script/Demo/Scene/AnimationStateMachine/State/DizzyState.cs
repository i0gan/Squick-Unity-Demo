using UnityEngine;
using System.Collections;

using SquickProtocol;
using Squick;

public class DizzyState : IState
{

    public DizzyState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

	}
	

}