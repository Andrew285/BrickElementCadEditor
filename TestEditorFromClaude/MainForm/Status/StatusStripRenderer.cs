using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Theme;

namespace App.MainForm.Status
{
    public class StatusStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(ColorScheme.AccentBlue), e.AffectedBounds);
        }
    }
}
