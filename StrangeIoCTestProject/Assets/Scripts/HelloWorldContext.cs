using strange.examples.signals;
using UnityEngine;

namespace Assets.Scripts
{
	internal class HelloWorldContext : SignalContext
	{
		public HelloWorldContext(MonoBehaviour view) : base(view)
		{
			
		}

		protected override void mapBindings()
		{
			base.mapBindings();
			commandBinder.Bind<StartSignal>().To<HelloWorldCommand>().Once();
		}
	}
}