

// Use the sample data? Comment to not use it.
#define UseSampleData

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace FarmMonkey_GUI
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class NewWindow : Window
    {

        #region Global Variables
        Settings Config;
        DispatcherTimer dispatcherTimer;
        DateTime lastUpdate;
        private ObservableCollection<RowItem> _plantTimers = new ObservableCollection<RowItem>();
        Random random = new Random();
        #endregion Global Variables

        public NewWindow()
        {
            InitializeComponent();

            // Get our settings
            Config = Settings.GetSection(System.Configuration.ConfigurationUserLevel.PerUserRoaming);
            // Apply saved or default settings
            chkEnableGPS.IsChecked = Config.EnableGPS;
            chkDeathRoutes.IsChecked = Config.DeathCheck;
            GpsFile.Text = Config.GpsFile;
            MinLabor.Text = Config.MinLabor.ToString();


            dispatcherTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Normal, dispatcherTimer_Tick, Dispatcher);
            dispatcherTimer.Start();

            lastUpdate = DateTime.Now;

            // Timers collection Binding
            PlantTimers.ItemsSource = _plantTimers;

#if UseSampleData
            SampleData();
#endif
        }
#if UseSampleData
        // Below is for sample table data
        public void SampleData()
        {
            for (int i = 0; i <= 25; i++)
            {
                _plantTimers.Add(new RowItem
                {
                    Farm = (new Housing { name = "Sample Data " + i, uniqHousingId = 100000 + (uint)i }),
                    Plant = "Unknown Plant " + i,
                    Timer = (TimeSpan.FromSeconds(random.Next(20, 300))),
                });
            }
        }
#endif

        /// <summary>
        /// Main Timer Method
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                DataGridAutomaticVisibility();
                TimeSpan Elapsed = (DateTime.Now - lastUpdate);
                if (Elapsed >= TimeSpan.FromSeconds(1))
                {
                    AutoTimerProc(Elapsed);
                    TimerProc(Elapsed);

                    // Keep this at the end of the If block
                    lastUpdate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Decreases the countdown timer used for Auto Harvesting/Planting
        /// </summary>
        /// <param name="Elapsed">Timespan with the difference to subtract</param>
        private void AutoTimerProc(TimeSpan Elapsed)
        {
            TimeSpan timer = new TimeSpan();
            if (TimerTextBlock.Tag != null)
            {
                timer = (TimeSpan)TimerTextBlock.Tag;
            }
            else
            {
                timer = TimeSpan.Parse("00:" + TimerTextBlock.Text);
                Console.WriteLine("TimerTextBlock.Tag wasn't a Timespan.");
            }

            TimeSpan result = timer.Subtract(Elapsed);
            TimerTextBlock.Tag = result;
            TimerTextBlock.Text = result.ToString(@"mm\:ss");
        }

        /// <summary>
        /// Decreases all the timers in the list
        /// </summary>
        /// <param name="Elapsed">Timespan with the difference to subtract</param>
        private void TimerProc(TimeSpan Elapsed)
        {
            // Update the plant timers
            if (_plantTimers.Count >= 1)
            {
                try
                {
                    // Remove expired timers
                    _plantTimers
                        .Where(i => i.Timer.TotalSeconds < 1)
                        .ToList()
                        .ForEach(i => _plantTimers.Remove(i));

                    // Decrement the timers.
                    _plantTimers
                        .Where(i => i.Timer.TotalSeconds >= 1)
                        .ToList()
                        .ForEach(i => i.Timer = i.Timer - Elapsed);
                }
                // Show us any errors that appear.
                //catch (InvalidOperationException Exception) {  }
                catch (Exception Exception)
                {
                    Console.WriteLine(Exception.ToString());
                }
            }
        }

        /// <summary>
        /// Automatically sets the visibility of the datagrid
        /// according to the amount of items it has
        /// </summary>
        private void DataGridAutomaticVisibility()
        {
            if (_plantTimers.Count <= 0 && PlantTimers.Visibility == System.Windows.Visibility.Visible) PlantTimers.Visibility = System.Windows.Visibility.Collapsed;
            if (_plantTimers.Count > 0 && PlantTimers.Visibility == System.Windows.Visibility.Collapsed) PlantTimers.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Textbox Limiters
        /// Length & Value
        /// </summary>
        private void MinLabor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if (
                    // Limit length
                    ((TextBox)sender).Text.Length >= 4 |
                    // Limit amount
                    uint.Parse(((TextBox)sender).Text) > 5000
                    )
                    e.Handled = true;
            }
            catch { }
        }

        /// <summary>
        /// Textbox Limiters
        /// Valid Keys
        /// </summary>
        private void MinLabor_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            int key = (int)e.Key;

            e.Handled = !(
                (key >= (int)Key.D0 && key <= (int)Key.D9) || // Digits 0-9
                (key >= (int)Key.NumPad0 && key <= (int)Key.NumPad9) || // Numpad 0-9
                (key >= (int)Key.Left && key <= (int)Key.Down) || // Arrow Keys
                key == (int)Key.Back || // Backspace
                key == (int)Key.Delete
                );
        }

        private void doHarvesting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void doPlanting_Click(object sender, RoutedEventArgs e)
        {

        }

        private void doWatering_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTimers_Click(object sender, RoutedEventArgs e)
        {
            _plantTimers.Clear();

#if UseSampleData
            SampleData();
#endif
        }

        private void btnTimers_RightClick(object sender, ContextMenuEventArgs e)
        {
            _plantTimers.Clear();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsMenu.IsOpen = true;
        }

        private void SettingsSave_Click(object sender, RoutedEventArgs e)
        {
            SettingsMenu.IsOpen = false;
        }

        private void btnGpsFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Filter = "SQLite Database (*.db3)|*.db3";
            Dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Dialog.Multiselect = false;
            bool success = (bool)Dialog.ShowDialog(this);
            if (success && Dialog.SafeFileName.Split(char.Parse(".")).Last() == "db3")
            {
                GpsFile.Text = Dialog.FileName;
            }
        }
    }
}
