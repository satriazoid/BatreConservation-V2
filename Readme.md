# Battery Conservation

Battery Conservation adalah aplikasi desktop Windows yang dirancang khusus untuk laptop Lenovo yang mendukung Battery Charge Threshold. Aplikasi ini memungkinkan pengguna memantau kondisi baterai serta mengelola batas pengisian daya melalui antarmuka yang modern, ringan, dan mudah digunakan.

> Status: In Development

## Features

* Modern dark theme.
* Rounded UI dengan aksen putih.
* Real-time battery monitoring.
* Battery percentage.
* Charging status.
* AC adapter detection.
* Battery health information.
* Design Capacity.
* Full Charge Capacity.
* Battery Wear calculation.
* Cycle Count.
* Enable atau Disable charging threshold.
* Charging threshold presets.

  * Stop Charging: 80%, 90%, 95%.
  * Resume Charging: 75%, 85%, 90%.
* Start with Windows.
* Minimize to System Tray.
* Windows notification.
* Configuration auto save.
* Local log history.
* Offline mode.

## Screenshot

Coming Soon

## Technology Stack

| Component     | Technology         |
| ------------- | ------------------ |
| Language      | C#                 |
| Framework     | .NET 8             |
| UI            | WPF                |
| Pattern       | MVVM               |
| Configuration | JSON               |
| Logging       | Serilog            |
| Notifications | Windows App SDK    |
| IDE           | Visual Studio 2022 |

## Project Structure

```text
BatteryConservation/

├── Assets/
├── Config/
├── Helpers/
├── Logs/
├── Models/
├── Resources/
├── Services/
├── ViewModels/
├── Views/
├── App.xaml
└── Program.cs
```

## Requirements

* Windows 11 (64-bit)
* .NET 8 Runtime
* Lenovo laptop with Battery Charge Threshold support

## Installation

```bash
https://github.com/satriazoid/BatreConservation-V2
```

### Build dengan VS Code + .NET Dev Kit

1. Buka folder proyek di VS Code.
2. Pastikan ekstensi .NET Dev Kit sudah terinstall.
3. Buka terminal dan jalankan:

```bash
dotnet restore BatteryConservation.sln
dotnet build BatteryConservation.sln
```

4. Jika ingin menjalankan aplikasi:

```bash
dotnet run --project BatteryConservation.csproj
```

5. Atau pilih project di Explorer lalu klik Run/Debug.

> Catatan: project ini ditargetkan untuk Windows dan .NET 8.

## Planned Features

* Automatic update.
* Battery usage history.
* Battery health chart.
* Multiple charging profiles.
* Battery analytics dashboard.
* Portable version.
* CLI support.
* Local REST API.
* Export logs.
* Multi-language support.
* ASUS support.
* Dell support.
* HP support.
* MSI support.

## Roadmap

### Version 1.0

* Battery monitoring
* Threshold management
* System tray
* Notifications
* Settings

### Version 1.1

* Battery analytics
* Charts
* Export logs
* Performance improvements

### Version 2.0

* Multi-brand support
* Plugin architecture
* Auto updater
* Portable mode

## License

MIT License

## Disclaimer

Battery Conservation only supports laptops that expose battery charging controls through Lenovo firmware, BIOS, ACPI, or WMI interfaces. Hardware without charge threshold support cannot be controlled by software alone.
