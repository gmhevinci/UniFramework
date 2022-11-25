using UnityEngine;

namespace UniFramework.Manager
{
	internal class UniManagerDriver : MonoBehaviour
	{
		void Update()
		{
			UniManager.Update();
		}

		void OnDestroy()
		{
			UniManager.Destroy();
		}
	}
}