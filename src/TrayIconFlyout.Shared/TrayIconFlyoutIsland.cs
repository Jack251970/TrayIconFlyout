// Copyright (c) 0x5BFA. All rights reserved.
// Licensed under the MIT license.

using System;

#if UWP
using Windows.UI.Xaml.Controls;
#elif WASDK
using Microsoft.UI.Xaml.Controls;
#endif

namespace U5BFA.Libraries
{
	public partial class TrayIconFlyoutIsland
	{
		private const string PART_RootGrid = "PART_RootGrid";
		private const string PART_BackdropTargetGrid = "PART_BackdropTargetGrid";
		private const string PART_MainContentPresenter = "PART_MainContentPresenter";

		private Grid? RootGrid;
		private Grid? BackdropTargetGrid;
		private ContentPresenter? MainContentPresenter;

		private WeakReference<TrayIconFlyout>? _owner;
		private bool _isBackdropLinkAttached;
		private long _propertyChangedCallbackTokenForContentProperty;
		private long _propertyChangedCallbackTokenForCornerRadiusProperty;

		public TrayIconFlyoutIsland()
		{
			DefaultStyleKey = typeof(TrayIconFlyoutIsland);
		}
	}
}
