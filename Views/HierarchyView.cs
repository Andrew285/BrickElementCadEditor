using App.Core.Events;
using App.Core.Interfaces;
using App.Core.Models;
using App.Theme;

namespace App.Views
{
    /// <summary>
    /// Implementation of the hierarchy panel view.
    /// Displays the scene object hierarchy in a tree view.
    /// </summary>
    public class HierarchyView : UserControl, IHierarchyView
    {
        private readonly TreeView _treeView;
        private readonly ContextMenuStrip _contextMenu;
        private readonly Dictionary<Guid, TreeNode> _objectNodes;

        public event EventHandler ViewLoaded;
        public event EventHandler<ObjectSelectedEventArgs> ObjectSelected;
        public event EventHandler<ObjectRenamedEventArgs> ObjectRenamed;
        public event EventHandler<ObjectVisibilityEventArgs> ObjectVisibilityChanged;
        public event EventHandler<ObjectLockStateEventArgs> ObjectLockStateChanged;
        public event EventHandler<ObjectParentChangedEventArgs> ObjectParentChanged;
        public event EventHandler<ObjectEventArgs> DeleteRequested;
        public event EventHandler<ObjectEventArgs> DuplicateRequested;

        public HierarchyView()
        {
            _objectNodes = new Dictionary<Guid, TreeNode>();

            // Create and configure the TreeView
            _treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                HideSelection = false,
                LabelEdit = true,
                ShowLines = true,
                ShowPlusMinus = true,
                BackColor = ColorScheme.PanelBackground,
                ForeColor = ColorScheme.PanelForeground,
                Font = new Font("Segoe UI", 9F)
            };

            // Create context menu
            _contextMenu = CreateContextMenu();
            _treeView.ContextMenuStrip = _contextMenu;

            // Wire up TreeView events
            _treeView.AfterSelect += OnTreeViewAfterSelect;
            _treeView.AfterLabelEdit += OnTreeViewAfterLabelEdit;
            _treeView.ItemDrag += OnTreeViewItemDrag;
            _treeView.DragEnter += OnTreeViewDragEnter;
            _treeView.DragOver += OnTreeViewDragOver;
            _treeView.DragDrop += OnTreeViewDragDrop;

            // Add TreeView to the control
            Controls.Add(_treeView);

            // Enable drag and drop
            _treeView.AllowDrop = true;

            // Handle load event
            Load += (s, e) => ViewLoaded?.Invoke(this, EventArgs.Empty);
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var menu = new ContextMenuStrip
            {
                BackColor = ColorScheme.MenuBackground,
                ForeColor = ColorScheme.MenuForeground
            };

            menu.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Rename", null, (s, e) => BeginRenameSelected()),
                new ToolStripMenuItem("Delete", null, (s, e) => DeleteSelected()),
                new ToolStripMenuItem("Duplicate", null, (s, e) => DuplicateSelected()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Show/Hide", null, (s, e) => ToggleVisibility()),
                new ToolStripMenuItem("Lock/Unlock", null, (s, e) => ToggleLockState())
            });

            return menu;
        }

        public void Show()
        {
            Visible = true;
        }

        public void Close()
        {
            Visible = false;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowInfo(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void SetObjects(IEnumerable<SceneObject> objects)
        {
            _treeView.BeginUpdate();
            try
            {
                _treeView.Nodes.Clear();
                _objectNodes.Clear();

                foreach (var obj in objects)
                {
                    AddObjectRecursive(obj, null);
                }
            }
            finally
            {
                _treeView.EndUpdate();
            }
        }

        public void AddObject(SceneObject obj)
        {
            var parentNode = obj.Parent != null ? _objectNodes[obj.Parent.Id] : null;
            AddObjectRecursive(obj, parentNode);
        }

        public void RemoveObject(SceneObject obj)
        {
            if (_objectNodes.TryGetValue(obj.Id, out var node))
            {
                node.Remove();
                _objectNodes.Remove(obj.Id);
            }
        }

        public void UpdateObject(SceneObject obj)
        {
            if (_objectNodes.TryGetValue(obj.Id, out var node))
            {
                UpdateNodeText(node, obj);
            }
        }

        public void UpdateSelection(IEnumerable<SceneObject> selectedObjects)
        {
            _treeView.BeginUpdate();
            try
            {
                _treeView.SelectedNodes.Clear();
                foreach (var obj in selectedObjects)
                {
                    if (_objectNodes.TryGetValue(obj.Id, out var node))
                    {
                        _treeView.SelectedNodes.Add(node);
                    }
                }
            }
            finally
            {
                _treeView.EndUpdate();
            }
        }

        public void EnsureVisible(SceneObject obj)
        {
            if (_objectNodes.TryGetValue(obj.Id, out var node))
            {
                node.EnsureVisible();
            }
        }

        public void BeginRename(SceneObject obj)
        {
            if (_objectNodes.TryGetValue(obj.Id, out var node))
            {
                node.BeginEdit();
            }
        }

        private void AddObjectRecursive(SceneObject obj, TreeNode parentNode)
        {
            var node = new TreeNode { Tag = obj };
            UpdateNodeText(node, obj);

            if (parentNode != null)
                parentNode.Nodes.Add(node);
            else
                _treeView.Nodes.Add(node);

            _objectNodes[obj.Id] = node;

            foreach (var child in obj.Children)
            {
                AddObjectRecursive(child, node);
            }
        }

        private void UpdateNodeText(TreeNode node, SceneObject obj)
        {
            var visibilityIcon = obj.IsVisible ? "👁" : "🚫";
            var lockIcon = obj.IsLocked ? "🔒" : "";
            node.Text = $"{visibilityIcon} {lockIcon} {obj.Name}";
        }

        private void OnTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            var obj = e.Node?.Tag as SceneObject;
            if (obj != null)
            {
                ObjectSelected?.Invoke(this, new ObjectSelectedEventArgs(obj, SelectionSource.Hierarchy));
            }
        }

        private void OnTreeViewAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!e.CancelEdit && e.Label != null)
            {
                var obj = e.Node?.Tag as SceneObject;
                if (obj != null)
                {
                    ObjectRenamed?.Invoke(this, new ObjectRenamedEventArgs(obj, e.Label));
                }
            }
        }

        private void OnTreeViewItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeNode node)
            {
                _treeView.DoDragDrop(node, DragDropEffects.Move);
            }
        }

        private void OnTreeViewDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void OnTreeViewDragOver(object sender, DragEventArgs e)
        {
            var targetPoint = _treeView.PointToClient(new Point(e.X, e.Y));
            var targetNode = _treeView.GetNodeAt(targetPoint);

            if (targetNode != null)
            {
                _treeView.SelectedNode = targetNode;
            }
        }

        private void OnTreeViewDragDrop(object sender, DragEventArgs e)
        {
            var targetPoint = _treeView.PointToClient(new Point(e.X, e.Y));
            var targetNode = _treeView.GetNodeAt(targetPoint);
            var draggedNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;

            if (draggedNode != null && targetNode != null)
            {
                var sourceObj = draggedNode.Tag as SceneObject;
                var targetObj = targetNode.Tag as SceneObject;

                if (sourceObj != null && targetObj != null && !IsAncestor(sourceObj, targetObj))
                {
                    ObjectParentChanged?.Invoke(this, new ObjectParentChangedEventArgs(sourceObj, targetObj));
                }
            }
        }

        private bool IsAncestor(SceneObject potentialAncestor, SceneObject obj)
        {
            var current = obj;
            while (current != null)
            {
                if (current == potentialAncestor)
                    return true;
                current = current.Parent;
            }
            return false;
        }

        private void BeginRenameSelected()
        {
            if (_treeView.SelectedNode != null)
            {
                _treeView.SelectedNode.BeginEdit();
            }
        }

        private void DeleteSelected()
        {
            var obj = _treeView.SelectedNode?.Tag as SceneObject;
            if (obj != null)
            {
                DeleteRequested?.Invoke(this, new ObjectEventArgs(obj));
            }
        }

        private void DuplicateSelected()
        {
            var obj = _treeView.SelectedNode?.Tag as SceneObject;
            if (obj != null)
            {
                DuplicateRequested?.Invoke(this, new ObjectEventArgs(obj));
            }
        }

        private void ToggleVisibility()
        {
            var obj = _treeView.SelectedNode?.Tag as SceneObject;
            if (obj != null)
            {
                ObjectVisibilityChanged?.Invoke(this, new ObjectVisibilityEventArgs(obj, !obj.IsVisible));
            }
        }

        private void ToggleLockState()
        {
            var obj = _treeView.SelectedNode?.Tag as SceneObject;
            if (obj != null)
            {
                ObjectLockStateChanged?.Invoke(this, new ObjectLockStateEventArgs(obj, !obj.IsLocked));
            }
        }
    }
} 