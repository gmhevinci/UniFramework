using UnityEngine;
using YooAsset;

namespace UniFramework.Pooling
{
	public sealed class SpawnHandle : GameAsyncOperation
	{
		private enum ESteps
		{
			None,
			Clone,
			Done,
		}

		private readonly GameObjectPool _pool;
		private readonly AssetOperationHandle _handle;
		private readonly Vector3 _position;
		private readonly Quaternion _rotation;
		private readonly Transform _parent;
		private readonly bool _setPositionRotation;
		private ESteps _steps = ESteps.None;

		/// <summary>
		/// 实例化的游戏对象
		/// </summary>
		public GameObject SpawnGameObject = null;

		/// <summary>
		/// 用户自定义数据集
		/// </summary>
		public System.Object[] UserDatas { private set; get; }

		private SpawnHandle()
		{
		}
		internal SpawnHandle(GameObjectPool pool, AssetOperationHandle handle, Vector3 position, Quaternion rotation, Transform parent, bool setPositionRotation, params System.Object[] userDatas)
		{
			_pool = pool;
			_handle = handle;
			_position = position;
			_rotation = rotation;
			_parent = parent;
			_setPositionRotation = setPositionRotation;
			UserDatas = userDatas;
		}
		protected override void OnStart()
		{
			_steps = ESteps.Clone;
		}
		protected override void OnUpdate()
		{
			if (_steps == ESteps.None || _steps == ESteps.Done)
				return;

			if (_steps == ESteps.Clone)
			{
				if (_handle.IsValid == false)
				{
					_steps = ESteps.Done;
					Status = EOperationStatus.Failed;
					Error = $"{nameof(AssetOperationHandle)} is invalid.";
					return;
				}

				if (_handle.IsDone == false)
					return;

				if (_handle.AssetObject == null)
				{
					_steps = ESteps.Done;
					Status = EOperationStatus.Failed;
					Error = $"{nameof(AssetOperationHandle.AssetObject)} is null.";
					return;
				}

				if (_setPositionRotation)
				{
					if (_parent == null)
						SpawnGameObject = Object.Instantiate(_handle.AssetObject as GameObject, _position, _rotation);
					else
						SpawnGameObject = Object.Instantiate(_handle.AssetObject as GameObject, _position, _rotation, _parent);
				}
				else
				{
					if (_parent == null)
						SpawnGameObject = Object.Instantiate(_handle.AssetObject as GameObject);
					else
						SpawnGameObject = Object.Instantiate(_handle.AssetObject as GameObject, _parent);
				}

				_steps = ESteps.Done;
				Status = EOperationStatus.Succeed;
			}
		}

		/// <summary>
		/// 销毁
		/// </summary>
		internal void Destroy()
		{
			Cancel();

			if (SpawnGameObject != null)
			{
				GameObject.Destroy(SpawnGameObject);
			}
		}
		private void Cancel()
		{
			if (IsDone == false)
			{
				_steps = ESteps.Done;
				Status = EOperationStatus.Failed;
				Error = $"User cancelled !";
				ClearCompletedCallback();
			}
		}

		/// <summary>
		/// 回收
		/// </summary>
		public void Restore()
		{
			ClearCompletedCallback();
			_pool.Restore(this);
		}

		/// <summary>
		/// 丢弃
		/// </summary>
		public void Discard()
		{
			ClearCompletedCallback();
			_pool.Discard(this);
		}

		/// <summary>
		/// 等待异步实例化结束
		/// </summary>
		public void WaitForAsyncComplete()
		{
			if (_steps == ESteps.Done)
				return;
			_handle.WaitForAsyncComplete();
			OnUpdate();
		}
	}
}
