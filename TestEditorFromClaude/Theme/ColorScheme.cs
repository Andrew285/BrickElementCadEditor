using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Theme
{
    public static class ColorScheme
    {
        // Змінна для переключення теми
        public static bool IsDarkTheme { get; set; } = false; // Змінено на false для світлої теми

        // Main backgrounds
        public static Color PanelBackground => IsDarkTheme ? Color.FromArgb(37, 37, 38) : Color.FromArgb(240, 240, 240);
        public static Color ViewportBackground => IsDarkTheme ? Color.FromArgb(30, 30, 30) : Color.White;
        public static Color SplitterBackground => IsDarkTheme ? Color.FromArgb(45, 45, 48) : Color.FromArgb(230, 230, 230);

        // Menu and toolbar
        public static Color MenuBackground => IsDarkTheme ? Color.FromArgb(45, 45, 48) : Color.FromArgb(245, 245, 245);
        public static Color MenuForeground => IsDarkTheme ? Color.White : Color.Black;
        public static Color ToolbarBackground => IsDarkTheme ? Color.FromArgb(37, 37, 38) : Color.FromArgb(240, 240, 240);
        public static Color ToolbarForeground => IsDarkTheme ? Color.White : Color.Black;

        // Titles and headers
        public static Color TitleBackground => IsDarkTheme ? Color.FromArgb(45, 45, 48) : Color.FromArgb(230, 230, 230);
        public static Color TitleForeground => IsDarkTheme ? Color.White : Color.Black;
        public static Color InfoBackground => IsDarkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(250, 250, 250);
        public static Color InfoForeground => IsDarkTheme ? Color.LightGray : Color.DarkGray;

        // Input controls
        public static Color InputBackground => IsDarkTheme ? Color.FromArgb(60, 60, 60) : Color.White;
        public static Color InputForeground => IsDarkTheme ? Color.White : Color.Black;
        public static Color InputPlaceholder => IsDarkTheme ? Color.Gray : Color.Gray;

        // Buttons
        public static Color ButtonBackground => IsDarkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(230, 230, 230);
        public static Color ButtonForeground => IsDarkTheme ? Color.White : Color.Black;
        public static Color ButtonBorder => IsDarkTheme ? Color.FromArgb(80, 80, 80) : Color.FromArgb(180, 180, 180);
        public static Color ButtonHover => IsDarkTheme ? Color.FromArgb(70, 70, 70) : Color.FromArgb(220, 220, 220);
        public static Color ButtonPressed => IsDarkTheme ? Color.FromArgb(50, 50, 50) : Color.FromArgb(200, 200, 200);

        // Tree view
        public static Color TreeBackground => IsDarkTheme ? Color.FromArgb(37, 37, 38) : Color.White;
        public static Color TreeForeground => IsDarkTheme ? Color.White : Color.Black;
        public static Color TreeSelection => Color.FromArgb(0, 122, 204); // Залишається синім

        // Property grid
        public static Color PropertyGridBackground => IsDarkTheme ? Color.FromArgb(37, 37, 38) : Color.White;
        public static Color PropertyGridText => IsDarkTheme ? Color.White : Color.Black;
        public static Color PropertyGridLine => IsDarkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(200, 200, 200);
        public static Color PropertyGridCategoryText => IsDarkTheme ? Color.White : Color.Black;
        public static Color PropertyGridHelpBackground => IsDarkTheme ? Color.FromArgb(45, 45, 48) : Color.FromArgb(245, 245, 245);
        public static Color PropertyGridHelpText => IsDarkTheme ? Color.White : Color.Black;

        // Viewport specific
        public static Color GridLine => IsDarkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(220, 220, 220);
        public static Color GridMajorLine => IsDarkTheme ? Color.FromArgb(80, 80, 80) : Color.FromArgb(180, 180, 180);

        // Status and accent colors (залишаються незмінними)
        public static readonly Color AccentBlue = Color.FromArgb(0, 122, 204);
        public static readonly Color SuccessGreen = Color.FromArgb(0, 128, 0);
        public static readonly Color WarningYellow = Color.FromArgb(255, 193, 7);
        public static readonly Color ErrorRed = Color.FromArgb(220, 53, 69);

        // Метод для переключення теми
        public static void SetDarkTheme(bool isDark)
        {
            IsDarkTheme = isDark;
        }
    }

    //public static class ColorScheme
    //{
    //    // Main backgrounds
    //    public static readonly Color PanelBackground = Color.FromArgb(37, 37, 38);
    //    public static readonly Color ViewportBackground = Color.FromArgb(30, 30, 30);
    //    public static readonly Color SplitterBackground = Color.FromArgb(45, 45, 48);

    //    // Menu and toolbar
    //    public static readonly Color MenuBackground = Color.FromArgb(45, 45, 48);
    //    public static readonly Color MenuForeground = Color.White;
    //    public static readonly Color ToolbarBackground = Color.FromArgb(37, 37, 38);
    //    public static readonly Color ToolbarForeground = Color.White;

    //    // Titles and headers
    //    public static readonly Color TitleBackground = Color.FromArgb(45, 45, 48);
    //    public static readonly Color TitleForeground = Color.White;
    //    public static readonly Color InfoBackground = Color.FromArgb(60, 60, 60);
    //    public static readonly Color InfoForeground = Color.LightGray;

    //    // Input controls
    //    public static readonly Color InputBackground = Color.FromArgb(60, 60, 60);
    //    public static readonly Color InputForeground = Color.White;
    //    public static readonly Color InputPlaceholder = Color.Gray;

    //    // Buttons
    //    public static readonly Color ButtonBackground = Color.FromArgb(60, 60, 60);
    //    public static readonly Color ButtonForeground = Color.White;
    //    public static readonly Color ButtonBorder = Color.FromArgb(80, 80, 80);
    //    public static readonly Color ButtonHover = Color.FromArgb(70, 70, 70);
    //    public static readonly Color ButtonPressed = Color.FromArgb(50, 50, 50);

    //    // Tree view
    //    public static readonly Color TreeBackground = Color.FromArgb(37, 37, 38);
    //    public static readonly Color TreeForeground = Color.White;
    //    public static readonly Color TreeSelection = Color.FromArgb(0, 122, 204);

    //    // Property grid
    //    public static readonly Color PropertyGridBackground = Color.FromArgb(37, 37, 38);
    //    public static readonly Color PropertyGridText = Color.White;
    //    public static readonly Color PropertyGridLine = Color.FromArgb(60, 60, 60);
    //    public static readonly Color PropertyGridCategoryText = Color.White;
    //    public static readonly Color PropertyGridHelpBackground = Color.FromArgb(45, 45, 48);
    //    public static readonly Color PropertyGridHelpText = Color.White;

    //    // Viewport specific
    //    public static readonly Color GridLine = Color.FromArgb(60, 60, 60);
    //    public static readonly Color GridMajorLine = Color.FromArgb(80, 80, 80);

    //    // Status and accent colors
    //    public static readonly Color AccentBlue = Color.FromArgb(0, 122, 204);
    //    public static readonly Color SuccessGreen = Color.FromArgb(0, 128, 0);
    //    public static readonly Color WarningYellow = Color.FromArgb(255, 193, 7);
    //    public static readonly Color ErrorRed = Color.FromArgb(220, 53, 69);
    //}

}
