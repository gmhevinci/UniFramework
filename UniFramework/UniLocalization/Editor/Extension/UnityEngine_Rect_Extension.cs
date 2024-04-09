using System;
using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine
{
    public static class UnityEngine_Rect_Extension 
    {
        public static void MoveToNextLine(this ref Rect rect)
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}