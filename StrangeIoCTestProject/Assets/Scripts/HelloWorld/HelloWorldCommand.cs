
using strange.extensions.command.impl;
using UnityEngine;

namespace Assets.Scripts
{
	public class HelloWorldCommand : Command
	{
		public override void Execute()
		{
			Debug.Log("hello fuck world");
		}
	}
}
