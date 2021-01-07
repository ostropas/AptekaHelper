using DesctopAptekaHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AptekaHelper
{
    /// <summary>
    /// Interaction logic for SitePage.xaml
    /// </summary>
    public partial class SitePage : Page
    {
        private float _avrIterationTime;
        private Stopwatch _sw = new Stopwatch();
        private Stopwatch _allTime = new Stopwatch();
        BaseSiteParser _parser;
        private List<string> _file = new List<string>();
        public SitePage(BaseSiteParser parser, Action<bool> blockAct = null)
        {
            InitializeComponent();
            _parser = parser;
            _parser.ProgressUpdated += UpdateProgressBar;
            //Block += BlockElements;
            //BlockElements(false);
            //Block += blockAct;
        }

        private void BlockElements(bool blocked)
        {
            DownLoadButton.IsEnabled = !blocked && _file.Count > 0;
            City.IsEnabled = !blocked && _parser.NeedCity;
            FileButton.IsEnabled = !blocked;
        }

        private void UpdateProgressBar(float progress)
        {
            _sw.Stop();
            if (_avrIterationTime == 0)
            {
                _avrIterationTime = _sw.ElapsedMilliseconds / 1000;
            }
            _sw.Restart();
            var avrTime = _avrIterationTime * _file.Count;
            var value = progress * 100;
            Progress.Value = value;
            ProgressText.Content = $"P:{Math.Round(value, 0)}, S:{_allTime.ElapsedMilliseconds / 1000}, R:{avrTime}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _parser.Init(_file, City.Text);
            _sw.Start();
            _allTime.Start();

            _avrIterationTime = 0;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(async delegate ()
            {
                await _parser.SaveToFile(_sw);
                _sw.Reset();
                _allTime.Reset();
            }));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            //dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                this.FileButton.Content = $"File:\"{filename}\"";
                _file = System.IO.File.ReadAllLines(filename).ToList();
                DownLoadButton.IsEnabled = true;
            }
        }
    }
}
