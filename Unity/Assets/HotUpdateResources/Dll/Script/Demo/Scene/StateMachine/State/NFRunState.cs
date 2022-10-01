using UnityEngine;
using System.Collections;
using SquickProtocol;
using Squick;

public class NFRunState : IState
{

    public NFRunState (GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base (gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
	{

	}
	BodyIdent xBodyIdent;
    AnimatStateController xHeroAnima;
	HeroMotor xHeroMotor;

    private int standCount = 0;
    private Vector3 lastPos = new Vector3();

    public override void Enter (GameObject gameObject, int index)
	{
		xBodyIdent = gameObject.GetComponent<BodyIdent> ();
        xHeroAnima = gameObject.GetComponent<AnimatStateController> ();
		xHeroMotor = gameObject.GetComponent<HeroMotor> ();

		base.Enter (gameObject, index);

        xHeroMotor.speed = xHeroMotor.runSpeed;
        standCount = 0;
    }

	public override void Execute (GameObject gameObject)
	{
		base.Execute (gameObject);


		if (mStateMachine.IsMainRole())
		{
            if (Vector3.Distance(gameObject.transform.position, lastPos) < 0.01f)
            {
                standCount++;
            }
            else
            {
                standCount = 0;
            }

            lastPos = gameObject.transform.position;

            if (standCount > 4)
            {
                xHeroMotor.Stop();
            }
        }
	}

	public override void Exit(GameObject gameObject)
    {

    }

	public override void OnCheckInput(GameObject gameObject)
    {
     
    }
}