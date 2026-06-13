using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCupStatistics.DataLayer.Models;
using WorldCupStatistics.WinForms.Localization;

namespace WorldCupStatistics.WinForms.Controls
{
    internal sealed class PlayerControl : UserControl
    {
        private static readonly Color DefaultColor = Color.White;
        private static readonly Color SelectedColor = Color.FromArgb(204, 229, 255);

        private readonly PictureBox _picture = new();
        private readonly Label _nameLabel = new();
        private readonly Label _detailLabel = new();
        private readonly Label _captainLabel = new();
        private readonly Label _starLabel = new();

        /// <summary>Raised for a mouse press anywhere on the card (incl. its child controls).</summary>
        public event MouseEventHandler? CardMouseDown;
        /// <summary>Raised for mouse movement anywhere on the card.</summary>
        public event MouseEventHandler? CardMouseMove;

        public Player Player { get; }
        public int ShirtNumber => Player.ShirtNumber;

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set { _isFavorite = value; _starLabel.Visible = value; }
        }

        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set { _selected = value; BackColor = value ? SelectedColor : DefaultColor; }
        }

        public PlayerControl(Player player, string imagePath, bool isFavorite)
        {
            Player = player;

            Width = 230;
            Height = 76;
            Margin = new Padding(6);
            BackColor = DefaultColor;
            BorderStyle = BorderStyle.FixedSingle;

            _picture.Size = new Size(60, 60);
            _picture.Location = new Point(8, 8);
            _picture.SizeMode = PictureBoxSizeMode.Zoom;
            _picture.Image = ImageLoader.Load(imagePath);

            _nameLabel.Text = player.Name;
            _nameLabel.Font = new Font(Font.FontFamily, 9.5f, FontStyle.Bold);
            _nameLabel.Location = new Point(78, 8);
            _nameLabel.AutoSize = true;
            _nameLabel.MaximumSize = new Size(140, 0);

            _detailLabel.Text = $"#{player.ShirtNumber}  {player.Position}";
            _detailLabel.Location = new Point(78, 32);
            _detailLabel.AutoSize = true;
            _detailLabel.ForeColor = Color.DimGray;

            _captainLabel.Text = Loc.T("Captain");
            _captainLabel.Location = new Point(78, 52);
            _captainLabel.AutoSize = true;
            _captainLabel.ForeColor = Color.SaddleBrown;
            _captainLabel.Font = new Font(Font.FontFamily, 8f, FontStyle.Bold);
            _captainLabel.Visible = player.IsCaptain;

            _starLabel.Text = "★";
            _starLabel.Font = new Font(Font.FontFamily, 14f, FontStyle.Bold);
            _starLabel.ForeColor = Color.Goldenrod;
            _starLabel.Location = new Point(202, 6);
            _starLabel.AutoSize = true;
            _starLabel.Visible = isFavorite;
            _isFavorite = isFavorite;

            Controls.Add(_picture);
            Controls.Add(_nameLabel);
            Controls.Add(_detailLabel);
            Controls.Add(_captainLabel);
            Controls.Add(_starLabel);

            // Forward children's input to the card so the whole thing acts as one unit.
            foreach (Control child in new Control[] { _picture, _nameLabel, _detailLabel, _captainLabel, _starLabel })
            {
                child.Click += (_, e) => OnClick(e);
                child.MouseDown += (_, e) => CardMouseDown?.Invoke(this, e);
                child.MouseMove += (_, e) => CardMouseMove?.Invoke(this, e);
            }
            MouseDown += (_, e) => CardMouseDown?.Invoke(this, e);
            MouseMove += (_, e) => CardMouseMove?.Invoke(this, e);
        }

        /// <summary>Attaches the same context menu to the card and all its children.</summary>
        public void SetContextMenu(ContextMenuStrip menu)
        {
            ContextMenuStrip = menu;
            foreach (Control child in Controls)
                child.ContextMenuStrip = menu;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _picture.Image?.Dispose();
            base.Dispose(disposing);
        }
    }
}
