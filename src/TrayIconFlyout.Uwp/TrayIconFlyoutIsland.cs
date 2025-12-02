// Copyright (c) 0x5BFA. All rights reserved.
// Licensed under the MIT license.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;

namespace U5BFA.Libraries
{
	public partial class TrayIconFlyoutIsland : ContentControl
	{
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			RootGrid = GetTemplateChild(PART_RootGrid) as Grid
				?? throw new MissingFieldException($"Could not find {PART_RootGrid} in the given {nameof(TrayIconFlyoutIsland)}'s style.");
			BackdropTargetGrid = GetTemplateChild(PART_BackdropTargetGrid) as Grid
				?? throw new MissingFieldException($"Could not find {PART_BackdropTargetGrid} in the given {nameof(TrayIconFlyoutIsland)}'s style.");
			MainContentPresenter = GetTemplateChild(PART_MainContentPresenter) as ContentPresenter
				?? throw new MissingFieldException($"Could not find {PART_MainContentPresenter} in the given {nameof(TrayIconFlyoutIsland)}'s style.");

			_propertyChangedCallbackTokenForContentProperty = RegisterPropertyChangedCallback(ContentProperty, (s, e) => ((TrayIconFlyoutIsland)s).OnContentChanged());
			_propertyChangedCallbackTokenForCornerRadiusProperty = RegisterPropertyChangedCallback(CornerRadiusProperty, (s, e) => ((TrayIconFlyoutIsland)s).OnCornerRadiusChanged());

			Unloaded += TrayIconFlyoutIsland_Unloaded;
		}

		private void TrayIconFlyoutIsland_Unloaded(object sender, RoutedEventArgs e)
		{
			Unloaded -= TrayIconFlyoutIsland_Unloaded;

			UnregisterPropertyChangedCallback(ContentProperty, _propertyChangedCallbackTokenForContentProperty);
			UnregisterPropertyChangedCallback(CornerRadiusProperty, _propertyChangedCallbackTokenForCornerRadiusProperty);
		}

		internal void SetOwner(TrayIconFlyout owner)
		{
			_owner = new(owner);
		}

		private void Content_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}
	}
}
