
using Assets.Scripts.DoSomeManager;
using strange.extensions.mediation.impl;

namespace Assets.Scripts.HelloWorld
{
	/// <summary>
	/// cpntroller ---> mediator -- view 
	/// </summary>
	public class HelloWorldMediator : Mediator
	{
		[Inject]
		public HelloWorldView View { get; set; }

		[Inject]
		public DoManagerSignal DoSomeManager { get; set; }

		public override void OnRegister()
		{
			base.OnRegister();

			View.ButtonClick.AddListener((() =>
			{
				DoSomeManager.Dispatch();
			}));
		}
	}
}
