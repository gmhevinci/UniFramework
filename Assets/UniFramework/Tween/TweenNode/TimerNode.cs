using UniFramework.Utility;

namespace UniFramework.Tween
{
	/// <summary>
	/// 计时器节点
	/// </summary>
	public class TimerNode : ITweenNode
	{
		private readonly UniTimer _timer;
		private readonly System.Action _triggerCallback;
		public bool IsDone { private set; get; } = false;

		public TimerNode(UniTimer timer, System.Action triggerCallback)
		{
			_timer = timer;
			_triggerCallback = triggerCallback;
		}
		void ITweenNode.OnUpdate(float deltaTime)
		{
			if (_timer.Update(deltaTime))
			{
				_triggerCallback?.Invoke();
			}
			IsDone = _timer.IsOver;
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