// Copyright (c) Jack251970. All rights reserved.
// Licensed under the MIT license.

using iNKORE.UI.WPF.Modern.Common;
using System;
using System.Windows;

namespace U5BFA.Libraries
{
	public partial class App : Application
	{
        private Window? _window;

        public App()
        {
            ShadowAssist.UseBitmapCache = false;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            TrayIconManager.Default.Initialize(new(
                "Assets\\Tray.ico",
                "TrayIconFlyout sample app (WPF)",
                new("056EACEE-82B0-48AC-A6E9-34DAE5CD37F3")));

            _window = new MainWindow();
            _window.Show();

            RegisterExitEvents();
        }

        private void RegisterExitEvents()
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Dispose();
            };

            Current.Exit += (s, e) =>
            {
                Dispose();
            };

            Current.SessionEnding += (s, e) =>
            {
                Dispose();
            };
        }

        private void Dispose()
        {
            TrayIconManager.Default.Dispose();
        }
    }
}
