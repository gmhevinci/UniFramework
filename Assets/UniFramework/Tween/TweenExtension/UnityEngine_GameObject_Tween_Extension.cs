using UniFramework.Tween;

namespace UnityEngine
{
    public static class UnityEngine_GameObject_Tween_Extension
    {
        public static long PlayTween(this GameObject go, ITweenNode tween)
        {
            return UniTween.Play(tween, go);
        }
        public static long PlayTween(this GameObject go, ITweenChain tween)
        {
            return UniTween.Play(tween, go);
        }
        public static long PlayTween(this GameObject go, ChainNode tween)
		{
            return UniTween.Play(tween, go);
        }
    }
}
