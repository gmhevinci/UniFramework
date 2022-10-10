
namespace UniFramework.Tween
{
	/// <summary>
	/// 补间动画句柄
	/// </summary>
	public class TweenHandle
	{
		private readonly ITweenNode _tweenRoot;
		private readonly UnityEngine.Object _unityObject;
		private readonly bool _safeMode;
		internal int InstanceID { private set; get; }

		private TweenHandle()
		{
		}
		internal TweenHandle(ITweenNode tweenRoot, UnityEngine.Object unityObject)
		{
			_tweenRoot = tweenRoot;
			_unityObject = unityObject;

			if (unityObject == null)
			{
				InstanceID = 0;
				_safeMode = false;
			}
			else
			{
				InstanceID = unityObject.GetInstanceID();
				_safeMode = true;
			}
		}
		internal void Update(float deltaTime)
		{
			_tweenRoot.OnUpdate(deltaTime);
		}
		internal void Dispose()
		{
			_tweenRoot.OnDispose();
		}
		internal bool IsCanRemove()
		{
			if (_safeMode)
			{
				if (_unityObject == null)
				{
					_tweenRoot.Abort();
					return true;
				}
			}

			if (_tweenRoot.Status == ETweenStatus.Abort || _tweenRoot.Status == ETweenStatus.Completed)
				return true;
			else
				return false;
		}

		/// <summary>
		/// 终止补间动画
		/// </summary>
		public void Abort()
		{
			_tweenRoot.Abort();
		}
	}
}