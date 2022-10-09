
namespace UniFramework.Tween
{
	public interface ITweenNode
	{
		bool IsDone { get; }

		void OnUpdate(float deltaTime);
		void OnDispose();
		void Kill();
	}
}