using System;


namespace SquickProtocol
{

	public enum AnimaStateType
	{
		Idle = 0,
		Run = 1,   //跑步---需主动切换stop状态to Idle
		Walk = 2,  //前走---------------需主动切换stop状态to Idle
        Jump = 3,  //跳跃
		NormalSkill1 = 11,
		NormalSkill2 = 12,
		NormalSkill3 = 13,
		NormalSkill4 = 14,

		NONE = 100,
	}
}