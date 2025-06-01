using App.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.MainForm.Library
{
    public class LibraryPanel : UserControl
    {
        private FlowLayoutPanel primitivesFlow;
        private ComboBox categoryCombo;
        private TextBox searchBox;

        public event EventHandler<PrimitiveSelectedEventArgs> PrimitiveSelected;

        private readonly Dictionary<PrimitiveCategory, List<PrimitiveInfo>> primitivesByCategory;

        public LibraryPanel()
        {
            primitivesByCategory = new Dictionary<PrimitiveCategory, List<PrimitiveInfo>>();

            InitializeComponent();
            SetupLayout();
            InitializePrimitives();
            PopulatePrimitives();
            WireEvents();
        }

        private void InitializeComponent()
        {
            BackColor = ColorScheme.PanelBackground;
            Dock = DockStyle.Fill;

            CreateSearchBox();
            CreateCategoryCombo();
            CreatePrimitivesFlow();
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
                Text = "Search primitives..."
            };
        }

        private void CreateCategoryCombo()
        {
            categoryCombo = new ComboBox
            {
                Dock = DockStyle.Top,
                BackColor = ColorScheme.InputBackground,
                ForeColor = ColorScheme.InputForeground,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };

            // Populate categories
            categoryCombo.Items.Add("All");
            foreach (PrimitiveCategory category in Enum.GetValues<PrimitiveCategory>())
            {
                categoryCombo.Items.Add(GetCategoryDisplayName(category));
            }
            categoryCombo.SelectedIndex = 0;
        }

        private void CreatePrimitivesFlow()
        {
            primitivesFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorScheme.PanelBackground,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(5)
            };
        }

        private void SetupLayout()
        {
            // Create title label
            var titleLabel = new Label
            {
                Text = "Library",
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
                primitivesFlow,
                searchBox,
                categoryCombo,
                titleLabel
            });
        }

        private void InitializePrimitives()
        {
            // Basic Shapes
            var basicShapes = new List<PrimitiveInfo>
            {
                new PrimitiveInfo("Point", "●", "Create a single point", PrimitiveType.Point),
                new PrimitiveInfo("Line", "—", "Create a straight line", PrimitiveType.Line),
                new PrimitiveInfo("Cube", "⬜", "Create a cube", PrimitiveType.Cube),
                new PrimitiveInfo("Sphere", "⚫", "Create a sphere", PrimitiveType.Sphere),
                new PrimitiveInfo("Cylinder", "🥫", "Create a cylinder", PrimitiveType.Cylinder),
                new PrimitiveInfo("Cone", "🔺", "Create a cone", PrimitiveType.Cone),
                new PrimitiveInfo("Plane", "▭", "Create a plane", PrimitiveType.Plane)
            };

            // Curves
            var curves = new List<PrimitiveInfo>
            {
                new PrimitiveInfo("Bezier Curve", "〰️", "Create a Bezier curve", PrimitiveType.BezierCurve),
                new PrimitiveInfo("NURBS Curve", "📏", "Create a NURBS curve", PrimitiveType.NurbsCurve),
                new PrimitiveInfo("Circle", "⭕", "Create a circle", PrimitiveType.Circle),
                new PrimitiveInfo("Arc", "⌒", "Create an arc", PrimitiveType.Arc),
                new PrimitiveInfo("Spline", "〜", "Create a spline", PrimitiveType.Spline)
            };

            // Surfaces
            var surfaces = new List<PrimitiveInfo>
            {
                new PrimitiveInfo("Bezier Surface", "📄", "Create a Bezier surface", PrimitiveType.BezierSurface),
                new PrimitiveInfo("NURBS Surface", "📋", "Create a NURBS surface", PrimitiveType.NurbsSurface),
                new PrimitiveInfo("Ruled Surface", "📐", "Create a ruled surface", PrimitiveType.RuledSurface),
                new PrimitiveInfo("Revolution Surface", "🌀", "Create a surface of revolution", PrimitiveType.RevolutionSurface)
            };

            // Complex Objects
            var complexObjects = new List<PrimitiveInfo>
            {
                new PrimitiveInfo("Hexahedron", "🧊", "Create a curvilinear hexahedron", PrimitiveType.CurvilinearHexahedron),
                new PrimitiveInfo("Tetrahedron", "🔺", "Create a tetrahedron", PrimitiveType.Tetrahedron),
                new PrimitiveInfo("Prism", "📐", "Create a prism", PrimitiveType.Prism),
                new PrimitiveInfo("Pyramid", "🔺", "Create a pyramid", PrimitiveType.Pyramid)
            };

            // Materials
            var materials = new List<PrimitiveInfo>
            {
                new PrimitiveInfo("Steel", "⚙️", "Add steel material", PrimitiveType.SteelMaterial),
                new PrimitiveInfo("Aluminum", "✨", "Add aluminum material", PrimitiveType.AluminumMaterial),
                new PrimitiveInfo("Plastic", "🧪", "Add plastic material", PrimitiveType.PlasticMaterial),
                new PrimitiveInfo("Concrete", "🧱", "Add concrete material", PrimitiveType.ConcreteMaterial),
                new PrimitiveInfo("Wood", "🌳", "Add wood material", PrimitiveType.WoodMaterial)
            };

            // Lights
            var lights = new List<PrimitiveInfo>
            {
                new PrimitiveInfo("Point Light", "💡", "Add point light", PrimitiveType.PointLight),
                new PrimitiveInfo("Directional Light", "☀️", "Add directional light", PrimitiveType.DirectionalLight),
                new PrimitiveInfo("Spot Light", "🔦", "Add spot light", PrimitiveType.SpotLight),
                new PrimitiveInfo("Area Light", "🔆", "Add area light", PrimitiveType.AreaLight)
            };

            // Populate dictionary
            primitivesByCategory[PrimitiveCategory.BasicShapes] = basicShapes;
            primitivesByCategory[PrimitiveCategory.Curves] = curves;
            primitivesByCategory[PrimitiveCategory.Surfaces] = surfaces;
            primitivesByCategory[PrimitiveCategory.ComplexObjects] = complexObjects;
            primitivesByCategory[PrimitiveCategory.Materials] = materials;
            primitivesByCategory[PrimitiveCategory.Lights] = lights;
        }

        private void PopulatePrimitives()
        {
            primitivesFlow.Controls.Clear();

            var selectedCategory = GetSelectedCategory();
            var searchText = GetSearchText();

            var primitivesToShow = GetFilteredPrimitives(selectedCategory, searchText);

            foreach (var primitive in primitivesToShow)
            {
                var button = CreatePrimitiveButton(primitive);
                primitivesFlow.Controls.Add(button);
            }
        }

        private Button CreatePrimitiveButton(PrimitiveInfo primitive)
        {
            var button = new Button
            {
                Text = $"{primitive.Emoji} {primitive.Name}",
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(primitivesFlow.Width - 30, 35),
                BackColor = ColorScheme.ButtonBackground,
                ForeColor = ColorScheme.ButtonForeground,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(2),
                Tag = primitive,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderColor = ColorScheme.ButtonBorder;
            button.FlatAppearance.MouseOverBackColor = ColorScheme.ButtonHover;
            button.FlatAppearance.MouseDownBackColor = ColorScheme.ButtonPressed;

            // Add tooltip
            var toolTip = new ToolTip();
            toolTip.SetToolTip(button, primitive.Description);

            button.Click += OnPrimitiveButtonClick;

            return button;
        }

        private void WireEvents()
        {
            categoryCombo.SelectedIndexChanged += OnCategoryChanged;
            searchBox.TextChanged += OnSearchTextChanged;
            searchBox.Enter += OnSearchBoxEnter;
            searchBox.Leave += OnSearchBoxLeave;
        }

        #region Event Handlers

        private void OnCategoryChanged(object sender, EventArgs e)
        {
            PopulatePrimitives();
        }

        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text != "Search primitives...")
            {
                PopulatePrimitives();
            }
        }

        private void OnSearchBoxEnter(object sender, EventArgs e)
        {
            if (searchBox.Text == "Search primitives...")
            {
                searchBox.Text = "";
                searchBox.ForeColor = ColorScheme.InputForeground;
            }
        }

        private void OnSearchBoxLeave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchBox.Text))
            {
                searchBox.Text = "Search primitives...";
                searchBox.ForeColor = ColorScheme.InputPlaceholder;
            }
        }

        private void OnPrimitiveButtonClick(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is PrimitiveInfo primitive)
            {
                PrimitiveSelected?.Invoke(this, new PrimitiveSelectedEventArgs(primitive.Type, primitive));
            }
        }

        #endregion

        #region Private Methods

        private PrimitiveCategory? GetSelectedCategory()
        {
            if (categoryCombo.SelectedIndex == 0) // "All"
                return null;

            var categoryName = categoryCombo.SelectedItem.ToString();
            foreach (PrimitiveCategory category in Enum.GetValues<PrimitiveCategory>())
            {
                if (GetCategoryDisplayName(category) == categoryName)
                    return category;
            }
            return null;
        }

        private string GetSearchText()
        {
            return searchBox.Text == "Search primitives..." ? "" : searchBox.Text;
        }

        private List<PrimitiveInfo> GetFilteredPrimitives(PrimitiveCategory? category, string searchText)
        {
            var allPrimitives = new List<PrimitiveInfo>();

            if (category.HasValue)
            {
                if (primitivesByCategory.ContainsKey(category.Value))
                    allPrimitives.AddRange(primitivesByCategory[category.Value]);
            }
            else
            {
                foreach (var categoryPrimitives in primitivesByCategory.Values)
                {
                    allPrimitives.AddRange(categoryPrimitives);
                }
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                allPrimitives = allPrimitives
                    .Where(p => p.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                               p.Description.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            return allPrimitives;
        }

        private string GetCategoryDisplayName(PrimitiveCategory category)
        {
            return category switch
            {
                PrimitiveCategory.BasicShapes => "Basic Shapes",
                PrimitiveCategory.Curves => "Curves",
                PrimitiveCategory.Surfaces => "Surfaces",
                PrimitiveCategory.ComplexObjects => "Complex Objects",
                PrimitiveCategory.Materials => "Materials",
                PrimitiveCategory.Lights => "Lights",
                _ => category.ToString()
            };
        }

        #endregion

        #region Public Methods

        public void RefreshPrimitives()
        {
            PopulatePrimitives();
        }

        public void SelectCategory(PrimitiveCategory category)
        {
            var displayName = GetCategoryDisplayName(category);
            categoryCombo.SelectedItem = displayName;
        }

        public void ClearSearch()
        {
            searchBox.Text = "";
            PopulatePrimitives();
        }

        #endregion
    }

    // Helper classes and enums
    public enum PrimitiveCategory
    {
        BasicShapes,
        Curves,
        Surfaces,
        ComplexObjects,
        Materials,
        Lights
    }

    public enum PrimitiveType
    {
        // Basic Shapes
        Point, Line, Cube, Sphere, Cylinder, Cone, Plane,

        // Curves
        BezierCurve, NurbsCurve, Circle, Arc, Spline,

        // Surfaces
        BezierSurface, NurbsSurface, RuledSurface, RevolutionSurface,

        // Complex Objects
        CurvilinearHexahedron, Tetrahedron, Prism, Pyramid,

        // Materials
        SteelMaterial, AluminumMaterial, PlasticMaterial, ConcreteMaterial, WoodMaterial,

        // Lights
        PointLight, DirectionalLight, SpotLight, AreaLight
    }

    public class PrimitiveInfo
    {
        public string Name { get; }
        public string Emoji { get; }
        public string Description { get; }
        public PrimitiveType Type { get; }

        public PrimitiveInfo(string name, string emoji, string description, PrimitiveType type)
        {
            Name = name;
            Emoji = emoji;
            Description = description;
            Type = type;
        }
    }

    public class PrimitiveSelectedEventArgs : EventArgs
    {
        public PrimitiveType Type { get; }
        public PrimitiveInfo Info { get; }

        public PrimitiveSelectedEventArgs(PrimitiveType type, PrimitiveInfo info)
        {
            Type = type;
            Info = info;
        }
    }
}
