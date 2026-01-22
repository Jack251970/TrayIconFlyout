// Copyright (c) Jack251970. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Windows;

namespace U5BFA.Libraries
{
	public partial class App : Application
	{
        private Window? _window;

        public App()
        {
            AppDomain.CurrentDomain.ProcessExit += AppDomain_ProcessExit;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            TrayIconManager.Default.Initialize(new(
                "Assets\\Tray.ico",
                "TrayIconFlyout sample app (WPF)",
                new("056EACEE-82B0-48AC-A6E9-34DAE5CD37F3")));

            _window = new MainWindow();
            _window.Activate();
        }

        private void AppDomain_ProcessExit(object? sender, EventArgs e)
        {
            TrayIconManager.Default.Dispose();
        }
    }
}
