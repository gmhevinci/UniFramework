using UniFramework.Tween;

namespace UnityEngine
{
    public static class UnityEngine_CanvasGroup_Tween_Extension
    {
        public static FloatTween TweenAlpha(this CanvasGroup obj, float duration, float from, float to)
        {
            FloatTween node = FloatTween.Allocate(duration, from, to);
            node.SetOnUpdate((result) => { obj.alpha = result; });
            return node;
        }
        public static FloatTween TweenAlphaTo(this CanvasGroup obj, float duration, float to, bool setRuntimeValue = true)
        {
            FloatTween node = TweenAlpha(obj, duration, obj.alpha, to);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    node.SetValueFrom(obj.alpha);
                });
            }
            return node;
        }
        public static FloatTween TweenAlphaFrom(this CanvasGroup obj, float duration, float from, bool setRuntimeValue = true)
        {
            FloatTween node = TweenAlpha(obj, duration, from, obj.alpha);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    node.SetValueTo(obj.alpha);
                });
            }
            return node;
        }
    }
}
