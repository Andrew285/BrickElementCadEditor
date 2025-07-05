using App.MainForm.Toolstrip;
using App.Theme;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace App.MainForm.Viewport
{
    public class ViewportPanel : UserControl
    {
        private Panel renderPanel;
        private Panel overlayPanel;
        private Form form;
        private ContextMenuStrip viewportContextMenu;
        private System.Windows.Forms.Timer raylibTimer;
        private bool raylibInitialized = false;

        public event EventHandler<ObjectSelectedEventArgs> ObjectSelected;
        public event EventHandler<ViewportEventArgs> ViewportAction;

        private bool isDragging;
        private Point lastMousePosition;
        private ViewportMode currentViewMode;
        Camera3D camera;

        public ViewportPanel(Form form)
        {
            this.form = form;
            currentViewMode = ViewportMode.Solid;
            InitializeComponent();
            SetupLayout();
            CreateContextMenu();
            WireEvents();

            camera = new Camera3D
            {
                Position = new Vector3(0.0f, 0.0f, 10.0f),
                Target = Vector3.Zero,
                Up = Vector3.UnitY,
                FovY = 45.0f,
                Projection = CameraProjection.Perspective
            };
        }

        private void InitializeComponent()
        {
            BackColor = ColorScheme.ViewportBackground;
            Dock = DockStyle.Fill;

            CreateRenderPanel();
            //CreateOverlayPanel();
        }

        private void CreateRenderPanel()
        {
            renderPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorScheme.ViewportBackground,
                BorderStyle = BorderStyle.None
            };

            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, renderPanel, new object[] { true });
        }

        //private void CreateOverlayPanel()
        //{
        //    overlayPanel = new Panel
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = System.Drawing.Color.Transparent,
        //        BorderStyle = BorderStyle.None
        //    };
        //}

        #region WinAPI Entry Points
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowPos(IntPtr handle, IntPtr handleAfter, int x, int y, int cx, int cy, uint flags);
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr child, IntPtr newParent);
        [DllImport("user32.dll")]
        private static extern IntPtr ShowWindow(IntPtr handle, int command);
        #endregion

        public void StartRaylibRenderLoop()
        {
            if (raylibInitialized)
                return;

            Raylib.SetConfigFlags(ConfigFlags.UndecoratedWindow);
            Raylib.InitWindow(renderPanel.Width, renderPanel.Height, "Raylib Viewport");
            Raylib.SetTargetFPS(60);

            unsafe
            {
                void* windowHandleVoid = Raylib.GetWindowHandle();
                var winHandle2 = new IntPtr(windowHandleVoid);

                SetWindowPos(winHandle2, form.Handle, 0, 0, 0, 0, 0x0401);
                SetParent(winHandle2, renderPanel.Handle);
                ShowWindow(winHandle2, 1);
            }

            raylibInitialized = true;

            //raylibTimer = new System.Windows.Forms.Timer();
            //raylibTimer.Interval = 16; // ~60 FPS
            //raylibTimer.Tick += (s, e) => RaylibRenderTick();
            //raylibTimer.Start();
        }

        //private void RaylibRenderTick()
        //{
        //    if (!Raylib.IsWindowReady() || Raylib.WindowShouldClose())
        //    {
        //        raylibTimer?.Stop();
        //        return;
        //    }

        //    Camera3D camera = new Camera3D
        //    {
        //        Position = new Vector3(0.0f, 0.0f, 10.0f),
        //        Target = Vector3.Zero,
        //        Up = Vector3.UnitY,
        //        FovY = 45.0f,
        //        Projection = CameraProjection.Perspective
        //    };

        //    Raylib.BeginDrawing();
        //    Raylib.ClearBackground(Raylib_cs.Color.DarkGray);

        //    Raylib.BeginMode3D(camera);
        //    Raylib.DrawCube(Vector3.Zero, 2.0f, 2.0f, 2.0f, Raylib_cs.Color.Red);
        //    Raylib.DrawGrid(10, 1.0f);
        //    Raylib.EndMode3D();

        //    Raylib.DrawFPS(10, 10);
        //    Raylib.EndDrawing();
        //}

        public void StopRaylibRenderLoop()
        {
            raylibTimer?.Stop();
            if (raylibInitialized)
            {
                Raylib.CloseWindow();
                raylibInitialized = false;
            }
        }

        private void SetupLayout()
        {
            //Controls.Add(overlayPanel);
            Controls.Add(renderPanel);

            var titleLabel = new Label
            {
                Text = "Viewport",
                Dock = DockStyle.Top,
                Height = 25,
                BackColor = ColorScheme.TitleBackground,
                ForeColor = ColorScheme.TitleForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold)
            };

            Controls.Add(titleLabel);

            // Start Raylib after control is created and visible
            this.HandleCreated += (s, e) => StartRaylibRenderLoop();
            renderPanel.Resize += (s, e) =>
            {
                if (Raylib.IsWindowReady())
                {
                    Raylib.SetWindowSize(renderPanel.Width, renderPanel.Height);
                }
            };
        }

        private void CreateContextMenu()
        {
            viewportContextMenu = new ContextMenuStrip
            {
                BackColor = ColorScheme.MenuBackground,
                ForeColor = ColorScheme.MenuForeground
            };

            viewportContextMenu.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Create Vertex", null, (s, e) => CreateVertex()),
                new ToolStripMenuItem("Create Edge", null, (s, e) => CreateEdge()),
                new ToolStripMenuItem("Create Face", null, (s, e) => CreateFace()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Paste", null, (s, e) => PasteObjects()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Frame All", null, (s, e) => FrameAll()),
                new ToolStripMenuItem("Frame Selected", null, (s, e) => FrameSelected()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Viewport Settings...", null, (s, e) => ShowViewportSettings())
            });

            ContextMenuStrip = viewportContextMenu;
        }

        private void WireEvents()
        {
            // Mouse events for interaction
            renderPanel.MouseDown += OnMouseDown;
            renderPanel.MouseMove += OnMouseMove;
            renderPanel.MouseUp += OnMouseUp;
            renderPanel.MouseWheel += OnMouseWheel;
            renderPanel.MouseClick += OnMouseClick;
            renderPanel.MouseDoubleClick += OnMouseDoubleClick;

            // Paint event for rendering
            //renderPanel.Paint += OnRenderPanelPaint;

            ((IMainForm)this.form).OnLoaded += OnAct;

            // Resize event
            //renderPanel.Resize += OnRenderPanelResize;

            // Keyboard events (requires focus)
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            // Drag and drop
            AllowDrop = true;
            DragEnter += OnDragEnter;
            DragDrop += OnDragDrop;
        }

        #region Event Handlers

        private void OnAct(object sender, EventArgs e)
        {
            //StartRaylibRenderLoop();
            OnRenderPanelPaint(sender, e);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            // Focus for keyboard input
            Focus();

            lastMousePosition = e.Location;
            isDragging = true;

            if (e.Button == MouseButtons.Left)
            {
                // TODO: Handle object selection/manipulation
                HandleLeftMouseDown(e);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                // TODO: Start camera navigation
                StartCameraNavigation(e);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var deltaX = e.X - lastMousePosition.X;
                var deltaY = e.Y - lastMousePosition.Y;

                if (e.Button == MouseButtons.Middle)
                {
                    // TODO: Update camera based on mouse movement
                    UpdateCameraNavigation(deltaX, deltaY);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    // TODO: Handle object manipulation
                    HandleObjectManipulation(deltaX, deltaY);
                }

                lastMousePosition = e.Location;
                Invalidate(); // Trigger repaint
            }
            else
            {
                // TODO: Handle mouse hover (highlighting)
                HandleMouseHover(e.Location);
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;

            // TODO: Finalize any ongoing operations
            FinalizeMouseOperation(e);
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            // TODO: Handle camera zoom
            var zoomDelta = e.Delta > 0 ? 1.1f : 0.9f;
            ZoomCamera(zoomDelta);
            Invalidate();
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !isDragging)
            {
                // TODO: Perform object selection
                SelectObjectAtPoint(e.Location);
            }
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // TODO: Frame selected object or start editing
                FrameSelected();
            }
        }

        private void OnRenderPanelPaint(object sender, EventArgs e)
        {
            // Main rendering happens here
            RenderScene();
            //RenderOverlays(e.Graphics);
        }

        private void OnRenderPanelResize(object sender, EventArgs e)
        {
            // TODO: Update camera aspect ratio
            UpdateCameraAspectRatio();
            Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // TODO: Handle viewport-specific shortcuts
            HandleKeyboardShortcut(e);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // TODO: Handle key release events
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            // TODO: Validate drop data
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            // TODO: Handle dropped objects/files
            HandleObjectDrop(e);
        }

        #endregion

        #region Public Methods

        public void SetViewMode(ViewportMode mode)
        {
            currentViewMode = mode;
            Invalidate();
        }

        public void FocusOnObject(object obj)
        {
            // TODO: Frame specific object in viewport
            ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.FocusObject, obj));
        }

        public void SetTool(EditorTool tool)
        {
            // TODO: Change active manipulation tool
            ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.ToolChanged, tool));
        }

        public void FrameAll()
        {
            // TODO: Frame all objects in scene
            ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.FrameAll, null));
        }

        public void FrameSelected()
        {
            // TODO: Frame selected objects
            ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.FrameSelected, null));
        }

        public void ResetCamera()
        {
            // TODO: Reset camera to default position
            ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.ResetCamera, null));
        }

        #endregion

        #region Private Methods

        private void RenderScene()
        {
            //// Clear background
            //graphics.Clear(ColorScheme.ViewportBackground);

            //// TODO: Render 3D scene using graphics context
            //// This is where Raylib-cs integration would go

            //// For now, draw a simple grid and axes
            //DrawGrid(graphics);
            //DrawAxes(graphics);
            //DrawPlaceholderObjects(graphics);


            Camera3D camera = new Camera3D
            {
                Position = new Vector3(0.0f, 0.0f, 10.0f),
                Target = Vector3.Zero,
                Up = Vector3.UnitY,
                FovY = 45.0f,
                Projection = CameraProjection.Perspective
            };

            while (!Raylib.WindowShouldClose())
            {
                if (Raylib.IsMouseButtonDown(MouseButton.Left))
                {
                    Console.WriteLine();
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Raylib_cs.Color(120, 126, 133, 255));
            
                Raylib.BeginMode3D(camera);
                
                // Simple 3D content
                Raylib.DrawCube(Vector3.Zero, 2.0f, 2.0f, 2.0f, Raylib_cs.Color.Red);
                Raylib.DrawGrid(10, 1.0f);
                Raylib.DrawFPS(8, 100);
                Raylib.EndMode3D();
            
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

        }

        private void RenderOverlays(Graphics graphics)
        {
            // Draw viewport info
            DrawViewportInfo(graphics);

            // Draw tool indicators
            DrawToolIndicators(graphics);

            // Draw selection indicators
            DrawSelectionIndicators(graphics);
        }

        private void DrawGrid(Graphics graphics)
        {
            var pen = new Pen(ColorScheme.GridLine, 1);
            var centerX = renderPanel.Width / 2;
            var centerY = renderPanel.Height / 2;
            var gridSize = 20;

            // Draw grid lines
            for (int x = centerX % gridSize; x < renderPanel.Width; x += gridSize)
            {
                graphics.DrawLine(pen, x, 0, x, renderPanel.Height);
            }

            for (int y = centerY % gridSize; y < renderPanel.Height; y += gridSize)
            {
                graphics.DrawLine(pen, 0, y, renderPanel.Width, y);
            }

            pen.Dispose();
        }

        private void DrawAxes(Graphics graphics)
        {
            var centerX = renderPanel.Width / 2;
            var centerY = renderPanel.Height / 2;
            var axisLength = 100;

            // X axis (red)
            graphics.DrawLine(new Pen(Color.Red, 2), centerX, centerY, centerX + axisLength, centerY);
            graphics.DrawString("X", new Font("Arial", 12), Brushes.Red, centerX + axisLength + 5, centerY - 10);

            // Y axis (green)
            graphics.DrawLine(new Pen(Color.Green, 2), centerX, centerY, centerX, centerY - axisLength);
            graphics.DrawString("Y", new Font("Arial", 12), Brushes.Green, centerX + 5, centerY - axisLength - 5);

            // Z axis (blue) - simulated 3D
            graphics.DrawLine(new Pen(Color.Blue, 2), centerX, centerY, centerX - 70, centerY + 70);
            graphics.DrawString("Z", new Font("Arial", 12), Brushes.Blue, centerX - 80, centerY + 70);
        }

        private void DrawPlaceholderObjects(Graphics graphics)
        {
            // TODO: Render actual 3D objects
            // For now, draw some placeholder shapes
            var brush = new SolidBrush(Color.FromArgb(100, Color.White));
            graphics.FillEllipse(brush, 200, 200, 50, 50);
            graphics.FillRectangle(brush, 300, 150, 60, 60);
            brush.Dispose();
        }

        private void DrawViewportInfo(Graphics graphics)
        {
            var info = $"Mode: {currentViewMode} | Tool: Active | FPS: 60";
            graphics.DrawString(info, new Font("Segoe UI", 9), Brushes.White, 10, 10);
        }

        private void DrawToolIndicators(Graphics graphics)
        {
            // TODO: Draw tool-specific indicators (gizmos, etc.)
        }

        private void DrawSelectionIndicators(Graphics graphics)
        {
            // TODO: Draw selection highlights, bounding boxes, etc.
        }

        // Navigation and interaction methods
        private void HandleLeftMouseDown(MouseEventArgs e)
        {
            // TODO: Implement object selection/manipulation start
        }

        private void StartCameraNavigation(MouseEventArgs e)
        {
            // TODO: Start camera navigation mode
        }

        private void UpdateCameraNavigation(int deltaX, int deltaY)
        {
            // TODO: Update camera position/rotation
        }

        private void HandleObjectManipulation(int deltaX, int deltaY)
        {
            // TODO: Handle object transformation
        }

        private void HandleMouseHover(Point location)
        {
            // TODO: Highlight objects under mouse
        }

        private void FinalizeMouseOperation(MouseEventArgs e)
        {
            // TODO: Complete any ongoing operations
        }

        private void ZoomCamera(float zoomDelta)
        {
            // TODO: Zoom camera
        }

        private void SelectObjectAtPoint(Point location)
        {
            // TODO: Perform ray casting to select object
            // For now, just fire event with null
            ObjectSelected?.Invoke(this, new ObjectSelectedEventArgs(null, location));
        }

        private void UpdateCameraAspectRatio()
        {
            // TODO: Update camera aspect ratio based on panel size
        }

        private void HandleKeyboardShortcut(KeyEventArgs e)
        {
            // TODO: Handle viewport-specific shortcuts
            switch (e.KeyCode)
            {
                case Keys.F:
                    FrameSelected();
                    break;
                case Keys.A:
                    FrameAll();
                    break;
                case Keys.G:
                    // Toggle grid
                    break;
            }
        }

        private void HandleObjectDrop(DragEventArgs e)
        {
            // TODO: Handle dropped objects
        }

        // Context menu actions
        private void CreateVertex() => ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.CreateVertex, lastMousePosition));
        private void CreateEdge() => ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.CreateEdge, lastMousePosition));
        private void CreateFace() => ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.CreateFace, lastMousePosition));
        private void PasteObjects() => ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.Paste, lastMousePosition));
        private void ShowViewportSettings() => ViewportAction?.Invoke(this, new ViewportEventArgs(ViewportActionType.ShowSettings, null));

        #endregion
    }

    // Enums and event args
    public enum ViewportMode
    {
        Wireframe, Solid, Textured, Shaded
    }

    public enum ViewportActionType
    {
        FocusObject, ToolChanged, FrameAll, FrameSelected, ResetCamera,
        CreateVertex, CreateEdge, CreateFace, Paste, ShowSettings
    }

    public class ObjectSelectedEventArgs : EventArgs
    {
        public object SelectedObject { get; }
        public Point ScreenLocation { get; }

        public ObjectSelectedEventArgs(object selectedObject, Point screenLocation)
        {
            SelectedObject = selectedObject;
            ScreenLocation = screenLocation;
        }
    }

    public class ViewportEventArgs : EventArgs
    {
        public ViewportActionType ActionType { get; }
        public object Data { get; }

        public ViewportEventArgs(ViewportActionType actionType, object data)
        {
            ActionType = actionType;
            Data = data;
        }
    }
}
