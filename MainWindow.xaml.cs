using NHotkey.WindowsForms;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace NagMePlenty
{
    public partial class MainWindow : Window
    {
        /* Properties*/
        private const String APP_ID = "NagMePlenty";
        Toaster toaster;
        Reader reader;

        // Initialize the application
        public MainWindow()
        {
            // Initialize app
            InitializeComponent();
            new ShortcutHelper(APP_ID).InitShortcut();
            toaster = new Toaster(APP_ID);
            reader = new Reader();
            reader.LoadFile();

            // Hotkeys
            HotkeyManager.Current.AddOrReplace("Increment",
                Keys.Control | Keys.Alt | Keys.S, ShowToast);

            // Windows settings
            WindowState = WindowState.Minimized;
            ShowInTaskbar = false;

            // Show tray icon
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon(Path.GetFullPath("Resources/Schedule.ico"));
            ni.Visible = true;
            ni.DoubleClick += delegate (object sender, EventArgs args)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            // Initialize timer
            System.Windows.Threading.DispatcherTimer dispatcherTimer = 
                new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += ShowToast;
            dispatcherTimer.Interval = new TimeSpan(0, 5, 0);
            dispatcherTimer.Start();
        }

        // Update window status
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }


        /* Show toast */

        private void ShowToast(object sender, EventArgs e)
        {
            toaster.showToast(reader.getRandomItem());
        }

        /*
        private void toastactivated(toastnotification sender, object e)
        {
            dispatcher.invoke(() =>
            {
                activate();
                output.text = "the user activated the toast.";
            });
        }

        private void toastdismissed(toastnotification sender, toastdismissedeventargs e)
        {
            string outputtext = "";
            switch (e.reason)
            {
                case toastdismissalreason.applicationhidden:
                    outputtext = "the app hid the toast using toastnotifier.hide";
                    break;
                case toastdismissalreason.usercanceled:
                    outputtext = "the user dismissed the toast";
                    break;
                case toastdismissalreason.timedout:
                    outputtext = "the toast has timed out";
                    break;
            }

            dispatcher.invoke(() =>
            {
                output.text = outputtext;
            });
        }

        private void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Output.Text = "The toast encountered an error.";
            });
        }
        */
    }
}
