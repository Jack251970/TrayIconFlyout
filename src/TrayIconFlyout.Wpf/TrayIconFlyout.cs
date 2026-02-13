// Copyright (c) Jack251970. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Markup;

namespace U5BFA.Libraries
{
	/// <summary>
	/// A flyout control for WPF that appears from the system tray.
	/// </summary>
	[ContentProperty(nameof(Islands))]
	public partial class TrayIconFlyout : Control, IDisposable
	{
		private const string PART_RootGrid = "PART_RootGrid";
		private const string PART_IslandsGrid = "PART_IslandsGrid";

		private Window? _host;
		private bool _isPopupAnimationPlaying;
		private Grid? RootGrid;
		private Grid? IslandsGrid;

		public bool IsOpen { get; private set; }

		public TrayIconFlyout()
		{
			DefaultStyleKey = typeof(TrayIconFlyout);
			InitializeHostWindow();
		}

		private void InitializeHostWindow()
		{
			_host = new Window
			{
				WindowStyle = WindowStyle.None,
				ResizeMode = ResizeMode.NoResize,
				ShowInTaskbar = false,
				Topmost = true,
				AllowsTransparency = true,
				Background = Brushes.Transparent,
				Content = this
			};

			_host.Deactivated += HostWindow_Deactivated;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			RootGrid = GetTemplateChild(PART_RootGrid) as Grid
				?? throw new InvalidOperationException($"Could not find {PART_RootGrid} in the given {nameof(TrayIconFlyout)}'s style.");
			IslandsGrid = GetTemplateChild(PART_IslandsGrid) as Grid
				?? throw new InvalidOperationException($"Could not find {PART_IslandsGrid} in the given {nameof(TrayIconFlyout)}'s style.");

			UpdateIslands();
		}

		public void Show()
		{
			if (_host is null || _isPopupAnimationPlaying)
				return;

			_isPopupAnimationPlaying = true;

			// Ensure template is applied before accessing template parts
			ApplyTemplate();
			
			if (RootGrid is null)
				return;

			// Position the window near the system tray
			PositionWindow();

			UpdateLayout();

			// Ensure to hide first
			if (RootGrid.RenderTransform is TranslateTransform translateTransform)
			{
				if (PopupDirection is Orientation.Vertical)
					translateTransform.Y = DesiredSize.Height;
				else
					translateTransform.X = DesiredSize.Width;
			}

			_host.Show();

			if (IsTransitionAnimationEnabled)
			{
				var storyboard = PopupDirection is Orientation.Vertical
					? GetBottomToTopTransitionStoryboard(RootGrid, (int)DesiredSize.Height, 0)
					: GetRightToLeftTransitionStoryboard(RootGrid, (int)DesiredSize.Width, 0);
				storyboard.Completed += OpenAnimationStoryboard_Completed;
				storyboard.Begin();
			}
			else
			{
				IsOpen = true;
				_isPopupAnimationPlaying = false;
			}
		}

		public void Hide()
		{
			if (_host is null || RootGrid is null || _isPopupAnimationPlaying)
				return;

			_isPopupAnimationPlaying = true;

			if (IsTransitionAnimationEnabled)
			{
				var storyboard = PopupDirection is Orientation.Vertical
					? GetTopToBottomTransitionStoryboard(RootGrid, 0, (int)DesiredSize.Height)
					: GetLeftToRightTransitionStoryboard(RootGrid, 0, (int)DesiredSize.Width);
				storyboard.Completed += CloseAnimationStoryboard_Completed;
				storyboard.Begin();
			}
			else
			{
				_host.Hide();
				IsOpen = false;
				_isPopupAnimationPlaying = false;
			}
		}

		private void PositionWindow()
		{
			if (_host is null)
				return;

			// Get the working area of the primary screen
			var workingArea = SystemParameters.WorkArea;

			// Position at bottom-right corner (near system tray)
			_host.Left = workingArea.Right - Width - 12;
			_host.Top = workingArea.Bottom - Height - 12;
			_host.Width = Width;
			_host.Height = Height;
		}

		private void UpdateIslands()
		{
			if (IslandsGrid is null)
				return;

			IslandsGrid.Children.Clear();
			IslandsGrid.RowDefinitions.Clear();
			IslandsGrid.ColumnDefinitions.Clear();

			if (IslandsOrientation is Orientation.Vertical)
			{
				for (int index = 0; index < Islands.Count; index++)
				{
					if (Islands[index] is not TrayIconFlyoutIsland island)
						continue;

					IslandsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
					Grid.SetRow(island, index);
					Grid.SetColumn(island, 0);
					island.SetOwner(this);
					IslandsGrid.Children.Add(island);
				}
			}
			else
			{
				for (int index = 0; index < Islands.Count; index++)
				{
					if (Islands[index] is not TrayIconFlyoutIsland island)
						continue;

					IslandsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
					Grid.SetRow(island, 0);
					Grid.SetColumn(island, index);
					island.SetOwner(this);
					IslandsGrid.Children.Add(island);
				}
			}
		}

		private Storyboard GetBottomToTopTransitionStoryboard(FrameworkElement target, int from, int to)
		{
			var animation = new DoubleAnimation
			{
				From = from,
				To = to,
				Duration = TimeSpan.FromMilliseconds(250),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
			};

			Storyboard.SetTarget(animation, target);
			Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

			var storyboard = new Storyboard();
			storyboard.Children.Add(animation);
			return storyboard;
		}

		private Storyboard GetTopToBottomTransitionStoryboard(FrameworkElement target, int from, int to)
		{
			return GetBottomToTopTransitionStoryboard(target, from, to);
		}

		private Storyboard GetRightToLeftTransitionStoryboard(FrameworkElement target, int from, int to)
		{
			var animation = new DoubleAnimation
			{
				From = from,
				To = to,
				Duration = TimeSpan.FromMilliseconds(250),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
			};

			Storyboard.SetTarget(animation, target);
			Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

			var storyboard = new Storyboard();
			storyboard.Children.Add(animation);
			return storyboard;
		}

		private Storyboard GetLeftToRightTransitionStoryboard(FrameworkElement target, int from, int to)
		{
			return GetRightToLeftTransitionStoryboard(target, from, to);
		}

		private void OpenAnimationStoryboard_Completed(object? sender, EventArgs e)
		{
			if (sender is not Storyboard storyboard)
				return;

			storyboard.Completed -= OpenAnimationStoryboard_Completed;
			_isPopupAnimationPlaying = false;
			IsOpen = true;
		}

		private void CloseAnimationStoryboard_Completed(object? sender, EventArgs e)
		{
			if (sender is not Storyboard storyboard)
				return;

			storyboard.Completed -= CloseAnimationStoryboard_Completed;
			_isPopupAnimationPlaying = false;
			IsOpen = false;
			_host?.Hide();
		}

		private void HostWindow_Deactivated(object? sender, EventArgs e)
		{
			if (HideOnLostFocus)
				Hide();
		}

		public void Dispose()
		{
			if (_host is not null)
			{
				_host.Deactivated -= HostWindow_Deactivated;
				_host.Close();
				_host = null;
			}

			GC.SuppressFinalize(this);
		}
	}
}
