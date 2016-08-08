
using strange.examples.signals;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Assets.Scripts
{
	public class SignalContext : MVCSContext
	{
		public SignalContext(MonoBehaviour view) : base(view)
		{

		}

		protected override void addCoreComponents()
		{
			base.addCoreComponents();

			injectionBinder.Unbind<ICommandBinder>();
			injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
		}

		public override void Launch()
		{
			base.Launch();

			Signal startSignal = injectionBinder.GetInstance<StartSignal>();
			startSignal.Dispatch();
		}
	}
}
