using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UniFramework.Localization.Editor
{
    internal class TranslationKeyTreeView : TreeView
    {
        protected string _tableName;
        protected Action<string> _selectionHandler;

        public TreeViewItem Root { get; private set; }

        public TranslationKeyTreeView()
            : base(new TreeViewState())
        {
            this.showAlternatingRowBackgrounds = true;
            this.showBorder = true;
        }

        public TranslationKeyTreeView(string tableName, Action<string> selectionHandler)
            : this()
        {
            _tableName = tableName;
            _selectionHandler = selectionHandler;
            this.showAlternatingRowBackgrounds = true;
            this.showBorder = true;
            Reload();
        }

        protected override bool CanMultiSelect(TreeViewItem item) => false;

        protected override TreeViewItem BuildRoot()
        {
            Root = new TreeViewItem(-1, -1);

            var readerInstance = LocalizationSettingData.GetLocalizeReader(_tableName);
            if (readerInstance != null)
            {
                int itemID = 1;
                foreach (var key in readerInstance.Keys)
                {
                    Root.AddChild(new TranslationKeyTreeViewItem(key, itemID++, 0) { displayName = key });
                }
            }

            if (Root.hasChildren == false)
            {
                Root.AddChild(new TreeViewItem(1, 0, "No Tables Found."));
            }

            SetupDepthsFromParentsAndChildren(Root);
            return Root;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count == 0)
                return;

            var selected = FindItem(selectedIds[0], rootItem);
            if (selected is TranslationKeyTreeViewItem keyNode)
            {
                _selectionHandler(keyNode.Key);
                return;
            }

            // Toggle the foldout
            if (selected.hasChildren)
            {
                SetExpanded(selected.id, !IsExpanded(selected.id));
            }

            // Ignore Table selections. We just care about table entries.
            SetSelection(new int[] { });
        }
    }
}