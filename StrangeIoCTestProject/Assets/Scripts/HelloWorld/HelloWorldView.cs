using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Assets.Scripts.HelloWorld
{
	public class HelloWorldView : View
	{
		public Signal ButtonClick = new Signal();

		private readonly Rect buttonRect = new Rect(0, 0, 200, 50);

		public void OnGUI()
		{
			if (GUI.Button(buttonRect, "Manger"))
			{
				ButtonClick.Dispatch();
			}
		}
	}
}
