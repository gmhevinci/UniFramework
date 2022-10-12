using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace UniFramework.Pooling
{
	internal class GameObjectPool
	{
		private readonly Transform _poolRoot;
		private readonly Queue<SpawnHandle> _cache;
		private readonly bool _dontDestroy;
		private readonly int _initCapacity;
		private readonly int _maxCapacity;
		private readonly float _destroyTime;
		private float _lastRestoreRealTime = -1f;

		/// <summary>
		/// 资源句柄
		/// </summary>
		public AssetOperationHandle AssetHandle { private set; get; }
		
		/// <summary>
		/// 资源定位地址
		/// </summary>
		public string Location { private set; get; }

		/// <summary>
		/// 内部缓存总数
		/// </summary>
		public int CacheCount
		{
			get { return _cache.Count; }
		}

		/// <summary>
		/// 外部使用总数
		/// </summary>
		public int SpawnCount { private set; get; } = 0;

		/// <summary>
		/// 是否常驻不销毁
		/// </summary>
		public bool DontDestroy
		{
			get { return _dontDestroy; }
		}


		public GameObjectPool(GameObject poolRoot, string location, bool dontDestroy, int initCapacity, int maxCapacity, float destroyTime)
		{
			_poolRoot = poolRoot.transform;
			Location = location;

			_dontDestroy = dontDestroy;
			_initCapacity = initCapacity;
			_maxCapacity = maxCapacity;
			_destroyTime = destroyTime;

			// 创建缓存池
			_cache = new Queue<SpawnHandle>(initCapacity);
		}
		
		/// <summary>
		/// 创建对象池
		/// </summary>
		public void CreatePool(YooAssetPackage assetPackage)
		{
			// 加载游戏对象
			AssetHandle = assetPackage.LoadAssetAsync<GameObject>(Location);

			// 创建初始对象
			for (int i = 0; i < _initCapacity; i++)
			{
				SpawnHandle operation = new SpawnHandle(this, AssetHandle, Vector3.zero, Quaternion.identity, _poolRoot, true);
				YooAssets.StartOperation(operation);
				_cache.Enqueue(operation);
			}
		}

		/// <summary>
		/// 销毁游戏对象池
		/// </summary>
		public void DestroyPool()
		{
			// 卸载资源对象
			AssetHandle.Release();
			AssetHandle = null;

			// 销毁游戏对象
			foreach (var operation in _cache)
			{
				operation.Destroy();
			}
			_cache.Clear();

			SpawnCount = 0;
		}

		/// <summary>
		/// 查询静默时间内是否可以销毁
		/// </summary>
		public bool CanAutoDestroy()
		{
			if (_dontDestroy)
				return false;
			if (_destroyTime < 0)
				return false;

			if (_lastRestoreRealTime > 0 && SpawnCount <= 0)
				return (Time.realtimeSinceStartup - _lastRestoreRealTime) > _destroyTime;
			else
				return false;
		}

		/// <summary>
		/// 游戏对象池是否已经销毁
		/// </summary>
		public bool IsDestroyed()
		{
			return AssetHandle == null;
		}

		/// <summary>
		/// 获取一个游戏对象
		/// </summary>
		public SpawnHandle Spawn(bool forceClone, params System.Object[] userDatas)
		{
			SpawnHandle operation;
			if (forceClone == false && _cache.Count > 0)
			{
				operation = _cache.Dequeue();
			}
			else
			{
				operation = new SpawnHandle(this, AssetHandle, Vector3.zero, Quaternion.identity, _poolRoot, true);
				YooAssets.StartOperation(operation);
			}

			SpawnCount++;
			return operation;
		}

		public void Restore(SpawnHandle operation)
		{
			if (IsDestroyed())
			{
				operation.Destroy();
				return;
			}

			SpawnCount--;
			if (SpawnCount <= 0)
			{
				_lastRestoreRealTime = Time.realtimeSinceStartup;
			}

			// 1. 资源加载失败
			// 2. 资源还未加载完毕
			// 3. 在异步实例化过程中被取消
			// 4. 外部逻辑销毁了游戏对象
			if (operation.SpawnGameObject == null)
			{
				operation.Destroy();
			}
			else
			{
				if (_cache.Count < _maxCapacity)
				{
					SetRestoreCloneObject(operation.SpawnGameObject);
					_cache.Enqueue(operation);
				}
				else
				{
					operation.Destroy();
				}
			}
		}
		public void Discard(SpawnHandle operation)
		{
			if (IsDestroyed())
			{
				operation.Destroy();
				return;
			}

			SpawnCount--;
			if (SpawnCount <= 0)
			{
				_lastRestoreRealTime = Time.realtimeSinceStartup;
			}

			operation.Destroy();
		}
		private void SetRestoreCloneObject(GameObject cloneObj)
		{
			cloneObj.SetActive(false);
			cloneObj.transform.SetParent(_poolRoot);
			cloneObj.transform.localPosition = Vector3.zero;
		}
	}
}