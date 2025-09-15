using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Reference;

namespace UniFramework.Event
{
    public static class UniEvent
    {
        public enum EBroadcastOrder
        {
            /// <summary>
            /// 正序广播
            /// </summary>
            Normal,

            /// <summary>
            /// 倒序广播
            /// </summary>
            Reverse
        }

        /// <summary>
        /// 订阅对象
        /// </summary>
        private class Listener : IReference
        {
            public Action<IEventMessage> Callback { set; get; }
            public bool IsDestroyed { set; get; } = false;
            public void OnSpawn()
            {
                Callback = null;
                IsDestroyed = false;
            }
        }

        /// <summary>
        /// 事件封装
        /// </summary>
        private class EventWrapper
        {
            private readonly Type _eventType;
            private readonly List<Listener> _subscribers = new List<Listener>(100);

            public EventWrapper(Type eventType)
            {
                _eventType = eventType;
            }
            public bool Contains(System.Action<IEventMessage> callback)
            {
                foreach (var listener in _subscribers)
                {
                    if (listener.IsDestroyed == false && listener.Callback == callback)
                        return true;
                }
                return false;
            }
            public void Add(System.Action<IEventMessage> callback)
            {
                if (AllowDuplicateRegistration == false)
                {
                    if (Contains(callback))
                    {
                        UniLogger.Warning($"The event {_eventType.FullName} listener has been registered !");
                        return;
                    }
                }

                var wrapper = UniReference.Spawn<Listener>();
                wrapper.Callback = callback;
                wrapper.IsDestroyed = false;
                _subscribers.Add(wrapper);
            }
            public void Remove(System.Action<IEventMessage> callback)
            {
                foreach (var listener in _subscribers)
                {
                    if (listener.Callback == callback)
                    {
                        listener.IsDestroyed = true; //注意：标记销毁（只移除一个）
                        break;
                    }
                }
            }
            public void Trigger(IEventMessage message)
            {
                bool isDirty = false;

                if (BroadcastOrder == EBroadcastOrder.Normal)
                {
                    // 注意：过程中新注册的订阅不起效！
                    int count = _subscribers.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var listener = _subscribers[i];
                        if (listener.IsDestroyed)
                        {
                            isDirty = true;
                            continue;
                        }

                        try
                        {
                            if (listener.Callback != null)
                                listener.Callback.Invoke(message);
                        }
                        catch (Exception ex)
                        {
                            UniLogger.Error($"Event {_eventType.FullName} listener threw exception: {ex.Message} {ex.StackTrace}");
                        }
                    }
                }
                else if (BroadcastOrder == EBroadcastOrder.Reverse)
                {
                    // 注意：过程中新注册的订阅不起效！
                    int count = _subscribers.Count;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var listener = _subscribers[i];
                        if (listener.IsDestroyed)
                        {
                            isDirty = true;
                            continue;
                        }

                        try
                        {
                            if (listener.Callback != null)
                                listener.Callback.Invoke(message);
                        }
                        catch (Exception ex)
                        {
                            UniLogger.Error($"Event {_eventType.FullName} listener threw exception: {ex.Message} {ex.StackTrace}");
                        }
                    }
                }
                else
                {
                    throw new System.NotImplementedException(BroadcastOrder.ToString());
                }

                // 移除销毁对象
                if (isDirty)
                {
                    for (int i = _subscribers.Count - 1; i >= 0; i--)
                    {
                        var listener = _subscribers[i];
                        if (listener.IsDestroyed)
                        {
                            _subscribers.RemoveAt(i);
                            UniReference.Release(listener);
                        }
                    }
                }

                // 回收消息对象
                if (message is IReference refClass)
                    UniReference.Release(refClass);
            }
            public void Clear()
            {
                foreach (var listener in _subscribers)
                {
                    UniReference.Release(listener);
                }
                _subscribers.Clear();
            }
        }

        /// <summary>
        /// 延迟分发
        /// </summary>
        private class PostWrapper : IReference
        {
            public int PostFrame;
            public IEventMessage Message;

            public void OnSpawn()
            {
                PostFrame = 0;
                Message = null;
            }
        }


        private static bool _isInitialize = false;
        private static GameObject _driver = null;
        private static readonly Dictionary<Type, EventWrapper> _eventWrappers = new Dictionary<Type, EventWrapper>(1000);
        private static readonly List<PostWrapper> _postingList = new List<PostWrapper>(1000);

        /// <summary>
        /// 是否允许重复注册（默认允许）
        /// </summary>
        public static bool AllowDuplicateRegistration { get; set; } = true;

        /// <summary>
        /// 广播模式
        /// </summary>
        public static EBroadcastOrder BroadcastOrder { get; set; } = EBroadcastOrder.Normal;


        /// <summary>
        /// 初始化事件系统
        /// </summary>
        public static void Initalize()
        {
            if (_isInitialize)
                throw new Exception($"{nameof(UniEvent)} is initialized !");

            if (_isInitialize == false)
            {
                // 创建驱动器
                _isInitialize = true;
                _driver = new UnityEngine.GameObject($"[{nameof(UniEvent)}]");
                _driver.AddComponent<UniEventDriver>();
                UnityEngine.Object.DontDestroyOnLoad(_driver);
                UniLogger.Log($"{nameof(UniEvent)} initalize !");
            }
        }

        /// <summary>
        /// 销毁事件系统
        /// </summary>
        public static void Destroy()
        {
            if (_isInitialize)
            {
                ClearAll();

                _isInitialize = false;
                if (_driver != null)
                    GameObject.Destroy(_driver);
                UniLogger.Log($"{nameof(UniEvent)} destroy all !");
            }
        }

        /// <summary>
        /// 更新事件系统
        /// </summary>
        internal static void Update()
        {
            for (int i = _postingList.Count - 1; i >= 0; i--)
            {
                var wrapper = _postingList[i];
                if (UnityEngine.Time.frameCount > wrapper.PostFrame)
                {
                    SendMessage(wrapper.Message);
                    _postingList.RemoveAt(i);
                    UniReference.Release(wrapper);
                }
            }
        }

        /// <summary>
        /// 清空所有监听
        /// </summary>
        public static void ClearAll()
        {
            foreach (var wrapper in _eventWrappers.Values)
            {
                wrapper.Clear();
            }
            _eventWrappers.Clear();
            _postingList.Clear();
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        public static void AddListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            System.Type eventType = typeof(TEvent);
            AddListener(eventType, listener);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        public static void AddListener(System.Type eventType, System.Action<IEventMessage> listener)
        {
            if (_eventWrappers.TryGetValue(eventType, out var wrapper) == false)
            {
                wrapper = new EventWrapper(eventType);
                _eventWrappers[eventType] = wrapper;
            }
            wrapper.Add(listener);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public static void RemoveListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            System.Type eventType = typeof(TEvent);
            RemoveListener(eventType, listener);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public static void RemoveListener(System.Type eventType, System.Action<IEventMessage> listener)
        {
            if (_eventWrappers.TryGetValue(eventType, out var wrapper))
            {
                wrapper.Remove(listener);
            }
        }

        /// <summary>
        /// 实时广播事件
        /// </summary>
        public static void SendMessage(IEventMessage message)
        {
            if (message == null)
                return;

            var eventType = message.GetType();
            if (_eventWrappers.TryGetValue(eventType, out var wrapper))
            {
                wrapper.Trigger(message);
            }
            else
            {
                // 如果事件没人监听，也直接回收
                if (message is IReference refClass)
                    UniReference.Release(refClass);
            }
        }

        /// <summary>
        /// 延迟广播事件
        /// </summary>
        public static void PostMessage(IEventMessage message)
        {
            var wrapper = UniReference.Spawn<PostWrapper>();
            wrapper.PostFrame = UnityEngine.Time.frameCount;
            wrapper.Message = message;
            _postingList.Add(wrapper);
        }
    }
}