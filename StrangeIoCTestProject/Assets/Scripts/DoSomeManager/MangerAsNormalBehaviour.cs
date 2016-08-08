
using UnityEngine;

namespace Assets.Scripts.DoSomeManager
{
	public class MangerAsNormalBehaviour : IDoSomeManager
	{
		public MangerAsNormalBehaviour()
		{
		}

		public void DoManager()
		{
			Debug.Log("manger imp as a normal class");
		}
	}
}
