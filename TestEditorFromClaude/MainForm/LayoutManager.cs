using App.MainForm.Hierarchy;
using App.MainForm.Library;
using App.MainForm.Properties;
using App.MainForm.Viewport;
using App.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.MainForm
{
    public class LayoutManager
    {
        private SplitContainer mainSplitContainer;
        private SplitContainer leftSplitContainer;
        private SplitContainer rightSplitContainer;

        public Control CreateMainLayout(HierarchyPanel hierarchy, LibraryPanel library,
                                      ViewportPanel viewport, PropertiesPanel properties)
        {
            // Main container that holds everything
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorScheme.PanelBackground
            };

            // Main horizontal split container (left panels | viewport | right panel)
            mainSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BackColor = ColorScheme.PanelBackground,
                Orientation = Orientation.Vertical,
                Panel1MinSize = 100,  // Reduced minimum sizes
                Panel2MinSize = 100,
                SplitterWidth = 5,
                IsSplitterFixed = false
            };

            // Left vertical split container (hierarchy | library)
            leftSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BackColor = ColorScheme.PanelBackground,
                Orientation = Orientation.Horizontal,
                Panel1MinSize = 50,   // Reduced minimum sizes
                Panel2MinSize = 50,
                SplitterWidth = 5,
                IsSplitterFixed = false
            };

            // Right split container (viewport | properties)
            rightSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                BackColor = ColorScheme.PanelBackground,
                Orientation = Orientation.Vertical,
                Panel1MinSize = 100,  // Reduced minimum sizes
                Panel2MinSize = 100,
                SplitterWidth = 5,
                IsSplitterFixed = false
            };

            // Set dock styles for panels
            hierarchy.Dock = DockStyle.Fill;
            library.Dock = DockStyle.Fill;
            viewport.Dock = DockStyle.Fill;
            properties.Dock = DockStyle.Fill;

            // Assemble the layout
            leftSplitContainer.Panel1.Controls.Add(hierarchy);
            leftSplitContainer.Panel2.Controls.Add(library);

            rightSplitContainer.Panel1.Controls.Add(viewport);
            rightSplitContainer.Panel2.Controls.Add(properties);

            mainSplitContainer.Panel1.Controls.Add(leftSplitContainer);
            mainSplitContainer.Panel2.Controls.Add(rightSplitContainer);

            mainContainer.Controls.Add(mainSplitContainer);

            // Handle layout initialization
            mainContainer.HandleCreated += (s, e) =>
            {
                mainContainer.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        InitializeLayout();
                    }
                    catch
                    {
                        // Fallback to default layout if initialization fails
                    }
                }));
            };

            return mainContainer;
        }

        private void InitializeLayout()
        {
            if (mainSplitContainer.Width > 0)
            {
                // Calculate safe initial distances
                int mainSplitDistance = Math.Max(mainSplitContainer.Panel1MinSize,
                    Math.Min(300, mainSplitContainer.Width - mainSplitContainer.Panel2MinSize));

                int leftSplitDistance = Math.Max(leftSplitContainer.Panel1MinSize,
                    Math.Min(leftSplitContainer.Height / 2, leftSplitContainer.Height - leftSplitContainer.Panel2MinSize));

                int rightSplitDistance = Math.Max(rightSplitContainer.Panel1MinSize,
                    Math.Min(rightSplitContainer.Width - 200, rightSplitContainer.Width - rightSplitContainer.Panel2MinSize));

                // Apply the distances
                mainSplitContainer.SplitterDistance = mainSplitDistance;
                leftSplitContainer.SplitterDistance = leftSplitDistance;
                rightSplitContainer.SplitterDistance = rightSplitDistance;

                // Wire up resize handling
                mainSplitContainer.SizeChanged += (s, e) => UpdateSplitterDistances();
            }
        }

        private void UpdateSplitterDistances()
        {
            try
            {
                if (mainSplitContainer.Width <= mainSplitContainer.Panel1MinSize + mainSplitContainer.Panel2MinSize)
                    return;

                int mainSplitDistance = Math.Max(mainSplitContainer.Panel1MinSize,
                    Math.Min(mainSplitContainer.Width / 4, mainSplitContainer.Width - mainSplitContainer.Panel2MinSize));

                int leftSplitDistance = Math.Max(leftSplitContainer.Panel1MinSize,
                    Math.Min(leftSplitContainer.Height / 2, leftSplitContainer.Height - leftSplitContainer.Panel2MinSize));

                int rightSplitDistance = Math.Max(rightSplitContainer.Panel1MinSize,
                    Math.Min((int)(rightSplitContainer.Width * 0.8), rightSplitContainer.Width - rightSplitContainer.Panel2MinSize));

                if (mainSplitContainer.SplitterDistance != mainSplitDistance)
                    mainSplitContainer.SplitterDistance = mainSplitDistance;

                if (leftSplitContainer.SplitterDistance != leftSplitDistance)
                    leftSplitContainer.SplitterDistance = leftSplitDistance;

                if (rightSplitContainer.SplitterDistance != rightSplitDistance)
                    rightSplitContainer.SplitterDistance = rightSplitDistance;
            }
            catch
            {
                // Ignore any layout errors during resize
            }
        }
    }
}
