// Copyright (c) Jack251970. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;
using System.Windows.Controls;

namespace U5BFA.Libraries
{
	/// <summary>
	/// Represents a content island within a TrayIconFlyout for WPF.
	/// </summary>
	public partial class TrayIconFlyoutIsland : ContentControl
	{
		private const string PART_RootGrid = "PART_RootGrid";
		private const string PART_MainContentPresenter = "PART_MainContentPresenter";

		private Grid? RootGrid;
		private ContentPresenter? MainContentPresenter;

		private WeakReference<TrayIconFlyout>? _owner;

		public TrayIconFlyoutIsland()
		{
			DefaultStyleKey = typeof(TrayIconFlyoutIsland);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			RootGrid = GetTemplateChild(PART_RootGrid) as Grid
				?? throw new InvalidOperationException($"Could not find {PART_RootGrid} in the given {nameof(TrayIconFlyoutIsland)}'s style.");
			MainContentPresenter = GetTemplateChild(PART_MainContentPresenter) as ContentPresenter
				?? throw new InvalidOperationException($"Could not find {PART_MainContentPresenter} in the given {nameof(TrayIconFlyoutIsland)}'s style.");

			Unloaded += TrayIconFlyoutIsland_Unloaded;
		}

		internal void SetOwner(TrayIconFlyout owner)
		{
			_owner = new(owner);
		}

		private void TrayIconFlyoutIsland_Unloaded(object sender, RoutedEventArgs e)
		{
			Unloaded -= TrayIconFlyoutIsland_Unloaded;
		}
	}
}
