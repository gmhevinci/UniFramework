using System;

namespace UniFramework
{
    public class LocalizedBehaviourAttribute : Attribute
    {
        public Type TranslationType { private set; get; }

        public LocalizedBehaviourAttribute(Type translationType)
        {
            TranslationType = translationType;
        }
    }
}