
namespace UniFramework.Manager
{
	public abstract class ManagerSingleton<T> where T : class, IManager
	{
		private static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
					UniLogger.Error($"{typeof(T)} is not create. Use {nameof(UniManager)}.{nameof(UniManager.CreateManager)} create.");
				return _instance;
			}
		}

		protected ManagerSingleton()
		{
			if (_instance != null)
				throw new System.Exception($"{typeof(T)} instance already created.");
			_instance = this as T;
		}
		protected void DestroySingleton()
		{
			_instance = null;
		}
	}
}