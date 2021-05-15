using System;
using System.Collections.Generic;

namespace ArchiveToday.Client
{
    public class GetTimemapResponse
    {
        public DateTime? DateFrom { get; internal set; }
        public DateTime? DateUntil { get; internal set; }

        public ArchiveMemento FirstMemento { get; internal set; }
        public ArchiveMemento LastMemento { get; internal set; }

        public List<ArchiveMemento> ArchiveMementos { get; internal set; } = new List<ArchiveMemento>();
    }
}
