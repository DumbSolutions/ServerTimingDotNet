using System.Collections.Generic;

namespace ServerTiming.DotNet.Core
{
    public class TimingHeaderWriter : List<List<TimingItem>>
    {
        public TimingHeaderValueList Add(string name, double? duration = null, string description = null)
        {
            var timingHeaderValueList = new TimingHeaderValueList()
            {
                new TimingItem()
                {
                    Name = name,
                    Duration = duration,
                    Description = description,
                }
            };
            Add(timingHeaderValueList);
            return timingHeaderValueList;
        }

        public TimingHeaderValueList AddAsOneList(
            IEnumerable<TimingItem> items)
        {
            var list = new TimingHeaderValueList();
            list.AddRange(items);
            Add(list);
            return list;
        }
    }
}