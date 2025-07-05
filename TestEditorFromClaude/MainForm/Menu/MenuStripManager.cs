using App.Theme;

namespace App.MainForm.Menu
{
    public class MenuStripManager
    {
        public MenuStrip MenuStrip { get; private set; }

        public event EventHandler<MenuItemEventArgs> MenuItemClicked;

        public MenuStripManager()
        {
            CreateMenuStrip();
            CreateMenuItems();
            ApplyTheme();
        }

        private void CreateMenuStrip()
        {
            MenuStrip = new MenuStrip(); // Use default appearance
        }

        private void CreateMenuItems()
        {
            // File Menu
            var fileMenu = CreateMenu("&File");
            fileMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&New", "Ctrl+N", MenuAction.FileNew),
                CreateMenuItem("&Open...", "Ctrl+O", MenuAction.FileOpen),
                new ToolStripSeparator(),
                CreateMenuItem("&Save", "Ctrl+S", MenuAction.FileSave),
                CreateMenuItem("Save &As...", "Ctrl+Shift+S", MenuAction.FileSaveAs),
                new ToolStripSeparator(),
                CreateMenuItem("&Import...", "", MenuAction.FileImport),
                CreateMenuItem("&Export...", "", MenuAction.FileExport),
                new ToolStripSeparator(),
                CreateMenuItem("E&xit", "Alt+F4", MenuAction.FileExit)
            });

            // Edit Menu
            var editMenu = CreateMenu("&Edit");
            editMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Undo", "Ctrl+Z", MenuAction.EditUndo),
                CreateMenuItem("&Redo", "Ctrl+Y", MenuAction.EditRedo),
                new ToolStripSeparator(),
                CreateMenuItem("Cu&t", "Ctrl+X", MenuAction.EditCut),
                CreateMenuItem("&Copy", "Ctrl+C", MenuAction.EditCopy),
                CreateMenuItem("&Paste", "Ctrl+V", MenuAction.EditPaste),
                CreateMenuItem("&Delete", "Delete", MenuAction.EditDelete),
                new ToolStripSeparator(),
                CreateMenuItem("Select &All", "Ctrl+A", MenuAction.EditSelectAll),
                new ToolStripSeparator(),
                CreateMenuItem("Preferences...", "", MenuAction.EditPreferences)
            });

            // Object Menu
            var objectMenu = CreateMenu("&Object");
            objectMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("Create &Vertex", "", MenuAction.ObjectCreateVertex),
                CreateMenuItem("Create &Edge", "", MenuAction.ObjectCreateEdge),
                CreateMenuItem("Create &Face", "", MenuAction.ObjectCreateFace),
                new ToolStripSeparator(),
                CreateMenuItem("&Transform...", "T", MenuAction.ObjectTransform),
                CreateMenuItem("&Duplicate", "Ctrl+D", MenuAction.ObjectDuplicate)
            });

            // Tools Menu
            var toolsMenu = CreateMenu("&Tools");
            toolsMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Triangulate", "F5", MenuAction.ToolsTriangulate),
                CreateMenuItem("&Optimize Mesh", "F6", MenuAction.ToolsOptimize),
                new ToolStripSeparator(),
                CreateMenuItem("&Validate Geometry", "", MenuAction.ToolsValidate),
                CreateMenuItem("&Statistics", "", MenuAction.ToolsStatistics)
            });

            // View Menu
            var viewMenu = CreateMenu("&View");
            viewMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Wireframe", "F1", MenuAction.ViewWireframe),
                CreateMenuItem("&Solid", "F2", MenuAction.ViewSolid),
                CreateMenuItem("&Textured", "F3", MenuAction.ViewTextured),
                new ToolStripSeparator(),
                CreateMenuItem("Show &Grid", "G", MenuAction.ViewGrid),
                CreateMenuItem("Show &Axes", "A", MenuAction.ViewAxes),
                new ToolStripSeparator(),
                CreateMenuItem("Reset &Camera", "Home", MenuAction.ViewResetCamera)
            });

            // Help Menu
            var helpMenu = CreateMenu("&Help");
            helpMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Help Topics", "F1", MenuAction.HelpTopics),
                CreateMenuItem("&Keyboard Shortcuts", "", MenuAction.HelpShortcuts),
                new ToolStripSeparator(),
                CreateMenuItem("&About...", "", MenuAction.HelpAbout)
            });

            // Add all menus to menu strip
            MenuStrip.Items.AddRange(new ToolStripItem[]
            {
                fileMenu, editMenu, objectMenu, toolsMenu, viewMenu, helpMenu
            });
        }

        private ToolStripMenuItem CreateMenu(string text)
        {
            return new ToolStripMenuItem(text); // Use default appearance
        }

        private ToolStripMenuItem CreateMenuItem(string text, string shortcut, MenuAction action)
        {
            var item = new ToolStripMenuItem(text)
            {
                ShortcutKeyDisplayString = shortcut,
                Tag = action
            };
            item.Click += OnMenuItemClick;
            return item;
        }

        private void OnMenuItemClick(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is MenuAction action)
            {
                MenuItemClicked?.Invoke(this, new MenuItemEventArgs(action));
            }
        }

        private void ApplyTheme()
        {
            // Do not set a custom renderer
        }
    }

    // Custom menu actions enum
    public enum MenuAction
    {
        // File actions
        FileNew, FileOpen, FileSave, FileSaveAs, FileImport, FileExport, FileExit,

        // Edit actions
        EditUndo, EditRedo, EditCut, EditCopy, EditPaste, EditDelete, EditSelectAll, EditPreferences,

        // Object actions
        ObjectCreateVertex, ObjectCreateEdge, ObjectCreateFace, ObjectTransform, ObjectDuplicate,

        // Tools actions
        ToolsTriangulate, ToolsOptimize, ToolsValidate, ToolsStatistics,

        // View actions
        ViewWireframe, ViewSolid, ViewTextured, ViewGrid, ViewAxes, ViewResetCamera,

        // Help actions
        HelpTopics, HelpShortcuts, HelpAbout
    }

    public class MenuItemEventArgs : EventArgs
    {
        public MenuAction Action { get; }

        public MenuItemEventArgs(MenuAction action)
        {
            Action = action;
        }
    }
}
