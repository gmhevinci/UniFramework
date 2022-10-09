
namespace UniFramework.Tween
{
	/// <summary>
	/// 执行节点
	/// </summary>
	public class ExecuteNode : ITweenNode
	{
		public bool IsDone { private set; get; } = false;
		public System.Action Execute { set; get; }

		void ITweenNode.OnUpdate(float deltaTime)
		{
			Execute.Invoke();
			IsDone = true;
		}
		void ITweenNode.OnDispose()
		{
		}
		void ITweenNode.Kill()
		{
			IsDone = true;
		}
	}
}