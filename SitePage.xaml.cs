using AptekaHelper.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;

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
        DataWriter _dataWriter;
        private List<string> _idsFile = new List<string>();
        private List<string> _cityFile = new List<string>();
        private List<string> _configFile = new List<string>();

        public SitePage(BaseSiteParser parser, Action<bool> blockAct = null)
        {
            InitializeComponent();
            _parser = parser;
            _parser.ProgressUpdated += UpdateProgressBar;
            _dataWriter = new DataWriter();
            //Block += BlockElements;
            //BlockElements(false);
            //Block += blockAct;
        }

        private void BlockElements(bool blocked)
        {
            //DownLoadButton.IsEnabled = !blocked && _file.Count > 0;
            //City.IsEnabled = !blocked && _parser.NeedCity;
            //FileButton.IsEnabled = !blocked;
        }

        private void UpdateProgressBar(float progress)
        {
            _sw.Stop();
            if (_avrIterationTime == 0)
            {
                _avrIterationTime = _sw.ElapsedMilliseconds / 1000;
            }
            _sw.Restart();
            var avrTime = _avrIterationTime * _idsFile.Count;
            var value = progress * 100;
            Progress.Value = value;
            int seconds = (int)(avrTime - _allTime.ElapsedMilliseconds / 1000);
            var time = new TimeSpan(0, 0, seconds);
            ProgressText.Content = $"{Math.Round(value, 0)}%, Time left: {time.ToString()}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //_parser.Init(_file, City.Text);
            _sw.Start();
            _allTime.Start();

            _avrIterationTime = 0;

            //var dataWriter = new DataWriter();
            //dataWriter.SelectDirectory();
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(async delegate ()
            {
                foreach (var city in _cityFile)
                {
                    _parser.Init(_idsFile, city, ShowBrowser.IsChecked.HasValue && ShowBrowser.IsChecked.Value);
                    var res = await _parser.ParseSite(_sw);
                    _parser.WriteToFile(res, _dataWriter);
                    _sw.Reset();
                    _allTime.Reset();
                }
                //try
                //{
                //}
                //catch (Exception exception)
                //{
                //    //Logger.Logger.Log(exception.ToString());
                //    if (_parser is IDisposable di)
                //        di.Dispose();
                //    throw exception;
                //}
            }));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //_file = ReadFileLines(this.FileButton);
        }

        private void FileCityButton_Click(object sender, RoutedEventArgs e)
        {
            //_cityFile = ReadFileLines(this.FileCityButton);
        }

        //private void ReadConfigFilt(string filePath)ReadFileLines
        //{
            
        //}

        private string GetFileName()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.Filter = "Ini documents (.ini)|*.ini"; // Filter files by extension

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                return filename;
            }

            return null;
        }

        private List<string> ReadFileLines(Button button)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                button.Content = $"File:\"{filename}\"";
                return System.IO.File.ReadAllLines(filename).Where(x => !string.IsNullOrEmpty(x)).ToList();
            }

            return new List<string>();
        }

        private List<string> ReadFileData(string fileName) => File.ReadAllLines(fileName).Where(x => !string.IsNullOrEmpty(x)).ToList();

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            var configFileName = GetFileName();
            if (string.IsNullOrEmpty(configFileName))
                return;
            var fileData = ReadFileData(configFileName);
            string idsFile = fileData[0];
            string cityFile = fileData[1];
            string outPutDir = fileData[2];

            string configFileDirectory = Path.GetDirectoryName(configFileName);
            _idsFile = ReadFileData(Path.Combine(configFileDirectory, idsFile));
            _cityFile = ReadFileData(Path.Combine(configFileDirectory, cityFile));
            _dataWriter.SetDirectory(Path.Combine(configFileDirectory, outPutDir));
            this.FileButton.Content = $"File:\"{configFileName}\"";
        }
    }
}
