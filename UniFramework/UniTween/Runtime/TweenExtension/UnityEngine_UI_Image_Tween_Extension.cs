using UniFramework.Tween;

namespace UnityEngine.UI
{
    public static class UnityEngine_UI_Image_Tween_Extension
    {
        public static ColorTween TweenColor(this Image obj, float duration, Color from, Color to)
        {
            ColorTween node = ColorTween.Allocate(duration, from, to);
            node.SetOnUpdate((result) => { obj.color = result; });
            return node;
        }
        public static ColorTween TweenColorTo(this Image obj, float duration, Color to, bool setRuntimeValue = true)
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
        public static ColorTween TweenColorFrom(this Image obj, float duration, Color from, bool setRuntimeValue = true)
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
