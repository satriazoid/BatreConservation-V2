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
        private readonly DispatcherTimer _timer;

        private int _batteryPercentage;
        private string _statusText = "Unknown";
        private bool _isThresholdEnabled;
        private bool _lastThresholdState;

        public int BatteryPercentage { get => _batteryPercentage; set { _batteryPercentage = value; OnPropertyChanged(); } }
        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }
        public bool IsThresholdEnabled { get => _isThresholdEnabled; set { _isThresholdEnabled = value; OnPropertyChanged(); } }
        public int StopCharging { get => _configService.Config.StopCharging; set { _configService.Config.StopCharging = value; _configService.SaveConfig(); OnPropertyChanged(); } }
        public int ResumeCharging { get => _configService.Config.ResumeCharging; set { _configService.Config.ResumeCharging = value; _configService.SaveConfig(); OnPropertyChanged(); } }

        public ICommand EnableCommand { get; }
        public ICommand DisableCommand { get; }

        public DashboardViewModel(BatteryService battery, LenovoThresholdService threshold, ConfigService config, NotificationService notifier, StartupService startup)
        {
            _batteryService = battery;
            _thresholdService = threshold;
            _configService = config;
            _notifier = notifier;
            _startupService = startup;

            IsThresholdEnabled = _configService.Config.ThresholdEnabled;
            _lastThresholdState = IsThresholdEnabled;

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

            if (!IsThresholdEnabled || !status.IsAcConnected)
            {
                return;
            }

            int stop = Math.Clamp(_configService.Config.StopCharging, 0, 100);
            int resume = Math.Clamp(_configService.Config.ResumeCharging, 0, 100);

            bool shouldStopCharging = status.Percentage >= stop && status.IsCharging;
            bool shouldResumeCharging = status.Percentage <= resume && !status.IsCharging;

            if (shouldStopCharging && !_lastThresholdState)
            {
                _lastThresholdState = true;
                _thresholdService.SetConservationMode(true);
                _notifier.ShowNotification("Charging Stopped", $"Battery reached {stop}%.");
            }
            else if (shouldResumeCharging && _lastThresholdState)
            {
                _lastThresholdState = false;
                _thresholdService.SetConservationMode(false);
                _notifier.ShowNotification("Charging Resumed", $"Battery dropped to {resume}%.");
            }
        }

        private void ToggleThreshold(bool enable)
        {
            IsThresholdEnabled = enable;
            _configService.Config.ThresholdEnabled = enable;
            _configService.SaveConfig();
            _lastThresholdState = enable;
            _thresholdService.SetConservationMode(enable);
            _notifier.ShowNotification(enable ? "Threshold Enabled" : "Threshold Disabled", enable ? "Battery limit active." : "Battery will charge to 100%.");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        public RelayCommand(Action<object?> execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged;
    }
}