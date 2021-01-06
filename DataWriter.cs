using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;

namespace DesctopAptekaHelper
{
    public class DataWriter
    {
        public void Write(string path, IEnumerable<Apteka> values)
        {
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.Unicode))
            {
                var serializer = new CsvHelper.CsvSerializer(sw, CultureInfo.CurrentCulture);
                using (CsvWriter csvReader = new CsvWriter(serializer))
                {
                    csvReader.Configuration.Delimiter = "\t";
                    try
                    {
                        csvReader.WriteRecords(values);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Please close {path} file");
                        throw;
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