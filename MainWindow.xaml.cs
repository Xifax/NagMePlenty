using NHotkey.WindowsForms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Threading;

namespace NagMePlenty
{
    public partial class MainWindow : Window
    {
        /* Properties*/
        private const String APP_ID = "NagMePlenty";
        Toaster toaster;
        Reader reader;
        DispatcherTimer timer;

        // Initialize the application
        public MainWindow()
        {
            // Initialize app
            InitializeComponent();
            new ShortcutHelper(APP_ID).InitShortcut();
            toaster = new Toaster(APP_ID);
            reader = new Reader();

            /* Load settings */

            // Interval
            intervalSlider.Value = Properties.Settings.Default.Interval;
            intervalLabel.Content = String.Format("Every {0} minute(s)", (int)intervalSlider.Value);
            
            // Stats
            totalShown.Content = String.Format("Total items shown: {0}", Properties.Settings.Default.StatsTotalShown);
            totalUptime.Content = String.Format("Total uptime: {0}", String.Format("{0:hh\\:mm\\:ss}",
                TimeSpan.FromSeconds(Properties.Settings.Default.StatsUptime)));

            // Files
            loadLocalFiles.SetCurrentValue(
                System.Windows.Controls.CheckBox.IsCheckedProperty,
                Properties.Settings.Default.LoadLocalFiles);
            if (Properties.Settings.Default.LoadLocalFiles)
            {
                reader.LoadPath();
            }

            // Load files
            if (Properties.Settings.Default.Files != null)
            {
                foreach (String file in Properties.Settings.Default.Files)
                {
                    // Load items
                    reader.LoadFile(file);
                    // Update file list
                    ListBoxItem item = new ListBoxItem();
                    item.Content = file;
                    fileList.Items.Add(item);
                }
            } else
            {
                Properties.Settings.Default.Files = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Save();
            }

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
            timer = new DispatcherTimer();
            timer.Tick += ShowToast;
            timer.Interval = new TimeSpan(
                0, // hours
                Properties.Settings.Default.Interval, // minutes
                0); // seconds
            timer.Start();
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

            // Update stats and labels
            Properties.Settings.Default.StatsTotalShown += 1;
            Properties.Settings.Default.Save();
            totalShown.Content = String.Format("Total items shown: {0}", Properties.Settings.Default.StatsTotalShown);
        }

        /* Events */

        private void addFiles(object sender, RoutedEventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog selectFiles = new OpenFileDialog();

            // Set filter options and filter index.
            selectFiles.Filter = "Text Files (.txt)|*.txt";
            selectFiles.FilterIndex = 1;

            selectFiles.Multiselect = true;

            // Process input if the user clicked OK.
            if (selectFiles.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Open the selected file to read.
                foreach(String file in selectFiles.FileNames)
                {
                    // Update list
                    ListBoxItem item = new ListBoxItem();
                    item.Content = file;
                    fileList.Items.Add(item);

                    // Load file contents
                    reader.LoadFile(file);

                    // Update settings
                    Properties.Settings.Default.Files.Add(file);
                    Properties.Settings.Default.Save();
                }

            }
        }

        // Double click to remove file from list & configuration
        private void fileList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (fileList.SelectedItem != null)
            {
                String item = ((ListBoxItem)fileList.SelectedItem).Content.ToString();
                Properties.Settings.Default.Files.Remove(item);
                Properties.Settings.Default.Save();
                fileList.Items.Remove(fileList.SelectedItem);
            }
        }

        /* Update config: load files from program folder */

        private void loadLocalFiles_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LoadLocalFiles = true;
            Properties.Settings.Default.Save();
        }

        private void loadLocalFiles_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LoadLocalFiles = false;
            Properties.Settings.Default.Save();
        }

        // Update interval value
        private void intervalUpdated(object sender, DragCompletedEventArgs e)
        {
            intervalLabel.Content = String.Format("Every {0} minute(s)", (int)intervalSlider.Value);
            Properties.Settings.Default.Interval = (int)intervalSlider.Value;
            Properties.Settings.Default.Save();

            // Restart timer
            timer.Stop();
            timer.Interval = new TimeSpan(
                0, // hours
                (int)intervalSlider.Value, // minutes
                0); // seconds
            timer.Start();
        }

        /* On exit */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Perform tasks at application exit
            Properties.Settings.Default.StatsUptime += 
                (int)(DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds;
            Properties.Settings.Default.Save();
        }
    }
}
