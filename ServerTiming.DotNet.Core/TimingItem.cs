using System.Text;

namespace ServerTiming.DotNet.Core
{
    public class TimingItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double? Duration { get; set; }

        public override string ToString()
        {
            var value = new StringBuilder();

            value.Append(Name);
            if (Duration.HasValue)
            {
                value.Append(";dur=" + Duration.Value);
            }

            if (!string.IsNullOrEmpty(Description))
            {
                value.Append($";desc=\"${Description.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"");
            }

            return value.ToString();
        }
    }
}