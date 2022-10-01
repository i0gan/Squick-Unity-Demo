using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SquickProtocol;
using Squick;
using ECM.Components;
using ECM.Controllers;

public class NFJumpLandState : NFIState
{
    public NFJumpLandState(GameObject gameObject, AnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
    }

    private CharacterMovement mCharacterMovement;
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

        mCharacterMovement = gameObject.GetComponent<CharacterMovement>();

        base.Enter(gameObject, index);

        Vector3 v = new Vector3(gameObject.transform.position.x, mCharacterMovement.groundHit.groundPoint.y, gameObject.transform.position.z);
        gameObject.transform.position = v;
    }

    public override void Execute(GameObject gameObject)
    {
		base.Execute(gameObject);

        mAnimatStateController.PlayAnimaState(AnimaStateType.Idle, -1);
    }

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);

        //判断是需要idle还是run
        if (mStateMachine.IsMainRole())
        {
       
        }
    }

    public override void OnCheckInput(GameObject gameObject)
    {
 
    }
}