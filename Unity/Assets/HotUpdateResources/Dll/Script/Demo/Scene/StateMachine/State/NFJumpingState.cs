using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SquickProtocol;
using Squick;

public class NFJumpingState : IState
{
    public NFJumpingState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
    }
    
	private HeroInput xInput;
	private HeroMotor xHeroMotor;
	private BodyIdent xBodyIdent;
	private AnimatStateController xHeroAnima;

    public override void Enter(GameObject gameObject, int index)
    {
		xBodyIdent = gameObject.GetComponent<BodyIdent>();
        xInput = gameObject.GetComponent<HeroInput>();
        xHeroAnima = gameObject.GetComponent<AnimatStateController>();
        xHeroMotor = gameObject.GetComponent<HeroMotor>();


        base.Enter(gameObject, index);
  
    }

    public override void Execute(GameObject gameObject)
    {
		base.Execute(gameObject);
  
        if (xHeroMotor.isOnGround)
		{
            mAnimatStateController.PlayAnimaState(AnimaStateType.JumpLand, -1);
		}
	}

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);

    }

    public override void OnCheckInput(GameObject gameObject)
    {
        if (mStateMachine.IsMainRole())
        {

        }
    }
}