using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace AptekaHelper
{
    public class DataWriter
    {
        public void Write(string fileName, IEnumerable<Apteka> values)
        {
            var fullFileName = $"{fileName}_{DateTime.Now.ToLocalTime()}.csv";
            fullFileName = fullFileName.Replace(':', '_');

            if (_directorySelected)
            {
                WriteToTargetPath(values, Path.Combine(_directory, fullFileName));
            } else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = fullFileName,
                    DefaultExt = "csv",
                    AddExtension = true
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    WriteToTargetPath(values, saveFileDialog.FileName);
                }
            }
        }

        private void WriteToTargetPath(IEnumerable<Apteka> values, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.Unicode))
            {
                var serializer = new CsvHelper.CsvSerializer(sw, CultureInfo.CurrentCulture);
                using (CsvWriter csvReader = new CsvWriter(serializer))
                {
                    csvReader.Configuration.Delimiter = "\t";
                    try
                    {
                        csvReader.WriteRecords(values);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Please close {fileName} file");
                        throw;
                    }
                }
            }
        }

        private bool _directorySelected => !string.IsNullOrEmpty(_directory);
        private string _directory;
        public void SelectDirectory()
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            var res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                _directory = dialog.SelectedPath;
            }
        }

        public void SetDirectory(string directory)
        {
            _directory = directory;
            if (!System.IO.Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);
        }

        public string GetByteArray(IEnumerable<Apteka> values)
        {
            var memStream = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(memStream, Encoding.Unicode))
            {

                var serializer = new CsvHelper.CsvSerializer(sw, CultureInfo.CurrentCulture);
                using (CsvWriter csvReader = new CsvWriter(serializer))
                {
                    csvReader.Configuration.Delimiter = "\t";
                    csvReader.WriteRecords(values);
                }
            }

            return Encoding.Unicode.GetString(memStream.ToArray());
        }
    }
}