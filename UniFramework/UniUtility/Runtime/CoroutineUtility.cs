using System;
using System.Collections;
using UnityEngine;

namespace UniFramework.Utility {

    /// <summary>
    /// 用于对Mono对象提供常用的时间类(无缩放时间)方法
    /// </summary>
    public static class CoroutineUtility
    {
        /// <summary>
        /// 延迟调用
        /// </summary>
        /// <param name="duetime">延迟秒</param>
        public static Coroutine TimeDelay(this MonoBehaviour mono, float duetime, Action action)
        {
            if (!mono.gameObject.activeInHierarchy) return null;

            return mono.StartCoroutine(Funcn(duetime, action));
        }

        /// <summary>
        /// 时间更新到从 [0 到 time]
        /// </summary>
        /// <param name="time">时间长度</param>
        /// <param name="actionStar">开始Action</param>
        /// <param name="action">每Time帧Action;传入 [0,time]</param>
        /// <param name="actionEnd">Time 结束Action</param>
        public static Coroutine TimeProcess(this MonoBehaviour mono, float time, Action actionStar, Action<float> action, Action actionEnd = null)
        {
            if (!mono.gameObject.activeInHierarchy) return null;
            return mono.StartCoroutine(Funcn(time, actionStar, action, actionEnd));
        }

        /// <summary>
        /// 时间更新到从 [0 到 time]
        /// </summary>
        /// <param name="time">时间长度</param>
        /// <param name="actionStar">开始Action</param>
        /// <param name="action">每Time帧Action;传入 [0,1]</param>
        /// <param name="actionEnd">Time 结束Action</param>
        public static Coroutine TimeProcessOne(this MonoBehaviour mono, float time, Action actionStar, Action<float> action, Action actionEnd = null)
        {
            if (!mono.gameObject.activeInHierarchy) return null;
            return mono.StartCoroutine(Funcno(time, actionStar, action, actionEnd));
        }

        /// <summary>
        /// 以指定的时间长度，重复执行几个周期
        /// </summary>
        /// <param name="setpTime">周期时长</param>
        /// <param name="count">周期次数</param>
        /// <param name="actionStar">开始 Action</param>
        /// <param name="action">每次周期的开始 Action;传入 周期次数</param>
        /// <param name="actionEnd">结束 Action</param>
        /// <returns></returns>
        public static Coroutine TimeStep(this MonoBehaviour mono, float setpTime, int count, Action actionStar, Action<int> action, Action actionEnd = null)
        {
            if (!mono.gameObject.activeInHierarchy) return null;
            return mono.StartCoroutine(Funcn(setpTime, count, actionStar, action, actionEnd));
        }

        /// <summary>
        /// 自定义协程
        /// </summary>
        /// <param name="yield_">每帧的停顿形式</param>
        /// <param name="action">终止条件</param>
        /// <param name="wait">终止后的等待时间</param>
        /// <param name="end">结束 Action</param>
        /// <returns></returns>
        public static Coroutine TimeUpdate(this MonoBehaviour mono, YieldInstruction yield_, Func<bool> action, float wait = 0, Action end = null)
        {
            if (!mono.gameObject.activeInHierarchy) return null;
            return mono.StartCoroutine(FuncUpdate(yield_, action, wait, end));
        }

        internal static IEnumerator Funcn(float duetime, Action action)
        {
            float t = Time.realtimeSinceStartup;

            while (Time.realtimeSinceStartup - t <= duetime) yield return null;

            action?.Invoke();
        }

        internal static IEnumerator Funcn(float time, int step, Action actionStar, Action<int> action, Action actionEnd)
        {
            actionStar?.Invoke();

            float c = Time.realtimeSinceStartup;
            int n = 0;

            while (true)
            {
                int n_ = Mathf.FloorToInt((Time.realtimeSinceStartup - c) / time);

                if (n_ >= n)
                {

                    bool isBreak = false;
                    for (int i = n; i <= n_; i++)
                    {
                        action.Invoke(n);
                        if (n >= step)
                        {
                            isBreak = true;
                            break;
                        }

                        n++;
                    }
                    if (isBreak) break;
                }

                yield return null;
            }

            actionEnd?.Invoke();
        }

        internal static IEnumerator Funcn(float time, Action actionStar, Action<float> action, Action actionEnd)
        {
            actionStar?.Invoke();

            float c = Time.realtimeSinceStartup;

            while (true)
            {
                float p = Mathf.Min(time, Time.realtimeSinceStartup - c);

                action.Invoke(p);

                if (p == time) break;

                yield return null;
            }

            actionEnd?.Invoke();
        }

        internal static IEnumerator Funcno(float time, Action actionStar, Action<float> action, Action actionEnd)
        {
            actionStar?.Invoke();

            float c = Time.realtimeSinceStartup;

            while (true)
            {
                float p = Mathf.Min(1, (Time.realtimeSinceStartup - c) / time);

                action.Invoke(p);

                if (p == 1) break;

                yield return null;
            }

            actionEnd?.Invoke();
        }

        internal static IEnumerator FuncUpdate(YieldInstruction yield_, Func<bool> action, float wait, Action end)
        {

            while (true)
            {
                if (!action.Invoke()) break;

                yield return yield_;
            }
            if (wait > 0) yield return new WaitForSeconds(wait);

            end?.Invoke();
        }
    }

}