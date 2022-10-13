using UnityEngine;

namespace UniFramework.Tween
{
	internal class UniTweenDriver : MonoBehaviour
	{
		void Update()
		{
			UniTween.Update();
		}

		void OnDestroy()
		{
			UniTween.Destroy();
		}
	}
}