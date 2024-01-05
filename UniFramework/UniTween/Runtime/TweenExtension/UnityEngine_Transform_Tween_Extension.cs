using UniFramework.Tween;

namespace UnityEngine
{
    public static class UnityEngine_Transform_Tween_Extension
    {
        public static Vector3Tween TweenScale(this Transform obj, float duration, Vector3 from, Vector3 to)
        {
            Vector3Tween node = Vector3Tween.Allocate(duration, from, to);
            node.SetOnUpdate((result) => { obj.localScale = result; });
            return node;
        }
        public static Vector3Tween TweenScaleTo(this Transform obj, float duration, Vector3 to, bool setRuntimeValue = true)
        {
            Vector3Tween node = TweenScale(obj, duration, obj.localScale, to);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    node.SetValueFrom(obj.localScale);
                });
            }
            return node;
        }
        public static Vector3Tween TweenScaleFrom(this Transform obj, float duration, Vector3 from, bool setRuntimeValue = true)
        {
            Vector3Tween node = TweenScale(obj, duration, from, obj.localScale);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    node.SetValueTo(obj.localScale);
                });
            }
            return node;
        }

        public static Vector3Tween ShakePosition(this Transform obj, float duration, Vector3 magnitude, bool relativeWorld = false)
        {
            Vector3 position = relativeWorld ? obj.position : obj.localPosition;
            Vector3Tween node = Vector3Tween.Allocate(duration, position, position);
            node.SetOnUpdate(
                (result) =>
                {
                    if (relativeWorld)
                        obj.position = result;
                    else
                        obj.localPosition = result;
                });
            node.SetLerp(
                (from, to, progress) =>
                {
                    return TweenMath.Shake(magnitude, from, progress);
                });
            node.SetLoop(ETweenLoop.PingPong, 1);
            return node;
        }
        public static Vector3Tween TweenPosition(this Transform obj, float duration, Vector3 from, Vector3 to, bool relativeWorld = false)
        {
            Vector3Tween node = Vector3Tween.Allocate(duration, from, to);
            node.SetOnUpdate(
                (result) =>
                {
                    if (relativeWorld)
                        obj.position = result;
                    else
                        obj.localPosition = result;
                });
            return node;
        }
        public static Vector3Tween TweenPositionTo(this Transform obj, float duration, Vector3 to, bool relativeWorld = false, bool setRuntimeValue = true)
        {
            Vector3 from = relativeWorld ? obj.position : obj.localPosition;
            Vector3Tween node = TweenPosition(obj, duration, from, to, relativeWorld);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    Vector3 runtimeValue = relativeWorld ? obj.position : obj.localPosition;
                    node.SetValueFrom(runtimeValue);
                });
            }
            return node;
        }
        public static Vector3Tween TweenPositionFrom(this Transform obj, float duration, Vector3 from, bool relativeWorld = false, bool setRuntimeValue = true)
        {
            Vector3 to = relativeWorld ? obj.position : obj.localPosition;
            Vector3Tween node = TweenPosition(obj, duration, from, to, relativeWorld);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    Vector3 runtimeValue = relativeWorld ? obj.position : obj.localPosition;
                    node.SetValueTo(runtimeValue);
                });
            }
            return node;
        }
        public static Vector3Tween TweenMove(this Transform obj, float duration, Vector3 dest, bool relativeWorld = false)
        {
            return TweenPositionTo(obj, duration, dest, relativeWorld);
        }

        public static Vector3Tween TweenAngles(this Transform obj, float duration, Vector3 from, Vector3 to, bool relativeWorld = false)
        {
            Vector3Tween node = Vector3Tween.Allocate(duration, from, to);
            node.SetOnUpdate(
                (result) =>
                {
                    if (relativeWorld)
                        obj.eulerAngles = result;
                    else
                        obj.localEulerAngles = result;
                });
            return node;
        }
        public static Vector3Tween TweenAnglesTo(this Transform obj, float duration, Vector3 to, bool relativeWorld = false, bool setRuntimeValue = true)
        {
            Vector3 from = relativeWorld ? obj.eulerAngles : obj.localEulerAngles;
            Vector3Tween node = TweenAngles(obj, duration, from, to, relativeWorld);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    Vector3 runtimeValue = relativeWorld ? obj.eulerAngles : obj.localEulerAngles;
                    node.SetValueFrom(runtimeValue);
                });
            }
            return node;
        }
        public static Vector3Tween TweenAnglesFrom(this Transform obj, float duration, Vector3 from, bool relativeWorld = false, bool setRuntimeValue = true)
        {
            Vector3 to = relativeWorld ? obj.eulerAngles : obj.localEulerAngles;
            Vector3Tween node = TweenAngles(obj, duration, from, to, relativeWorld);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    Vector3 runtimeValue = relativeWorld ? obj.eulerAngles : obj.localEulerAngles;
                    node.SetValueTo(runtimeValue);
                });
            }
            return node;
        }

        public static QuaternionTween TweenRotation(this Transform obj, float duration, Quaternion from, Quaternion to, bool relativeWorld = false)
        {
            QuaternionTween node = QuaternionTween.Allocate(duration, from, to);
            node.SetOnUpdate(
                (result) =>
                {
                    if (relativeWorld)
                        obj.rotation = result;
                    else
                        obj.localRotation = result;
                });
            return node;
        }
        public static QuaternionTween TweenRotationTo(this Transform obj, float duration, Quaternion to, bool relativeWorld = false, bool setRuntimeValue = true)
        {
            Quaternion from = relativeWorld ? obj.rotation : obj.localRotation;
            QuaternionTween node = TweenRotation(obj, duration, from, to, relativeWorld);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    Quaternion runtimeValue = relativeWorld ? obj.rotation : obj.localRotation;
                    node.SetValueFrom(runtimeValue);
                });
            }
            return node;
        }
        public static QuaternionTween TweenRotationFrom(this Transform obj, float duration, Quaternion from, bool relativeWorld = false, bool setRuntimeValue = true)
        {
            Quaternion to = relativeWorld ? obj.rotation : obj.localRotation;
            QuaternionTween node = TweenRotation(obj, duration, from, to, relativeWorld);
            if (setRuntimeValue)
            {
                node.SetOnBegin(() =>
                {
                    Quaternion runtimeValue = relativeWorld ? obj.rotation : obj.localRotation;
                    node.SetValueTo(runtimeValue);
                });
            }
            return node;
        }
    }
}