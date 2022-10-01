using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SquickProtocol;

public class NFHookState : IState
{
    public NFHookState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

	}

	HeroInput xInput;
	HeroMotor xHeroMotor;

	public override void Enter(GameObject gameObject, int index)
	{
		xInput = gameObject.GetComponent<HeroInput>();
		xHeroMotor = gameObject.GetComponent<HeroMotor>();

		base.Enter(gameObject, index);



	}

	public override void Execute(GameObject gameObject)
	{
		if (Vector3.Distance(xStateData.vTargetPos, gameObject.transform.position) < 0.1f)
		{
			//GetStateMachine().GetMachineMng().ChangeState (AnimaStateType.HookHold);
		}
	}

}
