// Copyright (c) 0x5BFA. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;
using System.Windows.Controls;

namespace U5BFA.Libraries
{
	internal partial class TrayIconManager : IDisposable
	{
		private static readonly Lazy<TrayIconManager> _default = new(() => new TrayIconManager());
		internal static TrayIconManager Default => _default.Value;

		internal SystemTrayIcon? SystemTrayIcon { get; set; }
		internal TrayIconFlyout? TrayIconFlyout { get; set; }
        // TODO
        /*internal TrayIconMenuFlyout? TrayIconMenuFlyout { get; set; }*/

        private TrayIconManager() { }

		internal void Initialize(SystemTrayIcon trayIcon)
		{
            TrayIconFlyout = new MainTrayIconFlyout();

            var island = new TrayIconFlyoutIsland
            {
                Content = new StackPanel
                {
                    Margin = new Thickness(16),
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "TrayIconFlyout for WPF",
                            FontSize = 20,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 0, 0, 16)
                        },
                        new TextBlock
                        {
                            Text = "This is a flyout that appears from the system tray.",
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(0, 0, 0, 16)
                        },
                        new Button
                        {
                            Content = "Close",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Padding = new Thickness(16, 8, 16, 8)
                        }
                    }
                }
            };

            if (island.Content is StackPanel panel)
            {
                if (panel.Children[panel.Children.Count - 1] is Button btn)
                {
                    btn.Click += (s, e) => TrayIconFlyout?.Hide();
                }
            }

            TrayIconFlyout.Islands.Add(island);

            SystemTrayIcon = trayIcon;
			SystemTrayIcon.Show();
			SystemTrayIcon.LeftClicked += SystemTrayIcon_LeftClicked;
			SystemTrayIcon.RightClicked += SystemTrayIcon_RightClicked;
		}

		private void SystemTrayIcon_LeftClicked(object? sender, MouseEventReceivedEventArgs e)
		{
			if (TrayIconFlyout is null)
				return;

			if (TrayIconFlyout.IsOpen)
				TrayIconFlyout.Hide();
			else
				TrayIconFlyout.Show();
		}

		private void SystemTrayIcon_RightClicked(object? sender, MouseEventReceivedEventArgs e)
		{
            // TODO
            /*if (TrayIconMenuFlyout is null)
				return;

			if (TrayIconMenuFlyout.IsOpen)
				TrayIconMenuFlyout.Hide();

			TrayIconMenuFlyout.Show(new(e.Point.X, e.Point.Y - 32));*/
        }

        public void Dispose()
		{
			SystemTrayIcon?.LeftClicked -= SystemTrayIcon_LeftClicked;
			SystemTrayIcon?.RightClicked -= SystemTrayIcon_RightClicked;
			SystemTrayIcon?.Destroy();
			TrayIconFlyout?.Dispose();
		}
	}
}
