using Assets.Scripts.DoSomeManager;
using strange.examples.signals;
using UnityEngine;

namespace Assets.Scripts.HelloWorld
{
	internal class HelloWorldContext : SignalContext
	{
		public HelloWorldContext(MonoBehaviour view) : base(view)
		{
			
		}

		protected override void mapBindings()
		{
			base.mapBindings();

			// we bind a command to StartSignal since it is invoked by SignalContext (the parent class) on Launch()
			commandBinder.Bind<StartSignal>().To<HelloWorldCommand>().Once();

			commandBinder.Bind<DoManagerSignal>().To<DoSomeManagerCommand>().Pooled();

			//bind view to mediator
			mediationBinder.Bind<HelloWorldView>().To<HelloWorldMediator>();

			//			//bind our interface imp
			//			injectionBinder.Bind<IDoSomeManager>().To<MangerAsNormalBehaviour>().ToSingleton();

			MangerAsNormalBehaviour manager = new MangerAsNormalBehaviour();
			injectionBinder.Bind<IDoSomeManager>().ToValue(manager);
		}
	}
}