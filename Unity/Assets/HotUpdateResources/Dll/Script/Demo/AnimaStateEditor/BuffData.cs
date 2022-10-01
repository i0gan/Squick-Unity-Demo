using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SquickProtocol
{

	[CreateAssetMenu(fileName = "New BuffData", menuName = "New BuffData")]
	public class BuffData : ScriptableObject
	{
		public List<string> VirtualPointList = new List<string> { "None" };
		public List<BuffStruct> BuffList = new List<BuffStruct>();
	}

	[System.Serializable]
	public class BuffStruct
	{
		public BuffType BuffType;
		public AnimaStateType AnimationType;

	}
}