using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Assets.Scripts.DoSomeManager
{
	public class DoManagerSignal : Signal { }

	public class DoSomeManagerCommand : Command
	{
		[Inject]
		public IDoSomeManager Manager { get; set; }

		public override void Execute()
		{
			Manager.DoManager();
		}
	}
}
 