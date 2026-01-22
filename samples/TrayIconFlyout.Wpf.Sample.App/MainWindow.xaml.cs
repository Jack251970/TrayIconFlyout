using System.Windows;

namespace U5BFA.Libraries
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private TrayIconFlyout? _trayIconFlyout;

		public MainWindow()
		{
			InitializeComponent();
			InitializeTrayIconFlyout();
		}

		private void InitializeTrayIconFlyout()
		{
			_trayIconFlyout = new TrayIconFlyout
			{
				Width = 360,
				Height = 300
			};

			var island = new TrayIconFlyoutIsland
			{
				Height = 250,
				Content = new System.Windows.Controls.StackPanel
				{
					Margin = new Thickness(16),
					Children =
					{
						new System.Windows.Controls.TextBlock
						{
							Text = "TrayIconFlyout for WPF",
							FontSize = 20,
							FontWeight = FontWeights.Bold,
							Margin = new Thickness(0, 0, 0, 16)
						},
						new System.Windows.Controls.TextBlock
						{
							Text = "This is a flyout that appears from the system tray.",
							TextWrapping = TextWrapping.Wrap,
							Margin = new Thickness(0, 0, 0, 16)
						},
						new System.Windows.Controls.Button
						{
							Content = "Close",
							HorizontalAlignment = HorizontalAlignment.Center,
							Padding = new Thickness(16, 8, 16, 8)
						}
					}
				}
			};

			if (island.Content is System.Windows.Controls.StackPanel panel)
			{
				if (panel.Children[panel.Children.Count - 1] is System.Windows.Controls.Button btn)
				{
					btn.Click += (s, e) => _trayIconFlyout?.Hide();
				}
			}

			_trayIconFlyout.Islands.Add(island);
		}

		private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
		{
			if (_trayIconFlyout?.IsOpen == true)
				_trayIconFlyout?.Hide();
			else
				_trayIconFlyout?.Show();
		}

		protected override void OnClosed(EventArgs e)
		{
			_trayIconFlyout?.Dispose();
			TrayIcon?.Dispose();
			base.OnClosed(e);
		}
	}
}
