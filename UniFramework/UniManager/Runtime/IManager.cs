
namespace UniFramework.Manager
{
	public interface IManager
	{
		/// <summary>
		/// 创建管理器
		/// </summary>
		void OnCreate(System.Object createParam);

		/// <summary>
		/// 更新管理器
		/// </summary>
		void OnUpdate();

		/// <summary>
		/// 销毁管理器
		/// </summary>
		void OnDestroy();
	}
}