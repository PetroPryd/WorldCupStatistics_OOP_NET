using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.DataLayer.Settings;
using WorldCupStatistics.WinForms.Localization;

namespace WorldCupStatistics.WinForms.Forms
{
    internal sealed class SettingsForm : Form
    {
        private readonly ComboBox _genderCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
        private readonly ComboBox _languageCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };

        public AppSettings Result { get; }

        public SettingsForm(AppSettings current)
        {
            // Work on a copy so Cancel leaves the original untouched.
            Result = new AppSettings
            {
                Gender = current.Gender,
                Language = current.Language,
                FavoriteTeamFifaCode = current.FavoriteTeamFifaCode,
                FavoritePlayerNumbers = [.. current.FavoritePlayerNumbers],
                DisplayMode = current.DisplayMode,
                WindowWidth = current.WindowWidth,
                WindowHeight = current.WindowHeight
            };

            Text = Loc.T("SettingsTitle");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(340, 180);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(15),
                AutoSize = true
            };
            layout.Controls.Add(new Label { Text = Loc.T("GenderLabel"), AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            layout.Controls.Add(_genderCombo, 1, 0);
            layout.Controls.Add(new Label { Text = Loc.T("LanguageLabel"), AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            layout.Controls.Add(_languageCombo, 1, 1);

            // Gender items carry their enum value; display text is localized.
            _genderCombo.DisplayMember = nameof(ComboItem<Gender>.Text);
            _genderCombo.Items.Add(new ComboItem<Gender>(Loc.T("Men"), Gender.Men));
            _genderCombo.Items.Add(new ComboItem<Gender>(Loc.T("Women"), Gender.Women));
            _genderCombo.SelectedIndex = current.Gender == Gender.Women ? 1 : 0;

            _languageCombo.DisplayMember = nameof(ComboItem<string>.Text);
            _languageCombo.Items.Add(new ComboItem<string>(Loc.T("English"), "en"));
            _languageCombo.Items.Add(new ComboItem<string>(Loc.T("Croatian"), "hr"));
            _languageCombo.SelectedIndex = current.Language == "hr" ? 1 : 0;

            var confirm = new Button { Text = Loc.T("Confirm"), Width = 110 };
            var cancel = new Button { Text = Loc.T("Cancel"), Width = 110 };
            confirm.Click += OnConfirm;
            cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 55,
                Padding = new Padding(12)
            };
            buttons.Controls.Add(cancel);
            buttons.Controls.Add(confirm);

            Controls.Add(layout);
            Controls.Add(buttons);

            AcceptButton = confirm;   // Enter
            CancelButton = cancel;    // Esc
        }

        private void OnConfirm(object? sender, EventArgs e)
        {
            if (!ConfirmDialog.Ask(this, Loc.T("SettingsTitle"), Loc.T("ConfirmSettingsMessage")))
                return;   // user said No → stay on the form

            Result.Gender = ((ComboItem<Gender>)_genderCombo.SelectedItem!).Value;
            Result.Language = ((ComboItem<string>)_languageCombo.SelectedItem!).Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>Small typed wrapper so a ComboBox item shows localized text but carries a real value.</summary>
        private sealed record ComboItem<T>(string Text, T Value)
        {
            public override string ToString() => Text;
        }
    }
}
