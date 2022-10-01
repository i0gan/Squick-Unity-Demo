﻿using UnityEngine;
using System.Collections;
using SquickProtocol;
using Squick;

public class NFSpecialSkill1 : NFIState
{
    private BodyIdent xBodyIdent;
    private HeroInput xInput;
    private HeroMotor xHeroMotor;
    private HeroSync xHeroSync;

    public NFSpecialSkill1(GameObject gameObject, AnimaStateType eState, NFAnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        xBodyIdent = gameObject.GetComponent<BodyIdent>();
        xInput = gameObject.GetComponent<HeroInput>();
        xHeroMotor = gameObject.GetComponent<HeroMotor>();
        xHeroSync = gameObject.GetComponent<HeroSync>();
    }
    // Use this for initialization
    public override void Enter(GameObject gameObject, int index)
    {
        base.Enter(gameObject, index);

        //mAnimatStateController.PlayAnimaState(AnimaStateType.SpecialSkill1);
    }

    public override void Execute(GameObject gameObject)
    {
        xHeroMotor.ProcessInput(false, false, false);
    }
}
