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
        public static ColorTween TweenColorTo(this SpriteRenderer obj, float duration, Color to)
        {
            return TweenColor(obj, duration, obj.color, to);
        }
        public static ColorTween TweenColorFrom(this SpriteRenderer obj, float duration, Color from)
        {
            return TweenColor(obj, duration, from, obj.color);
        }
    }
}
