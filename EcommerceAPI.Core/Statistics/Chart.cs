namespace EcommerceAPI.Core.Statistics
{
    public class Chart
    {
        public List<double> Data { get; set; } = [];
        public List<string> Titles { get; set; } = [];
    }
    public class RevenueChart
    {
        public List<double> Sales { get; set; } = [];
        public List<double> Revenue { get; set; } = [];
        public List<string> Titles { get; set; } = [];
    }
}
