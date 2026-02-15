// Copyright (c) Jack251970. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;
using System.Windows.Controls;
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

		private static readonly KeySpline OpenAnimationKeySpline = new(0.1, 0.9, 0.4, 1.0);
		private static readonly KeySpline CloseAnimationKeySpline = new(0.2, 0.0, 0.9, 0.0);

		private Window? _host;
		private bool _isPopupAnimationPlaying;
		private Grid? RootGrid;
		private Grid? IslandsGrid;

		public bool IsOpen { get; private set; }

		public TrayIconFlyout() : this(new Window
        {
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            Topmost = true,
            AllowsTransparency = true,
            Background = Brushes.Transparent
        })
		{

		}

        public TrayIconFlyout(Window host)
        {
            DefaultStyleKey = typeof(TrayIconFlyout);
            _host = host;
			_host.Content = this;
            host.Deactivated += HostWindow_Deactivated;
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

            // Ensure template is applied before accessing template parts
            ApplyTemplate();

            if (RootGrid is null)
                throw new InvalidOperationException($"Template part {PART_RootGrid} is missing. Ensure the control template is correctly defined.");

            _isPopupAnimationPlaying = true;

            UpdateLayout();

            UpdateBackdrop();

            UpdateFlyoutRegion();

            UpdateLayout();

            _host.Show();

			RootGrid.Visibility = Visibility.Visible;

            if (IsTransitionAnimationEnabled)
			{
				AnimateShow();
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

			RootGrid.Visibility = Visibility.Visible;

            if (IsTransitionAnimationEnabled)
			{
				AnimateHide();
			}
			else
			{
				_host.Hide();
				IsOpen = false;
				_isPopupAnimationPlaying = false;
			}
		}

		private void UpdateFlyoutRegion()
		{
			if (_host is null)
				return;

			// Get the working area of the primary screen
			var workingArea = SystemParameters.WorkArea;

			// Position at bottom-right corner (near system tray)
			_host.Left = workingArea.Right - Width;
			_host.Top = workingArea.Bottom - Height;
			_host.Width = Width;
			_host.Height = Height;
		}

        private void UpdateBackdrop()
        {
            var isLightTheme = GeneralHelpers.IsTaskbarLight();
            var isColorEnabled = GeneralHelpers.IsTaskbarColorPrevalenceEnabled();

            foreach (var island in Islands)
                island.UpdateBackdrop(isLightTheme, isColorEnabled);
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
				for (var index = 0; index < Islands.Count; index++)
				{
					if (Islands[index] is not TrayIconFlyoutIsland island)
						continue;

					IslandsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
					Grid.SetRow(island, index * 2);
					Grid.SetColumn(island, 0);
					island.SetOwner(this);
                    IslandsGrid.Children.Add(island);

					if (index != Islands.Count - 1)
						IslandsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(12) });
				}
            }
			else
			{
				for (var index = 0; index < Islands.Count; index++)
				{
					if (Islands[index] is not TrayIconFlyoutIsland island)
						continue;

					IslandsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
					Grid.SetRow(island, 0);
					Grid.SetColumn(island, index * 2);
					island.SetOwner(this);
                    IslandsGrid.Children.Add(island);

					if (index != Islands.Count - 1)
						IslandsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
                }
            }
		}

		private TranslateTransform EnsureTranslateTransform()
		{
			if (RootGrid is null)
				throw new InvalidOperationException($"{nameof(RootGrid)} is null");

			if (RootGrid.RenderTransform is not TranslateTransform translateTransform)
			{
				translateTransform = new TranslateTransform();
				RootGrid.RenderTransform = translateTransform;
			}

			return translateTransform;
		}

		private void ResetInactiveAxis(TranslateTransform translateTransform, Orientation direction)
		{
			if (direction is Orientation.Vertical)
			{
				translateTransform.BeginAnimation(TranslateTransform.XProperty, null);
				translateTransform.X = 0;
			}
			else
			{
				translateTransform.BeginAnimation(TranslateTransform.YProperty, null);
				translateTransform.Y = 0;
			}
		}

		private void AnimateShow()
		{
			if (RootGrid is null)
				return;

			// Ensure RenderTransform is set
			var translateTransform = EnsureTranslateTransform();

			// Get animation parameters based on popup direction
			var (animationProperty, fromValue, toValue, duration) = PopupDirection switch
			{
				Orientation.Vertical => (
					TranslateTransform.YProperty,
					ActualHeight,
					0.0,
					TimeSpan.FromMilliseconds(267)
				),
				_ => (
					TranslateTransform.XProperty,
					ActualWidth,
					0.0,
					TimeSpan.FromMilliseconds(167)
				)
			};

			// Reset the other axis
			ResetInactiveAxis(translateTransform, PopupDirection);

			// Create the keyframe animation
			var keyFrames = new DoubleAnimationUsingKeyFrames
			{
				Duration = duration
			};

			// Add discrete keyframe for initial position
			keyFrames.KeyFrames.Add(new DiscreteDoubleKeyFrame
			{
				KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
				Value = fromValue
			});

			// Add spline keyframe for smooth animation
			keyFrames.KeyFrames.Add(new SplineDoubleKeyFrame
			{
				KeySpline = OpenAnimationKeySpline,
				KeyTime = duration,
				Value = toValue
			});

			// Create and configure the storyboard
			var storyboard = new Storyboard();
			Storyboard.SetTarget(keyFrames, translateTransform);
			Storyboard.SetTargetProperty(keyFrames, new PropertyPath(animationProperty));
			storyboard.Children.Add(keyFrames);

			// Subscribe to completion event
			storyboard.Completed += OpenAnimationStoryboard_Completed;

			// Start the animation
			storyboard.Begin();
		}

		private void AnimateHide()
		{
			if (RootGrid is null)
				return;

			// Ensure RenderTransform is set
			var translateTransform = EnsureTranslateTransform();

			// Get animation parameters based on popup direction
			var (animationProperty, fromValue, toValue, duration) = PopupDirection switch
			{
				Orientation.Vertical => (
					TranslateTransform.YProperty,
					0.0,
					ActualHeight,
					TimeSpan.FromMilliseconds(200)
				),
				_ => (
					TranslateTransform.XProperty,
					0.0,
					ActualWidth,
					TimeSpan.FromMilliseconds(167)
				)
			};

			// Reset the other axis
			ResetInactiveAxis(translateTransform, PopupDirection);

			// Create the keyframe animation for translate transform
			var translateKeyFrames = new DoubleAnimationUsingKeyFrames
			{
				Duration = duration
			};

			// Add discrete keyframe for initial position
			translateKeyFrames.KeyFrames.Add(new DiscreteDoubleKeyFrame
			{
				KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
				Value = fromValue
			});

			// Add spline keyframe for smooth animation
			translateKeyFrames.KeyFrames.Add(new SplineDoubleKeyFrame
			{
				KeySpline = CloseAnimationKeySpline,
				KeyTime = duration,
				Value = toValue
			});

			// Create visibility animation to collapse at the end
			var visibilityAnimation = new ObjectAnimationUsingKeyFrames();
			visibilityAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame
			{
				KeyTime = duration,
				Value = Visibility.Collapsed
			});

			// Create and configure the storyboard
			var storyboard = new Storyboard();
			
			Storyboard.SetTarget(translateKeyFrames, translateTransform);
			Storyboard.SetTargetProperty(translateKeyFrames, new PropertyPath(animationProperty));
			storyboard.Children.Add(translateKeyFrames);

			Storyboard.SetTarget(visibilityAnimation, RootGrid);
			Storyboard.SetTargetProperty(visibilityAnimation, new PropertyPath(UIElement.VisibilityProperty));
			storyboard.Children.Add(visibilityAnimation);

			// Subscribe to completion event
			storyboard.Completed += CloseAnimationStoryboard_Completed;

			// Start the animation
			storyboard.Begin();
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
