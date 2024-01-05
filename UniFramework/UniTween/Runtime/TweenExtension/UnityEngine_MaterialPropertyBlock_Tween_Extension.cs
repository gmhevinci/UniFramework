#if UNITY_2021_3_OR_NEWER
using UniFramework.Tween;

namespace UnityEngine
{
    public static class UnityEngine_MaterialPropertyBlock_Tween_Extension
    {
        public static ColorTween TweenColorValue(this MaterialPropertyBlock obj, string property, float duration, Color from, Color to)
        {
            if (obj.HasProperty(property) == false)
            {
                Debug.LogWarning($"Not found material property : {property}");
                return ColorTween.Allocate(duration, from, to);
            }

            int propertyID = Shader.PropertyToID(property);
            ColorTween node = ColorTween.Allocate(duration, from, to);
            node.SetOnUpdate((result) => { obj.SetColor(propertyID, result); });
            return node;
        }	
        public static FloatTween TweenFloatValue(this MaterialPropertyBlock obj, string property, float duration, float from, float to)
        {
            if (obj.HasProperty(property) == false)
            {
                Debug.LogWarning($"Not found material property : {property}");
                return FloatTween.Allocate(duration, from, to);
            }

            int propertyID = Shader.PropertyToID(property);
            FloatTween node = FloatTween.Allocate(duration, from, to);
            node.SetOnUpdate((result) => { obj.SetFloat(propertyID, result); });
            return node;
        }
    }
}
#endif