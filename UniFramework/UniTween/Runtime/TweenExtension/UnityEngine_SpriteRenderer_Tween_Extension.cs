using UniFramework.Tween;

namespace UnityEngine
{
    public static class UnityEngine_SpriteRenderer_Tween_Extension
    {
        public static ColorTween TweenColor(this SpriteRenderer obj, float duration, Color from, Color to)
        {
            ColorTween node = ColorTween.Allocate(duration, from, to);
            node.SetOnUpdate((result) => { obj.color = result; });
            return node;
        }
        public static ColorTween TweenColorTo(this SpriteRenderer obj, float duration, Color to, bool setRuntimeValue = true)
        {
            ColorTween node = TweenColor(obj, duration, obj.color, to);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    node.SetValueFrom(obj.color);
                });
            }
            return node;
        }
        public static ColorTween TweenColorFrom(this SpriteRenderer obj, float duration, Color from, bool setRuntimeValue = true)
        {
            ColorTween node = TweenColor(obj, duration, from, obj.color);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    node.SetValueTo(obj.color);
                });
            }
            return node;
        }
    }
}
