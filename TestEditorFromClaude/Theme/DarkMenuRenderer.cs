using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Theme
{
    public class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(ColorScheme.AccentBlue), e.Item.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(ColorScheme.MenuBackground), e.Item.Bounds);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(ColorScheme.MenuBackground), e.AffectedBounds);
        }
    }
}
