## Usage

### TrayIconFlyout

```xml
<me:TrayIconFlyout x:Class="..." ... Width="360">

    <me:TrayIconFlyoutIsland Height="300">
        <!-- Put elements here -->
    </me:TrayIconFlyoutIsland>
    <me:TrayIconFlyoutIsland Height="300">
        <!-- Put elements here -->
    </me:TrayIconFlyoutIsland>

</me:TrayIconFlyout>
```

```csharp
if (_trayIconFlyout.IsOpen)
    _trayIconFlyout.Hide();
else
    _trayIconFlyout.Show();
```

### SystemTrayIcon

```csharp
SystemTrayIcon = new(new("Assets\\Tray.ico"),
    "TrayIconFlyout sample app (WPF)",
    new("056EACEE-82B0-48AC-A6E9-34DAE5CD37F3"));
```

## Screenshots

Visit the repo's README: https://github.com/Jack251970/TrayIconFlyout.WPF

## License

Copyright (c) Jack251970. All rights reserved.
