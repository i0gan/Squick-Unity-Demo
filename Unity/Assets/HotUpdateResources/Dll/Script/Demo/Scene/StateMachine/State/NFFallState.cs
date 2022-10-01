using UnityEngine;
using System.Collections;
using SquickProtocol;
using Squick;

public class NFFallState : IState
{
  
    private HeroMotor xHeroMotor;

    private LoginModule mLoginModule;
    private SceneModule mSceneModule;

    private Vector3 vector3 = new Vector3();

    public NFFallState(GameObject gameObject, AnimaStateType eState, AnimaStateMachine xStateMachine, float fHeartBeatTime, float fExitTime, bool input = false)
        : base(gameObject, eState, xStateMachine, fHeartBeatTime, fExitTime, input)
    {
        xHeroMotor = gameObject.GetComponent<HeroMotor>();
        mLoginModule = SquickRoot.Instance().GetPluginManager().FindModule<LoginModule>();
        mSceneModule = SquickRoot.Instance().GetPluginManager().FindModule<SceneModule>();
    }


	public override void Enter(GameObject gameObject, int index)
	{
		base.Enter(gameObject, index);
  
        //Tweener tweener = gameObject.transform.DOMove(new Vector3(fX, fY, gameObject.transform.position.z), fTime);
        //tweener.SetEase(Ease.InQuad).OnComplete(HandleTweenCallback);
    }

	public override void Execute(GameObject gameObject)
	{
		if (xHeroMotor.isOnGround)
        {
            mAnimatStateController.PlayAnimaState(AnimaStateType.JumpLand, -1);
        }
        else
        {
            if (gameObject.transform.position.y < -5)
            {
                GameObject go = mSceneModule.GetObject(mLoginModule.mRoleID);
                if (go != null)
                {
                    if (Vector3.Distance(go.transform.position, gameObject.transform.position) < 50)
                    {
                        vector3.x = gameObject.transform.position.x;
                        vector3.y = 22;
                        vector3.z = gameObject.transform.position.z;
                        gameObject.transform.position = vector3;
                    }
                }
            }
        }
	}
}
