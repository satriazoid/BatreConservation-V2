using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using BatteryGuardian.Services;

namespace BatteryGuardian.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly BatteryService _batteryService;
        private readonly LenovoThresholdService _thresholdService;
        private readonly ConfigService _configService;
        private readonly NotificationService _notifier;
        private readonly StartupService _startupService;
        private DispatcherTimer _timer;

        private int _batteryPercentage;
        private string _statusText = "Unknown";
        private bool _isThresholdEnabled;

        public int BatteryPercentage { get => _batteryPercentage; set { _batteryPercentage = value; OnPropertyChanged(); } }
        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }
        public bool IsThresholdEnabled { get => _isThresholdEnabled; set { _isThresholdEnabled = value; OnPropertyChanged(); } }

        public ICommand EnableCommand { get; }
        public ICommand DisableCommand { get; }

        public DashboardViewModel(BatteryService battery, LenovoThresholdService threshold, ConfigService config, NotificationService notifier, StartupService startup)
        {
            _batteryService = battery;
            _thresholdService = threshold;
            _configService = config;
            _notifier = notifier;
            _startupService = startup;

            EnableCommand = new RelayCommand(_ => ToggleThreshold(true));
            DisableCommand = new RelayCommand(_ => ToggleThreshold(false));

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            var status = _batteryService.GetBatteryStatus();
            BatteryPercentage = status.Percentage;
            StatusText = status.IsAcConnected ? (status.IsCharging ? "Charging" : "AC Connected, Not Charging") : "Discharging";

            // Logika Evaluasi Limit Baterai Berdasarkan Config
            if (IsThresholdEnabled && status.IsAcConnected)
            {
                if (status.Percentage >= _configService.Config.StopCharging && status.IsCharging)
                {
                    _thresholdService.SetConservationMode(true); // Stop charging
                    _notifier.ShowNotification("Charging Stopped", $"Battery reached {_configService.Config.StopCharging}%.");
                }
                else if (status.Percentage <= _configService.Config.ResumeCharging && !status.IsCharging)
                {
                    _thresholdService.SetConservationMode(false); // Resume charging
                    _notifier.ShowNotification("Charging Resumed", $"Battery dropped to {_configService.Config.ResumeCharging}%.");
                }
            }
        }

        private void ToggleThreshold(bool enable)
        {
            IsThresholdEnabled = enable;
            _thresholdService.SetConservationMode(enable);
            _notifier.ShowNotification(enable ? "Threshold Enabled" : "Threshold Disabled", enable ? "Battery limit active." : "Battery will charge to 100%.");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Simple RelayCommand Implementation untuk MVVM
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        public RelayCommand(Action<object?> execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged;
    }
}