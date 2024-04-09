using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniFramework.Localization.Editor
{
    class TreeViewPopupWindow : PopupWindowContent
    {
        readonly SearchField _searchField;
        readonly TreeView _treeView;
        private bool _shouldClose;

        public float Width { get; set; }

        public TreeViewPopupWindow(TreeView contents)
        {
            _searchField = new SearchField();
            _treeView = contents;
        }

        public override void OnGUI(Rect rect)
        {
            // Escape closes the window
            if (_shouldClose || UnityEngine.Event.current.type == EventType.KeyDown && UnityEngine.Event.current.keyCode == KeyCode.Escape)
            {
                GUIUtility.hotControl = 0;
                this.editorWindow.Close();
                GUIUtility.ExitGUI();
            }

            const int border = 4;
            const int topPadding = 12;
            const int searchHeight = 20;
            const int remainTop = topPadding + searchHeight + border;
            var searchRect = new Rect(border, topPadding, rect.width - border * 2, searchHeight);
            var remainingRect = new Rect(border, topPadding + searchHeight + border, rect.width - border * 2, rect.height - remainTop - border);

            _treeView.searchString = _searchField.OnGUI(searchRect, _treeView.searchString);
            _treeView.OnGUI(remainingRect);

            if (_treeView.HasSelection())
                ForceClose();
        }

        public override Vector2 GetWindowSize()
        {
            var result = base.GetWindowSize();
            result.x = Width;
            return result;
        }

        public override void OnOpen()
        {
            _searchField.SetFocus();
            base.OnOpen();
        }

        public void ForceClose() => _shouldClose = true;
    }
}