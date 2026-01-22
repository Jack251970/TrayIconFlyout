## Usage

### TrayIconFlyout (WinUI 2/3)

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

```cs
if (_trayIconFlyout.IsOpen)
    _trayIconFlyout.Hide();
else
    _trayIconFlyout.Show();
```

### TrayIconFlyout (WPF)

```csharp
// Create and configure the flyout
var trayIconFlyout = new TrayIconFlyout
{
    Width = 360,
    Height = 300
};

// Add an island with content
var island = new TrayIconFlyoutIsland
{
    Height = 250,
    Content = new StackPanel
    {
        // Add your WPF controls here
    }
};

trayIconFlyout.Islands.Add(island);

// Show/Hide the flyout
if (trayIconFlyout.IsOpen)
    trayIconFlyout.Hide();
else
    trayIconFlyout.Show();
```

### TrayIconMeunFlyout

```xml
<me:TrayIconMenuFlyout x:Class="..." ...>

    <MenuFlyoutSubItem Text="Settings">
        <MenuFlyoutSubItem.Icon>
            <FontIcon Glyph="..." />
        </MenuFlyoutSubItem.Icon>
        <MenuFlyoutSubItem.Items>
            <MenuFlyoutItem Text="Theme" />
            <MenuFlyoutItem Text="Language" />
            <MenuFlyoutItem Text="Privacy" />
        </MenuFlyoutSubItem.Items>
    </MenuFlyoutSubItem>
    <MenuFlyoutSeparator />
    <MenuFlyoutItem Text="Exit">
        <MenuFlyoutItem.Icon>
            <FontIcon Glyph="..." />
        </MenuFlyoutItem.Icon>
    </MenuFlyoutItem>

</me:TrayIconMenuFlyout>
```

```cs
if (_trayIconMenuFlyout.IsOpen)
    _trayIconMenuFlyout.Hide();

_trayIconMenuFlyout.Show(e.Point);
```

## Screenshots

Visit the repo's README: https://github.com/0x5bfa/TrayIconFlyout

## License

Copyright (c) 0x5BFA. All rights reserved.
