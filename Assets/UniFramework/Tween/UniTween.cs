using System.Collections.Generic;
using UniFramework.Utility;

namespace UniFramework.Tween
{
	/// <summary>
	/// 补间管理器
	/// </summary>
	public static class UniTween
	{
		private class TweenWrapper
		{
			public int GroupID { private set; get; }
			public long TweenUID { private set; get; }
			public ITweenNode TweenRoot { private set; get; }
			public UnityEngine.Object SafeObject { private set; get; }
			private readonly bool _safeMode = false;

			public TweenWrapper(int groupID, long tweenUID, ITweenNode tweenRoot, UnityEngine.Object safeObject)
			{
				GroupID = groupID;
				TweenUID = tweenUID;
				TweenRoot = tweenRoot;
				SafeObject = safeObject;
				_safeMode = safeObject != null;
			}
			public bool IsSafe()
			{
				if (_safeMode == false)
					return true;
				return SafeObject != null;
			}
		}

		private static int StaticTweenUID = 0;
		private static readonly List<TweenWrapper> _wrappers = new List<TweenWrapper>(1000);
		private static readonly List<TweenWrapper> _remover = new List<TweenWrapper>(1000);

		/// <summary>
		/// 是否忽略时间戳缩放
		/// </summary>
		public static bool IgnoreTimeScale { set; get; } = false;

		/// <summary>
		/// 所有补间动画的播放速度
		/// </summary>
		public static float PlaySpeed { set; get; } = 1f;

		internal static void Update()
		{
			_remover.Clear();

			// 更新所有补间动画
			float delatTime = IgnoreTimeScale ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime;
			delatTime *= PlaySpeed;
			for (int i = 0; i < _wrappers.Count; i++)
			{
				var wrapper = _wrappers[i];
				if (wrapper.IsSafe() == false)
				{
					wrapper.TweenRoot.Kill();
					_remover.Add(wrapper);
					continue;
				}

				if (wrapper.TweenRoot.IsDone)
					_remover.Add(wrapper);
				else
					wrapper.TweenRoot.OnUpdate(delatTime);
			}

			// 移除完成的补间动画
			for (int i = 0; i < _remover.Count; i++)
			{
				var wrapper = _remover[i];
				_wrappers.Remove(wrapper);
				wrapper.TweenRoot.OnDispose();
			}
		}
		
		/// <summary>
		/// 播放一个补间动画
		/// </summary>
		/// <param name="tweenRoot">补间根节点</param>
		/// <param name="go">游戏对象</param>
		/// <returns>补间动画唯一ID</returns>
		public static long Play(ITweenNode tweenRoot, UnityEngine.GameObject go = null)
		{
			int groupID = 0;
			if (go != null)
				groupID = go.GetInstanceID();
			return CreateTween(tweenRoot, go, groupID);
		}

		/// <summary>
		/// 播放一个补间动画
		/// </summary>
		/// <param name="tweenChain">补间根节点</param>
		/// <param name="go">游戏对象</param>
		/// <returns>补间动画唯一ID</returns>
		public static long Play(ITweenChain tweenChain, UnityEngine.GameObject go = null)
		{
			ITweenNode tweenRoot = tweenChain as ITweenNode;
			if (tweenRoot == null)
				throw new System.InvalidCastException();

			return Play(tweenRoot, go);
		}

		/// <summary>
		/// 播放一个补间动画
		/// </summary>
		/// <param name="tweenChain">补间根节点</param>
		/// <param name="go">游戏对象</param>
		/// <returns>补间动画唯一ID</returns>
		public static long Play(ChainNode chainNode, UnityEngine.GameObject go = null)
		{
			ITweenNode tweenRoot = chainNode as ITweenNode;
			if (tweenRoot == null)
				throw new System.InvalidCastException();

			return Play(tweenRoot, go);
		}

		/// <summary>
		/// 中途关闭一个补间动画
		/// </summary>
		/// <param name="tweenUID">补间动画唯一ID</param>
		public static void Kill(long tweenUID)
		{
			TweenWrapper wrapper = GetTweenWrapper(tweenUID);
			if (wrapper != null)
				wrapper.TweenRoot.Kill();
		}

		/// <summary>
		/// 中途关闭一组补间动画
		/// </summary>
		/// <param name="go">游戏对象</param>
		public static void Kill(UnityEngine.GameObject go)
		{
			int groupID = go.GetInstanceID();
			TweenWrapper wrapper = GetTweenWrapper(groupID);
			if (wrapper != null)
				wrapper.TweenRoot.Kill();
		}


		/// <summary>
		/// 创建补间动画
		/// </summary>
		/// <param name="tweenRoot">补间根节点</param>
		/// <param name="safeObject">安全游戏对象：如果安全游戏对象被销毁，补间动画会自动终止</param>
		/// <param name="groupID">补间组ID</param>
		/// <returns>补间动画唯一ID</returns>
		private static long CreateTween(ITweenNode tweenRoot, UnityEngine.Object safeObject, int groupID = 0)
		{
			if (tweenRoot == null)
			{
				UniLogger.Warning("Tween root is null.");
				return -1;
			}

			if (Contains(tweenRoot))
			{
				UniLogger.Warning("Tween root is running.");
				return -1;
			}

			long tweenUID = ++StaticTweenUID;
			TweenWrapper wrapper = new TweenWrapper(groupID, tweenUID, tweenRoot, safeObject);
			_wrappers.Add(wrapper);
			return wrapper.TweenUID;
		}

		private static bool Contains(ITweenNode tweenRoot)
		{
			for (int i = 0; i < _wrappers.Count; i++)
			{
				var wrapper = _wrappers[i];
				if (wrapper.TweenRoot == tweenRoot)
					return true;
			}
			return false;
		}
		private static TweenWrapper GetTweenWrapper(long tweenUID)
		{
			for (int i = 0; i < _wrappers.Count; i++)
			{
				var wrapper = _wrappers[i];
				if (wrapper.TweenUID == tweenUID)
					return wrapper;
			}
			return null;
		}
		private static TweenWrapper GetTweenWrapper(int groupID)
		{
			for (int i = 0; i < _wrappers.Count; i++)
			{
				var wrapper = _wrappers[i];
				if (wrapper.GroupID != 0 && wrapper.GroupID == groupID)
					return wrapper;
			}
			return null;
		}
	}
}