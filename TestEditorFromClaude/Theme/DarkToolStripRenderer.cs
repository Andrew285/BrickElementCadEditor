using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Theme
{
    public class DarkToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            var button = e.Item as ToolStripButton;

            if (button?.Checked == true)
            {
                e.Graphics.FillRectangle(new SolidBrush(ColorScheme.AccentBlue), e.Item.Bounds);
            }
            else if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(ColorScheme.ButtonHover), e.Item.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(ColorScheme.ToolbarBackground), e.Item.Bounds);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(ColorScheme.ToolbarBackground), e.AffectedBounds);
        }
    }
}
