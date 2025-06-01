using App.Theme;
using BrickElementCadEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.MainForm.Status
{
    public class StatusStripManager
    {
        public StatusStrip StatusStrip { get; private set; }

        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar progressBar;
        private ToolStripStatusLabel coordinatesLabel;
        private ToolStripStatusLabel selectionLabel;
        private ToolStripStatusLabel modeLabel;

        public StatusStripManager()
        {
            CreateStatusStrip();
            CreateStatusItems();
            ApplyTheme();
        }

        private void CreateStatusStrip()
        {
            StatusStrip = new StatusStrip
            {
                BackColor = ColorScheme.AccentBlue,
                ForeColor = Color.White,
                Dock = DockStyle.Bottom
            };
        }

        private void CreateStatusItems()
        {
            // Main status message
            statusLabel = new ToolStripStatusLabel("Ready")
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Progress bar (hidden by default)
            progressBar = new ToolStripProgressBar
            {
                Visible = false,
                Size = new Size(200, 16)
            };

            // Mouse coordinates
            coordinatesLabel = new ToolStripStatusLabel("X: 0, Y: 0, Z: 0")
            {
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                AutoSize = false,
                Width = 120
            };

            // Selection info
            selectionLabel = new ToolStripStatusLabel("Nothing selected")
            {
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                AutoSize = false,
                Width = 150
            };

            // Current mode/tool
            modeLabel = new ToolStripStatusLabel("Select")
            {
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                AutoSize = false,
                Width = 80
            };

            StatusStrip.Items.AddRange(new ToolStripItem[]
            {
                statusLabel,
                progressBar,
                coordinatesLabel,
                selectionLabel,
                modeLabel
            });
        }

        private void ApplyTheme()
        {
            StatusStrip.Renderer = new StatusStripRenderer();
        }

        public void UpdateStatus(string message)
        {
            statusLabel.Text = message;
        }

        public void SetProgress(int percentage)
        {
            if (percentage >= 0 && percentage <= 100)
            {
                progressBar.Value = percentage;
                progressBar.Visible = percentage > 0 && percentage < 100;
            }
            else
            {
                progressBar.Visible = false;
            }
        }

        public void UpdateCoordinates(float x, float y, float z)
        {
            coordinatesLabel.Text = $"X: {x:F2}, Y: {y:F2}, Z: {z:F2}";
        }

        public void UpdateSelection(string selectionInfo)
        {
            selectionLabel.Text = selectionInfo;
        }

        public void UpdateMode(string mode)
        {
            modeLabel.Text = mode;
        }
    }
}
