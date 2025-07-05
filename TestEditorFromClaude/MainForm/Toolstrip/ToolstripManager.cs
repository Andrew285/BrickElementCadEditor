using App.Theme;

namespace App.MainForm.Toolstrip
{
    public class ToolStripManager
    {
        public ToolStrip ToolStrip { get; private set; }

        public event EventHandler<ToolEventArgs> ToolSelected;

        private ToolStripButton currentTool;

        public ToolStripManager()
        {
            CreateToolStrip();
            CreateTools();
            ApplyTheme();
        }

        private void CreateToolStrip()
        {
            ToolStrip = new ToolStrip
            {
                BackColor = ColorScheme.ToolbarBackground,
                ForeColor = ColorScheme.ToolbarForeground,
                GripStyle = ToolStripGripStyle.Hidden,
                ImageScalingSize = new Size(24, 24),
                Height = 50,
                Dock = DockStyle.Top
            };
        }

        private void CreateTools()
        {
            // File operations
            AddToolGroup(new[]
            {
                CreateToolButton("New", "📄", EditorTool.FileNew),
                CreateToolButton("Open", "📁", EditorTool.FileOpen),
                CreateToolButton("Save", "💾", EditorTool.FileSave)
            });

            AddSeparator();

            // Edit operations
            AddToolGroup(new[]
            {
                CreateToolButton("Undo", "↶", EditorTool.EditUndo),
                CreateToolButton("Redo", "↷", EditorTool.EditRedo)
            });

            AddSeparator();

            // Selection and transformation tools
            AddToolGroup(new[]
            {
                CreateToolButton("Select", "⬆", EditorTool.Select, true), // Default tool
                CreateToolButton("Move", "✋", EditorTool.Move),
                CreateToolButton("Rotate", "🔄", EditorTool.Rotate),
                CreateToolButton("Scale", "📏", EditorTool.Scale)
            });

            AddSeparator();

            // Creation tools
            AddToolGroup(new[]
            {
                CreateToolButton("Vertex", "●", EditorTool.CreateVertex),
                CreateToolButton("Edge", "—", EditorTool.CreateEdge),
                CreateToolButton("Face", "▲", EditorTool.CreateFace)
            });

            AddSeparator();

            // Processing tools
            AddToolGroup(new[]
            {
                CreateToolButton("Triangulate", "🔺", EditorTool.Triangulate),
                CreateToolButton("Analyze", "📊", EditorTool.Analyze)
            });

            AddSeparator();

            // View modes
            AddToolGroup(new[]
            {
                CreateToolButton("Wireframe", "⬜", EditorTool.ViewWireframe),
                CreateToolButton("Solid", "⬛", EditorTool.ViewSolid, true), // Default view
                CreateToolButton("Textured", "🎨", EditorTool.ViewTextured)
            });
        }

        private void AddToolGroup(ToolStripButton[] tools)
        {
            ToolStrip.Items.AddRange(tools);
        }

        private void AddSeparator()
        {
            ToolStrip.Items.Add(new ToolStripSeparator());
        }

        private ToolStripButton CreateToolButton(string text, string emoji, EditorTool tool, bool isDefault = false)
        {
            var button = new ToolStripButton($"{emoji}\n{text}")
            {
                //BackColor = ColorScheme.ToolbarBackground,
                ForeColor = ColorScheme.ToolbarForeground,
                CheckOnClick = IsToggleTool(tool),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Size = new Size(70, 45),
                Font = new Font("Segoe UI", 8F),
                Tag = tool
            };

            button.Click += OnToolButtonClick;

            if (isDefault)
            {
                button.Checked = true;
                currentTool = button;
            }

            return button;
        }

        private bool IsToggleTool(EditorTool tool)
        {
            // Define which tools are toggle tools (mutually exclusive)
            return tool == EditorTool.Select ||
                   tool == EditorTool.Move ||
                   tool == EditorTool.Rotate ||
                   tool == EditorTool.Scale ||
                   tool == EditorTool.CreateVertex ||
                   tool == EditorTool.CreateEdge ||
                   tool == EditorTool.CreateFace ||
                   tool == EditorTool.ViewWireframe ||
                   tool == EditorTool.ViewSolid ||
                   tool == EditorTool.ViewTextured;
        }

        private void OnToolButtonClick(object sender, EventArgs e)
        {
            if (sender is ToolStripButton button && button.Tag is EditorTool tool)
            {
                // Handle tool selection
                if (IsToggleTool(tool))
                {
                    SetActiveTool(button);
                }

                ToolSelected?.Invoke(this, new ToolEventArgs(tool));
            }
        }

        private void SetActiveTool(ToolStripButton newTool)
        {
            // Uncheck previous tool if it's in the same group
            if (currentTool != null && IsInSameGroup(currentTool, newTool))
            {
                currentTool.Checked = false;
            }

            // Set new tool as active
            newTool.Checked = true;
            currentTool = newTool;
        }

        private bool IsInSameGroup(ToolStripButton tool1, ToolStripButton tool2)
        {
            var group1 = GetToolGroup((EditorTool)tool1.Tag);
            var group2 = GetToolGroup((EditorTool)tool2.Tag);
            return group1 == group2;
        }

        private ToolGroup GetToolGroup(EditorTool tool)
        {
            return tool switch
            {
                EditorTool.Select or EditorTool.Move or EditorTool.Rotate or EditorTool.Scale => ToolGroup.Transform,
                EditorTool.CreateVertex or EditorTool.CreateEdge or EditorTool.CreateFace => ToolGroup.Creation,
                EditorTool.ViewWireframe or EditorTool.ViewSolid or EditorTool.ViewTextured => ToolGroup.ViewMode,
                _ => ToolGroup.None
            };
        }

        private void ApplyTheme()
        {
            //ToolStrip.Renderer = new DarkToolStripRenderer();
        }

        public void EnableTool(EditorTool tool, bool enabled)
        {
            foreach (ToolStripItem item in ToolStrip.Items)
            {
                if (item is ToolStripButton button && button.Tag.Equals(tool))
                {
                    button.Enabled = enabled;
                    break;
                }
            }
        }

        public void SetToolChecked(EditorTool tool, bool isChecked)
        {
            foreach (ToolStripItem item in ToolStrip.Items)
            {
                if (item is ToolStripButton button && button.Tag.Equals(tool))
                {
                    button.Checked = isChecked;
                    break;
                }
            }
        }
    }

    public enum EditorTool
    {
        // File operations
        FileNew, FileOpen, FileSave,

        // Edit operations
        EditUndo, EditRedo,

        // Transform tools
        Select, Move, Rotate, Scale,

        // Creation tools
        CreateVertex, CreateEdge, CreateFace,

        // Processing tools
        Triangulate, Analyze,

        // View modes
        ViewWireframe, ViewSolid, ViewTextured
    }

    public enum ToolGroup
    {
        None, Transform, Creation, ViewMode
    }

    public class ToolEventArgs : EventArgs
    {
        public EditorTool Tool { get; }

        public ToolEventArgs(EditorTool tool)
        {
            Tool = tool;
        }
    }
}
