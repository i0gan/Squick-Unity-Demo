﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SquickProtocol;
using Squick;

public class HookHoldState : IState
{
	HeroInput xInput;
	HeroMotor xHeroMotor;

    public HookHoldState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {

	}


	public override void Enter(GameObject gameObject, int index)
	{

	}
	
	public override void Execute(GameObject gameObject)
	{
		base.Execute(gameObject);

	}

}