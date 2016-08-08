using strange.extensions.context.impl;

namespace Assets.Scripts
{
	public class HelloWorldBootstrap : ContextView
	{
		public void Awake()
		{
			this.context = new HelloWorldContext(this);
		}
	}
}
