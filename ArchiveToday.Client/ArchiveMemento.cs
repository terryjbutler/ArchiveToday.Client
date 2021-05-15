using System;

namespace ArchiveToday.Client
{
    public class ArchiveMemento
    {
        public ArchiveMemento(Uri uri)
        {
            Uri = uri;
        }

        public ArchiveMemento(Uri uri, DateTime dateTime, string rel)
            : this(uri)
        {
            DateTime = dateTime;
            Rel = rel;
        }

        public Uri Uri { get; internal set; }
        public DateTime? DateTime { get; internal set; }
        public string Rel { get; internal set; }
    }
}