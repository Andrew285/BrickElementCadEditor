using App.Theme;
using System.ComponentModel;

namespace App.MainForm.Properties
{
    public class PropertiesPanel : UserControl
    {
        private PropertyGrid propertyGrid;
        private ComboBox objectTypeCombo;
        private Label selectedObjectLabel;
        private Panel headerPanel;

        public event EventHandler<PropertyChangedEventArgs> PropertyValueChanged;

        private object currentObject;

        public PropertiesPanel()
        {
            InitializeComponent();
            SetupLayout();
            WireEvents();
        }

        private void InitializeComponent()
        {
            BackColor = ColorScheme.PanelBackground;
            Dock = DockStyle.Fill;

            CreateHeaderPanel();
            CreatePropertyGrid();
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = ColorScheme.PanelBackground
            };

            // Title label
            var titleLabel = new Label
            {
                Text = "Properties",
                Dock = DockStyle.Top,
                Height = 25,
                BackColor = ColorScheme.TitleBackground,
                ForeColor = ColorScheme.TitleForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            // Selected object info
            selectedObjectLabel = new Label
            {
                Text = "No object selected",
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = ColorScheme.InfoBackground,
                ForeColor = ColorScheme.InfoForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Font = new Font("Segoe UI", 8F, FontStyle.Italic)
            };

            // Object type selector
            objectTypeCombo = new ComboBox
            {
                Dock = DockStyle.Top,
                BackColor = ColorScheme.InputBackground,
                ForeColor = ColorScheme.InputForeground,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F),
                Visible = false
            };

            headerPanel.Controls.AddRange(new Control[] {
                objectTypeCombo,
                selectedObjectLabel,
                titleLabel
            });
        }

        private void CreatePropertyGrid()
        {
            propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                PropertySort = PropertySort.Categorized,
                ToolbarVisible = true,
                HelpVisible = true,

                // Dark theme colors
                BackColor = ColorScheme.PropertyGridBackground,
                LineColor = ColorScheme.PropertyGridLine,
                CategoryForeColor = ColorScheme.PropertyGridCategoryText,
                CategorySplitterColor = ColorScheme.PropertyGridLine,
                CommandsBackColor = ColorScheme.PropertyGridBackground,
                CommandsForeColor = ColorScheme.PropertyGridText,
                CommandsBorderColor = ColorScheme.PropertyGridLine,
                HelpBackColor = ColorScheme.PropertyGridHelpBackground,
                HelpForeColor = ColorScheme.PropertyGridHelpText,
                HelpBorderColor = ColorScheme.PropertyGridLine,
                ViewBackColor = ColorScheme.PropertyGridBackground,
                ViewForeColor = ColorScheme.PropertyGridText,
                ViewBorderColor = ColorScheme.PropertyGridLine
            };
        }

        private void SetupLayout()
        {
            Controls.AddRange(new Control[] {
                propertyGrid,
                headerPanel
            });
        }

        private void WireEvents()
        {
            propertyGrid.PropertyValueChanged += OnPropertyValueChanged;
            objectTypeCombo.SelectedIndexChanged += OnObjectTypeChanged;
        }

        #region Event Handlers

        private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // Notify that a property has changed
            PropertyValueChanged?.Invoke(this, new PropertyChangedEventArgs(e.ChangedItem.PropertyDescriptor.Name));

            // TODO: Update the actual object property
            // TODO: Mark document as modified
            // TODO: Add to undo history
        }

        private void OnObjectTypeChanged(object sender, EventArgs e)
        {
            // TODO: Handle object type change if needed
            // This could be used for polymorphic objects
        }

        #endregion

        #region Public Methods

        public void ShowProperties(object obj)
        {
            if (obj == null)
            {
                ClearProperties();
                return;
            }

            currentObject = obj;
            propertyGrid.SelectedObject = obj;

            UpdateObjectInfo(obj);
            SetupObjectTypeCombo(obj);
        }

        public void ShowProperties(object[] objects)
        {
            if (objects == null || objects.Length == 0)
            {
                ClearProperties();
                return;
            }

            if (objects.Length == 1)
            {
                ShowProperties(objects[0]);
                return;
            }

            // Multiple selection - show common properties
            propertyGrid.SelectedObjects = objects;
            selectedObjectLabel.Text = $"Multiple objects selected ({objects.Length})";
            objectTypeCombo.Visible = false;
        }

        public void RefreshProperties()
        {
            if (currentObject != null)
            {
                propertyGrid.Refresh();
            }
        }

        public void ClearProperties()
        {
            currentObject = null;
            propertyGrid.SelectedObject = null;
            selectedObjectLabel.Text = "No object selected";
            objectTypeCombo.Visible = false;
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            if (currentObject != null)
            {
                try
                {
                    var property = currentObject.GetType().GetProperty(propertyName);
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(currentObject, value);
                        propertyGrid.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Show error message
                    MessageBox.Show($"Failed to set property '{propertyName}': {ex.Message}",
                                  "Property Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public T GetPropertyValue<T>(string propertyName)
        {
            if (currentObject != null)
            {
                try
                {
                    var property = currentObject.GetType().GetProperty(propertyName);
                    if (property != null && property.CanRead)
                    {
                        return (T)property.GetValue(currentObject);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
            }
            return default;
        }

        #endregion

        #region Private Methods

        private void UpdateObjectInfo(object obj)
        {
            var typeName = obj.GetType().Name;
            var displayName = GetObjectDisplayName(obj);

            selectedObjectLabel.Text = $"Selected: {typeName}";
            if (!string.IsNullOrEmpty(displayName))
            {
                selectedObjectLabel.Text += $" - {displayName}";
            }
        }

        private string GetObjectDisplayName(object obj)
        {
            // Try to get name from common properties
            var nameProperty = obj.GetType().GetProperty("Name");
            if (nameProperty != null && nameProperty.CanRead)
            {
                return nameProperty.GetValue(obj)?.ToString();
            }

            var idProperty = obj.GetType().GetProperty("Id");
            if (idProperty != null && idProperty.CanRead)
            {
                return $"ID: {idProperty.GetValue(obj)}";
            }

            return null;
        }

        private void SetupObjectTypeCombo(object obj)
        {
            objectTypeCombo.Items.Clear();

            // TODO: Add relevant object type categories based on object type
            // This could be used for filtering property categories
            objectTypeCombo.Items.AddRange(new string[]
            {
                "General",
                "Transform",
                "Appearance",
                "Physics",
                "Advanced"
            });

            objectTypeCombo.SelectedIndex = 0;
            objectTypeCombo.Visible = true;
        }

        #endregion
    }

    // Helper class for property editing
    public class PropertyEditEventArgs : EventArgs
    {
        public string PropertyName { get; }
        public object OldValue { get; }
        public object NewValue { get; }
        public object Target { get; }

        public PropertyEditEventArgs(string propertyName, object oldValue, object newValue, object target)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
            Target = target;
        }
    }
}
