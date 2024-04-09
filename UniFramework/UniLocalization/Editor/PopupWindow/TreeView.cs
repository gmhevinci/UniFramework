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
        protected Action<string> _selectionHandler;

        public TreeViewItem Root { get; private set; }

        public TranslationKeyTreeView()
            : base(new TreeViewState())
        {
            this.showAlternatingRowBackgrounds = true;
            this.showBorder = true;
        }

        public TranslationKeyTreeView(Action<string> selectionHandler)
            : this()
        {
            _selectionHandler = selectionHandler;
            this.showAlternatingRowBackgrounds = true;
            this.showBorder = true;
            Reload();
        }

        protected override bool CanMultiSelect(TreeViewItem item) => false;

        protected override TreeViewItem BuildRoot()
        {
            Root = new TreeViewItem(-1, -1);
            var id = 1;

            Root.AddChild(new TranslationKeyTreeViewItem("None", id++, 0) { displayName = $"None" });
            Root.AddChild(new TranslationKeyTreeViewItem("None2", id++, 0) { displayName = $"None2" });

            if (!Root.hasChildren)
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