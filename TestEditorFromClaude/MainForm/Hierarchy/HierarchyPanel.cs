using App.Theme;

namespace App.MainForm.Hierarchy
{
    public class HierarchyPanel : UserControl
    {
        private TreeView hierarchyTree;
        private TextBox searchBox;
        private ToolStrip hierarchyToolStrip;

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<HierarchyActionEventArgs> ActionRequested;

        private readonly Dictionary<object, TreeNode> objectToNodeMap;
        private readonly List<object> selectedObjects;

        public HierarchyPanel()
        {
            objectToNodeMap = new Dictionary<object, TreeNode>();
            selectedObjects = new List<object>();

            InitializeComponent();
            SetupLayout();
            WireEvents();
        }

        private void InitializeComponent()
        {
            // Panel setup
            BackColor = ColorScheme.PanelBackground;
            Dock = DockStyle.Fill;

            CreateSearchBox();
            CreateToolStrip();
            CreateTreeView();
        }

        private void CreateSearchBox()
        {
            searchBox = new TextBox
            {
                Dock = DockStyle.Top,
                BackColor = ColorScheme.InputBackground,
                ForeColor = ColorScheme.InputForeground,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                Text = "Search objects..."
            };
        }

        private void CreateToolStrip()
        {
            hierarchyToolStrip = new ToolStrip
            {
                Dock = DockStyle.Top,
                BackColor = ColorScheme.ToolbarBackground,
                GripStyle = ToolStripGripStyle.Hidden,
                ImageScalingSize = new Size(16, 16)
            };

            // Add hierarchy-specific tools
            hierarchyToolStrip.Items.AddRange(new ToolStripItem[]
            {
                CreateToolButton("Expand All", "⊞", HierarchyAction.ExpandAll),
                CreateToolButton("Collapse All", "⊟", HierarchyAction.CollapseAll),
                new ToolStripSeparator(),
                CreateToolButton("Create Group", "📁", HierarchyAction.CreateGroup),
                CreateToolButton("Delete", "🗑", HierarchyAction.Delete)
            });
        }

        private ToolStripButton CreateToolButton(string tooltip, string emoji, HierarchyAction action)
        {
            var button = new ToolStripButton(emoji)
            {
                ToolTipText = tooltip,
                Tag = action,
                BackColor = ColorScheme.ToolbarBackground,
                ForeColor = ColorScheme.ToolbarForeground
            };

            button.Click += OnToolButtonClick;
            return button;
        }

        private void CreateTreeView()
        {
            hierarchyTree = new TreeView
            {
                Dock = DockStyle.Fill,
                BackColor = ColorScheme.TreeBackground,
                ForeColor = ColorScheme.TreeForeground,
                BorderStyle = BorderStyle.None,
                HideSelection = false,
                CheckBoxes = true,
                ShowLines = true,
                ShowPlusMinus = true,
                ShowRootLines = true,
                AllowDrop = true,
                LabelEdit = true,
                Font = new Font("Segoe UI", 9F)
            };

            // Initialize with scene root
            InitializeSceneHierarchy();
        }

        private void InitializeSceneHierarchy()
        {
            // Create root scene node
            var sceneNode = new TreeNode("🌐 Scene")
            {
                Tag = new SceneRootObject(),
                Checked = true
            };

            // Create category nodes
            var meshesNode = new TreeNode("🔺 Meshes");
            var materialsNode = new TreeNode("🎨 Materials");
            var lightsNode = new TreeNode("💡 Lights");
            var camerasNode = new TreeNode("📷 Cameras");

            sceneNode.Nodes.AddRange(new[] { meshesNode, materialsNode, lightsNode, camerasNode });
            hierarchyTree.Nodes.Add(sceneNode);
            sceneNode.Expand();
        }

        private void SetupLayout()
        {
            // Create title label
            var titleLabel = new Label
            {
                Text = "Hierarchy",
                Dock = DockStyle.Top,
                Height = 25,
                BackColor = ColorScheme.TitleBackground,
                ForeColor = ColorScheme.TitleForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            // Add controls in correct order
            Controls.AddRange(new Control[] {
                hierarchyTree,
                hierarchyToolStrip,
                searchBox,
                titleLabel
            });
        }

        private void WireEvents()
        {
            hierarchyTree.AfterSelect += OnTreeAfterSelect;
            hierarchyTree.AfterCheck += OnTreeAfterCheck;
            hierarchyTree.NodeMouseClick += OnTreeNodeMouseClick;
            hierarchyTree.DragEnter += OnTreeDragEnter;
            hierarchyTree.DragDrop += OnTreeDragDrop;
            hierarchyTree.AfterLabelEdit += OnTreeAfterLabelEdit;

            searchBox.TextChanged += OnSearchTextChanged;
            searchBox.Enter += OnSearchBoxEnter;
            searchBox.Leave += OnSearchBoxLeave;
        }

        #region Event Handlers

        private void OnTreeAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag != null)
            {
                selectedObjects.Clear();
                selectedObjects.Add(e.Node.Tag);

                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(selectedObjects));
            }
        }

        private void OnTreeAfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag != null)
            {
                // TODO: Update object visibility
                // ((ISceneObject)e.Node.Tag).IsVisible = e.Node.Checked;

                // Recursively update child nodes
                UpdateChildNodesCheckState(e.Node, e.Node.Checked);
            }
        }

        private void OnTreeNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                hierarchyTree.SelectedNode = e.Node;
                ShowContextMenu(e.Location);
            }
        }

        private void OnTreeDragEnter(object sender, DragEventArgs e)
        {
            // TODO: Implement drag and drop validation
            e.Effect = DragDropEffects.Move;
        }

        private void OnTreeDragDrop(object sender, DragEventArgs e)
        {
            // TODO: Implement hierarchy reordering
        }

        private void OnTreeAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            // TODO: Update object name
            // ((ISceneObject)e.Node.Tag).Name = e.Label;
        }

        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text != "Search objects...")
            {
                FilterNodes(searchBox.Text);
            }
        }

        private void OnSearchBoxEnter(object sender, EventArgs e)
        {
            if (searchBox.Text == "Search objects...")
            {
                searchBox.Text = "";
                searchBox.ForeColor = ColorScheme.InputForeground;
            }
        }

        private void OnSearchBoxLeave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchBox.Text))
            {
                searchBox.Text = "Search objects...";
                searchBox.ForeColor = ColorScheme.InputPlaceholder;
            }
        }

        private void OnToolButtonClick(object sender, EventArgs e)
        {
            if (sender is ToolStripButton button && button.Tag is HierarchyAction action)
            {
                HandleHierarchyAction(action);
            }
        }

        #endregion

        #region Public Methods

        public void AddObject(object obj, string displayName, string emoji = "📦")
        {
            // TODO: Determine appropriate parent node based on object type
            var parentNode = FindAppropriateParent(obj);

            var objectNode = new TreeNode($"{emoji} {displayName}")
            {
                Tag = obj,
                Checked = true // TODO: Get from object.IsVisible
            };

            parentNode.Nodes.Add(objectNode);
            objectToNodeMap[obj] = objectNode;

            parentNode.Expand();
        }

        public void RemoveObject(object obj)
        {
            if (objectToNodeMap.TryGetValue(obj, out var node))
            {
                node.Remove();
                objectToNodeMap.Remove(obj);
            }
        }

        public void SelectObject(object obj)
        {
            if (objectToNodeMap.TryGetValue(obj, out var node))
            {
                hierarchyTree.SelectedNode = node;
                node.EnsureVisible();
            }
        }

        public void UpdateObjectName(object obj, string newName)
        {
            if (objectToNodeMap.TryGetValue(obj, out var node))
            {
                // Preserve emoji prefix
                var emoji = node.Text.Split(' ')[0];
                node.Text = $"{emoji} {newName}";
            }
        }

        public void SetObjectVisibility(object obj, bool isVisible)
        {
            if (objectToNodeMap.TryGetValue(obj, out var node))
            {
                node.Checked = isVisible;
            }
        }

        #endregion

        #region Private Methods

        private TreeNode FindAppropriateParent(object obj)
        {
            // TODO: Implement logic to determine parent based on object type
            // For now, return the first category node (Meshes)
            return hierarchyTree.Nodes[0].Nodes[0]; // Scene -> Meshes
        }

        private void UpdateChildNodesCheckState(TreeNode parentNode, bool isChecked)
        {
            foreach (TreeNode childNode in parentNode.Nodes)
            {
                childNode.Checked = isChecked;
                UpdateChildNodesCheckState(childNode, isChecked);
            }
        }

        private void FilterNodes(string searchText)
        {
            // TODO: Implement search filtering
            // Show/hide nodes based on search text
        }

        private void ShowContextMenu(Point location)
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.BackColor = ColorScheme.MenuBackground;
            contextMenu.ForeColor = ColorScheme.MenuForeground;

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Rename", null, (s, e) => StartRename()),
                new ToolStripMenuItem("Duplicate", null, (s, e) => DuplicateSelected()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Create Group", null, (s, e) => CreateGroup()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Delete", null, (s, e) => DeleteSelected())
            });

            contextMenu.Show(hierarchyTree, location);
        }

        private void HandleHierarchyAction(HierarchyAction action)
        {
            switch (action)
            {
                case HierarchyAction.ExpandAll:
                    hierarchyTree.ExpandAll();
                    break;
                case HierarchyAction.CollapseAll:
                    hierarchyTree.CollapseAll();
                    break;
                case HierarchyAction.CreateGroup:
                    CreateGroup();
                    break;
                case HierarchyAction.Delete:
                    DeleteSelected();
                    break;
            }
        }

        private void StartRename()
        {
            if (hierarchyTree.SelectedNode != null)
            {
                hierarchyTree.SelectedNode.BeginEdit();
            }
        }

        private void DuplicateSelected()
        {
            if (hierarchyTree.SelectedNode?.Tag != null)
            {
                ActionRequested?.Invoke(this, new HierarchyActionEventArgs(
                    HierarchyAction.Duplicate, hierarchyTree.SelectedNode.Tag));
            }
        }

        private void CreateGroup()
        {
            ActionRequested?.Invoke(this, new HierarchyActionEventArgs(
                HierarchyAction.CreateGroup, null));
        }

        private void DeleteSelected()
        {
            if (hierarchyTree.SelectedNode?.Tag != null)
            {
                ActionRequested?.Invoke(this, new HierarchyActionEventArgs(
                    HierarchyAction.Delete, hierarchyTree.SelectedNode.Tag));
            }
        }

        #endregion
    }

    // Helper classes and enums
    public enum HierarchyAction
    {
        ExpandAll, CollapseAll, CreateGroup, Delete, Duplicate, Rename
    }

    public class SceneRootObject
    {
        public string Name { get; set; } = "Scene";
        public bool IsVisible { get; set; } = true;
    }

    public class SelectionChangedEventArgs : EventArgs
    {
        public List<object> SelectedObjects { get; }

        public SelectionChangedEventArgs(List<object> selectedObjects)
        {
            SelectedObjects = selectedObjects;
        }
    }

    public class HierarchyActionEventArgs : EventArgs
    {
        public HierarchyAction Action { get; }
        public object Target { get; }

        public HierarchyActionEventArgs(HierarchyAction action, object target)
        {
            Action = action;
            Target = target;
        }
    }
}
