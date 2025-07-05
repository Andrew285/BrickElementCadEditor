using App.MainForm;
using App.MainForm.Hierarchy;
using App.MainForm.Library;
using App.MainForm.Menu;
using App.MainForm.Properties;
using App.MainForm.Status;
using App.MainForm.Toolstrip;
using App.MainForm.Viewport;
using App.Theme;
using App.Utils;
using ToolStripManager = App.MainForm.Toolstrip.ToolStripManager;

namespace BrickElementCadEditor
{
    public partial class MainForm : Form, IMainForm
    {
        private MenuStripManager menuManager;
        private ToolStripManager toolStripManager;
        private StatusStripManager statusManager;
        private LayoutManager layoutManager;

        // UI Panels
        private HierarchyPanel hierarchyPanel;
        private PropertiesPanel propertiesPanel;
        private ViewportPanel viewportPanel;
        private LibraryPanel libraryPanel;
        public EventHandler OnLoaded {  get; set; }

        public MainForm()
        {
            InitializeComponent2();
            InitializeManagers();
            SetupLayout();
            ApplyTheme();
            WireEvents();
        }

        private void InitializeComponent2()
        {
            // Basic form setup
            Text = "MeshEditor";
            Size = new Size(1400, 900);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 700);

            // Enable double buffering for smooth rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer, true);
        }

        private void InitializeManagers()
        {
            // Initialize UI managers
            menuManager = new MenuStripManager();
            toolStripManager = new ToolStripManager();
            statusManager = new StatusStripManager();
            layoutManager = new LayoutManager();

            // Initialize panels
            hierarchyPanel = new HierarchyPanel();
            propertiesPanel = new PropertiesPanel();
            viewportPanel = new ViewportPanel(this);
            libraryPanel = new LibraryPanel();
        }

        private void SetupLayout()
        {
            var mainLayout = layoutManager.CreateMainLayout(
                 hierarchyPanel,
                 libraryPanel,
                 viewportPanel,
                 propertiesPanel);
            Controls.Add(mainLayout);

            // Потім статус бар (буде внизу)
            Controls.Add(statusManager.StatusStrip);

            // Потім панель інструментів
            Controls.Add(toolStripManager.ToolStrip);

            // І нарешті меню (буде зверху)
            Controls.Add(menuManager.MenuStrip);
            MainMenuStrip = menuManager.MenuStrip;
        }

        private void ApplyTheme()
        {
            // Apply dark theme to all components
            ThemeManager.ApplyDarkTheme(this);
            ThemeManager.ApplyDarkTheme(hierarchyPanel);
            ThemeManager.ApplyDarkTheme(propertiesPanel);
            ThemeManager.ApplyDarkTheme(viewportPanel);
            ThemeManager.ApplyDarkTheme(libraryPanel);
        }

        private void WireEvents()
        {
            // Wire up cross-panel communication events
            hierarchyPanel.SelectionChanged += OnHierarchySelectionChanged;
            libraryPanel.PrimitiveSelected += OnPrimitiveSelected;
            viewportPanel.ObjectSelected += OnViewportObjectSelected;

            // Wire menu and toolbar events
            menuManager.MenuItemClicked += OnMenuItemClicked;
            toolStripManager.ToolSelected += OnToolSelected;

            // Form events
            FormClosing += OnFormClosing;
            Resize += OnFormResize;

            this.Shown += (s, e) => OnLoaded?.Invoke(this, EventArgs.Empty);
        }

        #region Event Handlers

        private void OnHierarchySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Update properties panel with selected object
            // TODO: Highlight object in viewport
        }

        private void OnPrimitiveSelected(object sender, PrimitiveSelectedEventArgs e)
        {
            // TODO: Create new primitive and add to scene
        }

        private void OnViewportObjectSelected(object sender, ObjectSelectedEventArgs e)
        {
            // TODO: Update hierarchy selection
            // TODO: Update properties panel
        }

        private void OnMenuItemClicked(object sender, MenuItemEventArgs e)
        {
            // TODO: Handle menu actions (File, Edit, etc.)
            if (e.Action == MenuAction.FileExit)
            {
                this.Close();
            }
        }

        private void OnToolSelected(object sender, ToolEventArgs e)
        {
            // TODO: Change active tool in viewport
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO: Check for unsaved changes
            // TODO: Save window state
        }

        private void OnFormResize(object sender, EventArgs e)
        {
            // TODO: Handle responsive layout adjustments
        }

        #endregion

        #region Public Methods

        public void UpdateStatus(string message)
        {
            statusManager.UpdateStatus(message);
        }

        public void SetProgress(int percentage)
        {
            statusManager.SetProgress(percentage);
        }

        public void ShowNotification(string message, NotificationType type)
        {
            // TODO: Show notification to user
        }

        #endregion
    }
}
