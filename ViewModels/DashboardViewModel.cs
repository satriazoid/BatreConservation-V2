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
        private bool _isConservationActive;

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
            _isConservationActive = false;

            EnableCommand = new RelayCommand(_ => ToggleThreshold(true));
            DisableCommand = new RelayCommand(_ => ToggleThreshold(false));

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += OnTimerTick;
            _timer.Start();
            OnTimerTick(this, EventArgs.Empty);
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            var status = _batteryService.GetBatteryStatus();
            BatteryPercentage = status.Percentage;
            StatusText = status.IsAcConnected ? (status.IsCharging ? "Charging" : "AC Connected, Not Charging") : "Discharging";

            if (!IsThresholdEnabled)
            {
                if (_isConservationActive)
                {
                    _isConservationActive = false;
                    _thresholdService.SetConservationMode(false);
                    _notifier.ShowNotification("Threshold Disabled", "Battery threshold is off.");
                }
                return;
            }

            if (!status.IsAcConnected)
            {
                return;
            }

            int stop = Math.Clamp(_configService.Config.StopCharging, 0, 100);
            int resume = Math.Clamp(_configService.Config.ResumeCharging, 0, 100);

            bool shouldStopCharging = status.Percentage >= stop && status.IsCharging;
            bool shouldResumeCharging = status.Percentage <= resume && !status.IsCharging;

            if (shouldStopCharging && !_isConservationActive)
            {
                _isConservationActive = true;
                _thresholdService.SetConservationMode(true);
                _notifier.ShowNotification("Cut-off Charge Active", $"Battery reached {stop}% and charging was stopped.");
            }
            else if (shouldResumeCharging && _isConservationActive)
            {
                _isConservationActive = false;
                _thresholdService.SetConservationMode(false);
                _notifier.ShowNotification("Charging Resumed", $"Battery dropped to {resume}%.");
            }
        }

        private void ToggleThreshold(bool enable)
        {
            IsThresholdEnabled = enable;
            _configService.Config.ThresholdEnabled = enable;
            _configService.SaveConfig();

            if (!enable)
            {
                _isConservationActive = false;
                _thresholdService.SetConservationMode(false);
                _notifier.ShowNotification("Threshold Disabled", "Battery will charge to 100%.");
                return;
            }

            var status = _batteryService.GetBatteryStatus();
            int stop = Math.Clamp(_configService.Config.StopCharging, 0, 100);
            bool shouldActivateNow = status.IsAcConnected && status.IsCharging && status.Percentage >= stop;
            _isConservationActive = shouldActivateNow;

            _thresholdService.SetConservationMode(shouldActivateNow);
            _notifier.ShowNotification(
                "Threshold Enabled",
                shouldActivateNow
                    ? $"Battery limit active at {stop}%."
                    : $"Battery limit enabled at {stop}%. Waiting for cut-off threshold.");
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