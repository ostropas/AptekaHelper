namespace DesctopAptekaHelper
{
    public class IdsData
    {
        public string ProductName;
        public string Id;
        public string Count;

        public IdsData(string productName, string id, string count)
        {
            ProductName = productName;
            Id = id;
            Count = count;
        }

        public IdsData(string def)
        {
            var vals = def.Split(';');
            ProductName = vals[0];
            Id = vals[1];
            Count = vals[2];
        }

        public IdsData(IdsData idsData)
        {
            ProductName = idsData.ProductName;
            Id = idsData.Id;
            Count = idsData.Count;
        }
    }
}