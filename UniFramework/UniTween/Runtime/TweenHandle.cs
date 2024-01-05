
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
        private bool _isAbort = false;
        internal int InstanceID { private set; get; }

        /// <summary>
        /// 句柄释放时的回调方法
        /// </summary>
        public System.Action OnDispose { set; get; } = null;

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
            try
            {
                _tweenRoot.OnUpdate(deltaTime);
            }
            catch (System.Exception e)
            {
                _isAbort = true;
                UniLogger.Warning($"Tween exception : {e}");
            }
        }
        internal void Dispose()
        {
            OnDispose?.Invoke();
        }
        internal bool IsCanRemove()
        {
            // 检测游戏对象是否销毁
            if (_safeMode)
            {
                if (_unityObject == null)
                    return true;
            }

            // 检测运行过程是否发生异常
            if (_isAbort)
                return true;

            if (_tweenRoot.Status == ETweenStatus.Completed)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 终止补间动画
        /// </summary>
        public void Abort()
        {
            _isAbort = true;
        }
    }
}