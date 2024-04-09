using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniFramework.Localization.Editor
{
    internal class TranslationKeyTreeViewItem : TreeViewItem
    {
        public string Key { get; set; }
        
        public TranslationKeyTreeViewItem(string key, int id, int depth) :
            base(id, depth)
        {
            Key = key;
        }
    }
}