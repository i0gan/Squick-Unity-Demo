using UnityEngine;
using System.Collections;
using SquickProtocol;
using Squick;

public class NormalSkill4 : IState
{
    private BodyIdent xBodyIdent;
    private HeroInput xInput;
    private HeroMotor xHeroMotor;
    private HeroSync xHeroSync;

    public NormalSkill4(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
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

        //mAnimatStateController.PlayAnimaState(AnimaStateType.NormalSkill3);
    }

    public override void Execute(GameObject gameObject)
    {
        xHeroMotor.ProcessInput(false, false, false);
    }
}
