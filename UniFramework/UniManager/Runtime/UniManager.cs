using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniFramework.Manager
{
	public static class UniManager
	{
		private class ManagerWrapper
		{
			public int Priority { private set; get; }
			public IManager Manager { private set; get; }

			public ManagerWrapper(IManager manager, int priority)
			{
				Manager = manager;
				Priority = priority;
			}
		}

		private static bool _isInitialize = false;
		private static readonly List<ManagerWrapper> _managerList = new List<ManagerWrapper>(100);
		private static MonoBehaviour _behaviour;
		private static bool _isDirty = false;

		/// <summary>
		/// 初始化管理系统
		/// </summary>
		public static void Initialize()
		{
			if (_isInitialize)
				throw new Exception($"{nameof(UniManager)} is initialized !");

			if (_isInitialize == false)
			{
				// 创建驱动器
				_isInitialize = true;
				GameObject driver = new UnityEngine.GameObject($"[{nameof(UniManager)}]");
				_behaviour = driver.AddComponent<UniManagerDriver>();
				UnityEngine.Object.DontDestroyOnLoad(driver);
			}
		}

		/// <summary>
		/// 更新管理系统
		/// </summary>
		internal static void Update()
		{
			// 如果需要重新排序
			if (_isDirty)
			{
				_isDirty = false;
				_managerList.Sort((left, right) =>
				{
					if (left.Priority > right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}

			// 轮询所有管理器
			for (int i = 0; i < _managerList.Count; i++)
			{
				_managerList[i].Manager.OnUpdate();
			}
		}

		/// <summary>
		/// 销毁管理系统
		/// </summary>
		internal static void Destroy()
		{
			if (_isInitialize)
			{
				_isInitialize = false;
				DestroyAll();

				UniLogger.Log($"{nameof(UniManager)} destroy all !");
			}
		}

		/// <summary>
		/// 获取管理器
		/// </summary>
		public static T GetManager<T>() where T : class, IManager
		{
			System.Type managerType = typeof(T);
			for (int i = 0; i < _managerList.Count; i++)
			{
				if (_managerList[i].Manager.GetType() == managerType)
					return _managerList[i].Manager as T;
			}

			UniLogger.Error($"Not found manager : {managerType}");
			return null;
		}

		/// <summary>
		/// 查询管理器是否存在
		/// </summary>
		public static bool Contains<T>() where T : class, IManager
		{
			System.Type managerType = typeof(T);
			for (int i = 0; i < _managerList.Count; i++)
			{
				if (_managerList[i].Manager.GetType() == managerType)
					return true;
			}
			return false;
		}

		/// <summary>
		/// 创建管理器
		/// </summary>
		/// <param name="priority">运行时的优先级，优先级越大越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateManager<T>(int priority = 0) where T : class, IManager
		{
			return CreateManager<T>(null, priority);
		}

		/// <summary>
		/// 创建管理器
		/// </summary>
		/// <param name="createParam">附加参数</param>
		/// <param name="priority">运行时的优先级，优先级越大越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateManager<T>(System.Object createParam, int priority = 0) where T : class, IManager
		{
			if (priority < 0)
				throw new Exception("The priority can not be negative");

			if (Contains<T>())
				throw new Exception($"Manager is already existed : {typeof(T)}");

			// 如果没有设置优先级
			if (priority == 0)
			{
				int minPriority = GetMinPriority();
				priority = --minPriority;
			}

			T manager = Activator.CreateInstance<T>();
			ManagerWrapper wrapper = new ManagerWrapper(manager, priority);
			wrapper.Manager.OnCreate(createParam);
			_managerList.Add(wrapper);
			_isDirty = true;
			return manager;
		}

		/// <summary>
		/// 销毁管理器
		/// </summary>
		public static bool DestroyManager<T>() where T : class, IManager
		{
			var managerType = typeof(T);
			for (int i = 0; i < _managerList.Count; i++)
			{
				if (_managerList[i].Manager.GetType() == managerType)
				{
					_managerList[i].Manager.OnDestroy();
					_managerList.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 开启一个协程
		/// </summary>
		public static Coroutine StartCoroutine(IEnumerator coroutine)
		{
			return _behaviour.StartCoroutine(coroutine);
		}
		public static Coroutine StartCoroutine(string methodName)
		{
			return _behaviour.StartCoroutine(methodName);
		}

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public static void StopCoroutine(Coroutine coroutine)
		{
			_behaviour.StopCoroutine(coroutine);
		}
		public static void StopCoroutine(string methodName)
		{
			_behaviour.StopCoroutine(methodName);
		}

		/// <summary>
		/// 停止所有协程
		/// </summary>
		public static void StopAllCoroutines()
		{
			_behaviour.StopAllCoroutines();
		}

		private static int GetMinPriority()
		{
			int minPriority = 0;
			for (int i = 0; i < _managerList.Count; i++)
			{
				if (_managerList[i].Priority < minPriority)
					minPriority = _managerList[i].Priority;
			}
			return minPriority; //小于等于零
		}
		private static void DestroyAll()
		{
			for (int i = 0; i < _managerList.Count; i++)
			{
				_managerList[i].Manager.OnDestroy();
			}
			_managerList.Clear();
		}
	}
}