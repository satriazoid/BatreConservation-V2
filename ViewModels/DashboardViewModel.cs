using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using BatreConservation.Services;

namespace BatreConservation.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly BatteryService _batteryService;
        private readonly LenovoThresholdService _thresholdService;
        private readonly ConfigService _configService;
        private readonly StartupService _startupService;
        private readonly DispatcherTimer _timer;

        private int _batteryPercentage;
        private string _statusText = "Ready";
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

            if (!IsThresholdEnabled)
            {
                StatusText = "Monitoring only • ThinkPad T480";
            }
            else
            {
                StatusText = status.IsAcConnected
                    ? (status.IsCharging ? "Threshold enabled • charging" : "Threshold enabled • plugged in")
                    : "Threshold enabled • waiting for AC";
            }

            if (!IsThresholdEnabled)
            {
                if (_isConservationActive)
                {
                    _isConservationActive = false;
                    _thresholdService.SetConservationMode(false);
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
            }
            else if (shouldResumeCharging && _isConservationActive)
            {
                _isConservationActive = false;
                _thresholdService.SetConservationMode(false);
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
                StatusText = "Monitoring only • ThinkPad T480";
                _thresholdService.SetConservationMode(false);
                return;
            }

            var status = _batteryService.GetBatteryStatus();
            int stop = Math.Clamp(_configService.Config.StopCharging, 0, 100);
            bool shouldActivateNow = status.IsAcConnected && status.IsCharging && status.Percentage >= stop;
            bool appliedToHardware = _thresholdService.SetConservationMode(shouldActivateNow);

            if (!appliedToHardware)
            {
                _isConservationActive = false;
                StatusText = "Threshold enabled • monitor mode";
                return;
            }

            _isConservationActive = shouldActivateNow;
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