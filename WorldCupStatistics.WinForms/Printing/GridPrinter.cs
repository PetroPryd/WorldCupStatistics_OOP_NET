using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldCupStatistics.WinForms.Printing
{
    internal sealed class GridPrinter : IDisposable
    {
        private readonly string _title;
        private readonly string[] _headers;
        private readonly IReadOnlyList<string[]> _rows;
        private readonly float[] _weights;

        private readonly Font _titleFont = new("Segoe UI", 14, FontStyle.Bold);
        private readonly Font _headerFont = new("Segoe UI", 8.5f, FontStyle.Bold);
        private readonly Font _cellFont = new("Segoe UI", 8.5f, FontStyle.Regular);
        private readonly PrintDocument _doc = new();

        private int _rowIndex;

        public GridPrinter(string title, string[] headers, IReadOnlyList<string[]> rows, float[] weights)
        {
            if (headers.Length != weights.Length)
                throw new ArgumentException($"headers ({headers.Length}) and weights ({weights.Length}) must match.");
            
            _title = title;
            _headers = headers;
            _rows = rows;
            _weights = weights;

            _doc.BeginPrint += (_, _) => _rowIndex = 0;
            _doc.PrintPage += OnPrintPage;
        }

        public void ShowPreview(IWin32Window owner)
        {
            using var preview = new PrintPreviewDialog { Document = _doc, Width = 900, Height = 700 };
            preview.ShowDialog(owner);
        }

        private void OnPrintPage(object? sender, PrintPageEventArgs e)
        {
            var g = e.Graphics!;
            var bounds = e.MarginBounds;
            float x = bounds.Left, y = bounds.Top;

            if (_rowIndex == 0)
            {
                g.DrawString(_title, _titleFont, Brushes.Black, x, y);
                y += _titleFont.GetHeight(g) + 8;
            }

            float totalWeight = _weights.Sum();
            var colX = new float[_headers.Length + 1];
            colX[0] = x;
            for (int i = 0; i < _headers.Length; i++)
                colX[i + 1] = colX[i] + bounds.Width * (_weights[i] / totalWeight);

            for (int i = 0; i < _headers.Length; i++)
                g.DrawString(_headers[i], _headerFont, Brushes.Black, colX[i] + 2, y);
            y += _headerFont.GetHeight(g) + 4;
            g.DrawLine(Pens.Black, x, y, bounds.Right, y);
            y += 4;

            float rowHeight = _cellFont.GetHeight(g) + 6;
            while (_rowIndex < _rows.Count)
            {
                if (y + rowHeight > bounds.Bottom) { e.HasMorePages = true; return; }

                var row = _rows[_rowIndex];
                for (int i = 0; i < _headers.Length; i++)
                {
                    var rect = new RectangleF(colX[i] + 2, y, colX[i + 1] - colX[i] - 4, rowHeight);
                    g.DrawString(i < row.Length ? row[i] : "", _cellFont, Brushes.Black, rect, _cellFormat);
                }
                y += rowHeight;
                _rowIndex++;
            }
            e.HasMorePages = false;
        }

        public void Dispose()
        {
            _titleFont.Dispose();
            _headerFont.Dispose();
            _cellFont.Dispose();
            _cellFormat.Dispose();
            _doc.Dispose();
        }

        private readonly StringFormat _cellFormat = new(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit)
        {
            Trimming = StringTrimming.EllipsisCharacter
        };
    }
}
