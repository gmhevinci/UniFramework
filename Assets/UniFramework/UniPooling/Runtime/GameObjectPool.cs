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
		private readonly Queue<InstantiateOperation> _cache;
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
			_cache = new Queue<InstantiateOperation>(initCapacity);
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
				var operation = AssetHandle.InstantiateAsync(_poolRoot);
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
			foreach (var handle in _cache)
			{
				if (handle.Result != null)
					GameObject.Destroy(handle.Result);
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
		public SpawnHandle Spawn(Transform parent, Vector3 position, Quaternion rotation, bool forceClone, params System.Object[] userDatas)
		{
			InstantiateOperation operation;
			if (forceClone == false && _cache.Count > 0)
				operation = _cache.Dequeue();
			else
				operation = AssetHandle.InstantiateAsync();

			SpawnCount++;
			SpawnHandle handle = new SpawnHandle(this, operation, parent, position, rotation, userDatas);
			YooAssets.StartOperation(handle);
			return handle;
		}

		public void Restore(SpawnHandle handle)
		{
			if (IsDestroyed())
			{
				handle.Destroy();
				return;
			}

			SpawnCount--;
			if (SpawnCount <= 0)
				_lastRestoreRealTime = Time.realtimeSinceStartup;

			// 如果外部逻辑销毁了游戏对象
			if(handle.Status == EOperationStatus.Succeed)
			{
				if (handle.GameObj == null)
				{
					handle.Destroy();
					return;
				}
			}

			// 如果缓存池还未满员
			if (_cache.Count < _maxCapacity)
			{
				var operation = handle.GetOperation();
				SetRestoreCloneObject(operation.Result);
				_cache.Enqueue(operation);
				handle.Release();
			}
			else
			{
				handle.Destroy();
			}
		}
		public void Discard(SpawnHandle handle)
		{
			if (IsDestroyed())
			{
				handle.Destroy();
				return;
			}

			SpawnCount--;
			if (SpawnCount <= 0)
				_lastRestoreRealTime = Time.realtimeSinceStartup;

			handle.Destroy();
		}
		private void SetRestoreCloneObject(GameObject cloneObj)
		{
			if (cloneObj != null)
			{
				cloneObj.SetActive(false);
				cloneObj.transform.SetParent(_poolRoot);
				cloneObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			}
		}
	}
}