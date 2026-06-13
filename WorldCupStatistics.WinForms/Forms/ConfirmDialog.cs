using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.WinForms.Localization;

namespace WorldCupStatistics.WinForms.Forms
{
    internal sealed class ConfirmDialog : Form
    {
        private ConfirmDialog(string title, string message)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(360, 130);

            var label = new Label
            {
                Text = message,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(12)
            };

            var yes = new Button { Text = Loc.T("Yes"), DialogResult = DialogResult.Yes, Width = 100 };
            var no = new Button { Text = Loc.T("No"), DialogResult = DialogResult.No, Width = 100 };

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 50,
                Padding = new Padding(10)
            };
            buttons.Controls.Add(no);
            buttons.Controls.Add(yes);

            Controls.Add(label);
            Controls.Add(buttons);

            AcceptButton = yes;   // Enter
            CancelButton = no;    // Esc
        }

        public static bool Ask(IWin32Window? owner, string title, string message)
        {
            using var dialog = new ConfirmDialog(title, message);
            return owner is null ? dialog.ShowDialog() == DialogResult.Yes
                                 : dialog.ShowDialog(owner) == DialogResult.Yes;
        }
    }
}
