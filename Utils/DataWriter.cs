using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using Microsoft.Win32;

namespace DesctopAptekaHelper
{
    public class DataWriter
    {
        public void Write(string path, IEnumerable<Apteka> values)
        {
            var fullFileName = $"{path}_{DateTime.Now.ToLocalTime()}.csv";
            fullFileName = fullFileName.Replace(':', '_');
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = fullFileName,
                DefaultExt = "csv",
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.Unicode))
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
                            Console.WriteLine($"Please close {path} file");
                            throw;
                        }
                    }
                }
            }
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