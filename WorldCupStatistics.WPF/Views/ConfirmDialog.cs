using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WorldCupStatistics.WPF.Localization;

namespace WorldCupStatistics.WPF.Views
{
    public sealed class ConfirmDialog : Window
    {
        public static bool Ask(Window? owner, string title, string message)
        {
            var dialog = new ConfirmDialog(title, message);

            if (owner is not null)
            {
                try { dialog.Owner = owner; }
                catch (InvalidOperationException) {  }
            }

            if (dialog.Owner is null)
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            return dialog.ShowDialog() == true;
        }

        private ConfirmDialog(string title, string message)
        {
            Title = title;
            Width = 360;
            SizeToContent = SizeToContent.Height;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var text = new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(16) };

            var yes = new Button { Content = Loc.Localization_Yes(), MinWidth = 90, IsDefault = true, Margin = new Thickness(0, 0, 8, 0) };
            var no = new Button { Content = Loc.Localization_No(), MinWidth = 90, IsCancel = true };
            yes.Click += (_, _) => { DialogResult = true; };

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(16, 0, 16, 16)
            };
            buttons.Children.Add(yes);
            buttons.Children.Add(no);

            var root = new StackPanel();
            root.Children.Add(text);
            root.Children.Add(buttons);
            Content = root;
        }
    }
}
