using CsvHelper.Configuration.Attributes;

namespace AptekaHelper
{
    public class Apteka
    {
        [Name("Сеть")] public string Net { get; set; }
        [Name("Id товара")] public string Id { get; set; }
        [Name("Продукт")] public string ProductName { get; set; }
        [Name("Аптека")] public string Name { get; set; }
        [Name("Адрес")] public string Address { get; set; }
        [Name("Количество")] public string Count { get; set; }

        public Apteka(string productName, string name, string address, string count, string net, string id)
        {
            ProductName = productName;
            Address = address;
            Name = name;
            Count = count;
            Net = net;
            Id = id;
        }

        public override string ToString()
        {
            return $"{Net};{Id};{ProductName};{Name};{Address};{Count}";
        }
    }
}