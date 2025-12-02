// Copyright (c) 0x5BFA. All rights reserved.
// Licensed under the MIT license.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace U5BFA.Libraries
{
	public partial class TrayIconFlyoutIsland : ContentControl
	{
		private void OnContentChanged()
		{
			if (Content is not FrameworkElement newContentAsFrameworkElement)
				return;

			if (MainContentPresenter?.Content is FrameworkElement oldContentAsFrameworkElement)
				oldContentAsFrameworkElement.SizeChanged -= Content_SizeChanged;

			newContentAsFrameworkElement.SizeChanged += Content_SizeChanged;
			MainContentPresenter?.Content = Content;
		}

		private void OnCornerRadiusChanged()
		{
		}
	}
}
