using AptekaHelper.Parsers;
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
            var avrTime = _avrIterationTime * _file.Count;
            var value = progress * 100;
            Progress.Value = value;
            int seconds = (int)(avrTime - _allTime.ElapsedMilliseconds / 1000);
            var time = new TimeSpan(0, 0, seconds);
            ProgressText.Content = $"{Math.Round(value, 0)}%, Time left: {time.ToString()}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _parser.Init(_file, City.Text);
            _sw.Start();
            _allTime.Start();

            _avrIterationTime = 0;

            var dataWriter = new DataWriter();
            dataWriter.SelectDirectory();

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(async delegate ()
            {
                //try
                //{
                    var res = await _parser.ParseSite(_sw);
                    _parser.WriteToFile(res, dataWriter);
                    _sw.Reset();
                    _allTime.Reset();
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
                this.FileButton.Content = $"File:\"{filename}\"";                
                _file = System.IO.File.ReadAllLines(filename).Where(x => !string.IsNullOrEmpty(x)).ToList();
                DownLoadButton.IsEnabled = true;
            }
        }
    }
}
