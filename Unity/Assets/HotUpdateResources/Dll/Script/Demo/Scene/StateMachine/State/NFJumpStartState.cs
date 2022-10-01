using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SquickProtocol;
using Squick;

public class NFJumpStartState : NFIState
{
    public NFJumpStartState(GameObject gameObject, AnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
    }
    
	private NFHeroInput xInput;
	private NFHeroMotor xHeroMotor;
	private BodyIdent xBodyIdent;
	private AnimatStateController xHeroAnima;

    public override void Enter(GameObject gameObject, int index)
    {
		xBodyIdent = gameObject.GetComponent<BodyIdent>();
        xInput = gameObject.GetComponent<NFHeroInput>();
        xHeroAnima = gameObject.GetComponent<AnimatStateController>();
        xHeroMotor = gameObject.GetComponent<NFHeroMotor>();


        base.Enter(gameObject, index);

        mAnimatStateController.PlayAnimaState(AnimaStateType.JumpStart, -1);
    }

    public override void Execute(GameObject gameObject)
    {
		base.Execute(gameObject);
  
	}

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);

    }

    public override void OnCheckInput(GameObject gameObject)
    {
   
    }
}