# BatreConservation

BatreConservation adalah aplikasi desktop Windows yang ditujukan khusus untuk Lenovo ThinkPad T480 yang ingin memanfaatkan fitur cut-off battery. Aplikasi ini membantu memantau status baterai, mengatur batas cut-off charging, dan memberi pemahaman tentang dukungan perangkat yang tersedia.

> Status: ThinkPad T480 focused prototype

## Fitur Utama

* Monitoring baterai real-time
* Deteksi status pengisian dan adaptor AC
* Pengaturan batas stop charging dan resume charging
* Toggle enable/disable threshold
* Log aktivitas lokal
* Penyimpanan konfigurasi lokal

## Tujuan Proyek

Proyek ini difokuskan untuk Lenovo ThinkPad T480 agar lebih realistis dan tidak mengklaim fitur yang tidak bisa dijamin bekerja di semua model Lenovo. Tujuannya adalah memberikan pengalaman yang lebih cocok untuk pengguna ThinkPad T480 yang ingin menjaga kesehatan baterai.

## Persyaratan

* Windows 10/11 (64-bit)
* .NET 8 Runtime
* Lenovo ThinkPad T480

## Struktur Proyek

```text
BatreConservation/

├── Config/
├── Logs/
├── Models/
├── Services/
├── ViewModels/
├── Views/
├── App.xaml
├── App.xaml.cs
└── BatteryConservation.csproj
```

## Build

```bash
dotnet restore BatteryConservation.sln
dotnet build BatteryConservation.sln
```

## Jalankan

```bash
dotnet run --project BatteryConservation.csproj
```

## Catatan Penting

Aplikasi ini dibuat untuk ThinkPad T480 dan tidak menjamin fitur cut-off battery akan bekerja pada semua konfigurasi. Keberhasilan fitur sangat bergantung pada dukungan BIOS, driver, dan mekanisme sistem yang tersedia pada laptop Anda.

## Lisensi

MIT License
