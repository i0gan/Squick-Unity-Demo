using UnityEngine;
using System.Collections;

using SquickProtocol;
using Squick;

public class NFDeadState : IState
{
    private HeroMotor xHeroMotor;
    private float fStartTime = 0f;
    private bool bShowUI = false;

    UIModule mUIModule;

    public NFDeadState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        mUIModule = SquickRoot.Instance().GetPluginManager().FindModule<UIModule>();
    }

    public override void Enter(GameObject gameObject, int index)
    {
        base.Enter(gameObject, index);

        fStartTime = Time.time;
        bShowUI = false;

        xHeroMotor = gameObject.GetComponent<HeroMotor>();
        xHeroMotor.Stop();
    }

    public override void Execute(GameObject gameObject)
    {
        if (Time.time - 1f > fStartTime && bShowUI == false)
        {
            if (mStateMachine.IsMainRole())
            {
                bShowUI = true;

                //NFUIHeroDie winHeroDie = mUIModule.ShowUI<NFUIHeroDie>();
                //winHeroDie.ShowReliveUI();
            }
        }

        if (gameObject.transform.position.y < -10)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 22, gameObject.transform.position.z);
        }
    }

    public override void Exit(GameObject gameObject)
    {
        base.Exit(gameObject);

    }
}
