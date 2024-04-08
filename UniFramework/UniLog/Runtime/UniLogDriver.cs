using UnityEngine;

namespace UniFramework.Log {

    /// <summary>
    /// 当需要手动刷新时可创建此脚本
    /// </summary>
    public class UniLogDriver : MonoBehaviour
    {
        public static void Initalize() {

            UniLog.Instance.IsManualFlush = true;

            if (UniLog.Instance.driver == null) 
            {
                GameObject obj = new UnityEngine.GameObject($"[{nameof(UniLog)}]");
                obj.AddComponent<UniLogDriver>();
                UnityEngine.Object.DontDestroyOnLoad(obj);
                UniLog.Instance.driver = obj;
            }
        }

        private void FixedUpdate()
        {
            UniLog.Instance.ManualFlush();
        }
    }
}