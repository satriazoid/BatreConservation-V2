using System.ComponentModel;
using System.Windows;
using BatreConservation.ViewModels;

namespace BatreConservation.Views
{
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;

        public MainWindow(DashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Setup System Tray Icon
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Icon = System.Drawing.SystemIcons.Application; // Gunakan icon default/custom .ico Anda
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "Battery Guardian";
            
            _notifyIcon.DoubleClick += (s, e) => { ShowWindow(); };
            
            // Context Menu untuk Tray
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Open", null, (s, e) => ShowWindow());
            contextMenu.Items.Add("Exit", null, (s, e) => ExitApplication());
            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Intersept tombol X agar masuk ke Tray
            e.Cancel = true;
            Hide(); 
        }

        private void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void ExitApplication()
        {
            _notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }
    }
}