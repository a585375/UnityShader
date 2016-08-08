using strange.extensions.context.impl;

namespace Assets.Scripts.HelloWorld
{
	/// <summary>
	/// game entry,the game only one
	/// imp Monobehavir
	/// </summary>
	public class HelloWorldBootstrap : ContextView
	{
		public void Awake()
		{
			this.context = new HelloWorldContext(this);
		}
	}
}
