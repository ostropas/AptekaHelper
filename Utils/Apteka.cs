using CsvHelper.Configuration.Attributes;

namespace DesctopAptekaHelper
{
    public class Apteka
    {
        [Name("Продукт")] public string ProductName { get; set; }
        [Name("Аптека")] public string Name { get; set; }
        public string Address { get; set; }
        [Name("Количество")] public string Count { get; set; }

        public Apteka(string productName, string name, string address, string count)
        {
            ProductName = productName;
            Address = address;
            Name = name;
            Count = count;
        }

        public override string ToString()
        {
            return $"{ProductName};{Name};{Address};{Count}";
        }
    }
}