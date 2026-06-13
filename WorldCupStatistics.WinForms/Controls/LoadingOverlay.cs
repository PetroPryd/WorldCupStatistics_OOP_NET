using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.WinForms.Localization;

namespace WorldCupStatistics.WinForms.Controls
{
    internal sealed class LoadingOverlay : Panel
    {
        private readonly Label _label;

        public LoadingOverlay()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(230, 245, 245, 245);
            Visible = false;

            var bar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Width = 240,
                Height = 22
            };
            _label = new Label
            {
                Text = Loc.T("Loading"),
                AutoSize = true,
                Font = new Font(Font.FontFamily, 11, FontStyle.Bold)
            };

            var stack = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Anchor = AnchorStyles.None
            };
            stack.Controls.Add(_label);
            stack.Controls.Add(bar);
            Controls.Add(stack);
            Resize += (_, _) => stack.Location =
                new Point((Width - stack.Width) / 2, (Height - stack.Height) / 2);
        }

        public void Begin(string? message = null)
        {
            _label.Text = message ?? Loc.T("Loading");
            BringToFront();
            Visible = true;
        }

        public void End() => Visible = false;
    }
}
