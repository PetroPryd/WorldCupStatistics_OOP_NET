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

        public event MouseEventHandler? CardMouseDown;
        public event MouseEventHandler? CardMouseMove;
        public event Action<PlayerControl>? SetImageRequested;

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
            Height = 84;
            Margin = new Padding(6);
            BackColor = DefaultColor;
            BorderStyle = BorderStyle.FixedSingle;

            if (player.IsCaptain)
            {
                BorderStyle = BorderStyle.None;
                Padding = new Padding(2);
            }

            _picture.Size = new Size(60, 60);
            _picture.Location = new Point(8, 8);
            _picture.SizeMode = PictureBoxSizeMode.Zoom;
            _picture.Image = ImageLoader.Load(imagePath);

            var textStack = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Location = new Point(78, 8),
                Size = new Size(148, 64),
                Margin = new Padding(0)
            };

            _nameLabel.Text = player.Name;
            _nameLabel.Font = new Font(Font.FontFamily, 9.5f, FontStyle.Bold);
            _nameLabel.AutoSize = true;
            _nameLabel.MaximumSize = new Size(148, 0);
            _nameLabel.Margin = new Padding(0);

            _detailLabel.Text = $"#{player.ShirtNumber}  {player.Position}";
            _detailLabel.AutoSize = true;
            _detailLabel.ForeColor = Color.DimGray;
            _detailLabel.Margin = new Padding(0, 2, 0, 0);

            _captainLabel.Text = Loc.T("Captain");
            _captainLabel.AutoSize = true;
            _captainLabel.ForeColor = Color.SaddleBrown;
            _captainLabel.Font = new Font(Font.FontFamily, 8f, FontStyle.Bold);
            _captainLabel.Visible = player.IsCaptain;
            _captainLabel.Margin = new Padding(0, 2, 0, 0);

            textStack.Controls.Add(_nameLabel);
            textStack.Controls.Add(_detailLabel);
            textStack.Controls.Add(_captainLabel);

            _starLabel.Text = "★";
            _starLabel.Font = new Font(Font.FontFamily, 12f, FontStyle.Bold);
            _starLabel.ForeColor = Color.Goldenrod;
            _starLabel.Location = new Point(196, 8);
            _starLabel.AutoSize = true;
            _starLabel.Visible = isFavorite;
            _isFavorite = isFavorite;

            Controls.Add(_picture);
            Controls.Add(textStack);
            Controls.Add(_starLabel);


            foreach (Control child in new Control[] { _picture, textStack, _nameLabel, _detailLabel, _captainLabel, _starLabel })
            {
                child.Click += (_, e) => OnClick(e);
                child.MouseDown += (_, e) => CardMouseDown?.Invoke(this, e);
                child.MouseMove += (_, e) => CardMouseMove?.Invoke(this, e);
            }
            MouseDown += (_, e) => CardMouseDown?.Invoke(this, e);
            MouseMove += (_, e) => CardMouseMove?.Invoke(this, e);
        }

        public void ReloadImage(string imagePath)
        {
            var old = _picture.Image;
            _picture.Image = ImageLoader.Load(imagePath);
            old?.Dispose();
        }

        public void SetContextMenu(ContextMenuStrip menu)
        {
            ContextMenuStrip = menu;
            AttachMenu(this, menu);
        }

        private static void AttachMenu(Control parent, ContextMenuStrip menu)
        {
            foreach (Control child in parent.Controls)
            {
                child.ContextMenuStrip = menu;
                AttachMenu(child, menu);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _picture.Image?.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!Player.IsCaptain) return;

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            for (int i = 3; i >= 1; i--)
            {
                using var glowPen = new Pen(Color.FromArgb(40, 255, 200, 0), i * 2);
                g.DrawRectangle(glowPen, i, i, Width - 1 - i * 2, Height - 1 - i * 2);
            }

            using var goldPen = new Pen(Color.Goldenrod, 2);
            g.DrawRectangle(goldPen, 1, 1, Width - 3, Height - 3);
        }
    }
}
