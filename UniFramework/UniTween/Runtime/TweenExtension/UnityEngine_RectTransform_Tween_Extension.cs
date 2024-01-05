using UniFramework.Tween;

namespace UnityEngine
{
    public static class UnityEngine_RectTransform_Tween_Extension
    {
        public static Vector2Tween TweenAnchoredPosition(this RectTransform obj, float duration, Vector2 from, Vector2 to)
        {
            Vector2Tween node = Vector2Tween.Allocate(duration, from, to);
            node.SetOnUpdate(
                (result) => 
                {
                    obj.anchoredPosition = result;
                });
            return node;
        }
        public static Vector2Tween TweenAnchoredPositionTo(this RectTransform obj, float duration, Vector2 to, bool setRuntimeValue = true)
        {
            Vector2Tween node = TweenAnchoredPosition(obj, duration, obj.anchoredPosition, to);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    node.SetValueFrom(obj.anchoredPosition);
                });
            }
            return node;
        }
        public static Vector2Tween TweenAnchoredPositionFrom(this RectTransform obj, float duration, Vector2 from, bool setRuntimeValue = true)
        {
            Vector2Tween node = TweenAnchoredPosition(obj, duration, from, obj.anchoredPosition);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    node.SetValueTo(obj.anchoredPosition);
                });
            }
            return node;
        }
    }
}