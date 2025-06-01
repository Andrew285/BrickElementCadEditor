namespace App.Theme
{
    public static class ThemeManager
    {
        public static void ApplyDarkTheme(Control control)
        {
            ApplyDarkThemeRecursive(control);
        }

        private static void ApplyDarkThemeRecursive(Control control)
        {
            // Apply theme based on control type
            switch (control)
            {
                case Form form:
                    form.BackColor = ColorScheme.PanelBackground;
                    form.ForeColor = ColorScheme.MenuForeground;
                    break;

                case Panel panel:
                    panel.BackColor = ColorScheme.PanelBackground;
                    panel.ForeColor = ColorScheme.MenuForeground;
                    break;

                case SplitContainer splitContainer:
                    splitContainer.BackColor = ColorScheme.SplitterBackground;
                    break;

                case TextBox textBox:
                    textBox.BackColor = ColorScheme.InputBackground;
                    textBox.ForeColor = ColorScheme.InputForeground;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    break;

                case ComboBox comboBox:
                    comboBox.BackColor = ColorScheme.InputBackground;
                    comboBox.ForeColor = ColorScheme.InputForeground;
                    comboBox.FlatStyle = FlatStyle.Flat;
                    break;

                case Button button:
                    button.BackColor = ColorScheme.ButtonBackground;
                    button.ForeColor = ColorScheme.ButtonForeground;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = ColorScheme.ButtonBorder;
                    break;

                case TreeView treeView:
                    treeView.BackColor = ColorScheme.TreeBackground;
                    treeView.ForeColor = ColorScheme.TreeForeground;
                    treeView.BorderStyle = BorderStyle.None;
                    break;

                case ListView listView:
                    listView.BackColor = ColorScheme.TreeBackground;
                    listView.ForeColor = ColorScheme.TreeForeground;
                    listView.BorderStyle = BorderStyle.None;
                    break;
            }

            // Recursively apply to child controls
            foreach (Control child in control.Controls)
            {
                ApplyDarkThemeRecursive(child);
            }
        }
    }

}
