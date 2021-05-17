using RFC6690.Parser;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArchiveToday.Client
{
    public class ArchiveTodayClient
    {
        private readonly HttpClient _httpClient;
        private readonly RFC6690Parser _rfc6690Parser;

        public ArchiveTodayClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _rfc6690Parser = new RFC6690Parser();
        }

        public async Task<GetTimemapResponse> GetTimemapAsync(Uri requestUri)
        {
            var uriTimemap = new Uri("https://archive.ph/timemap/" + requestUri.AbsoluteUri);

            var responseString = await _httpClient.GetStringAsync(uriTimemap);

            var response = GetTimemap(responseString);

            return response;
        }

        public GetTimemapResponse GetTimemap(string text)
        {
            var rfc6690ParserResponse = _rfc6690Parser.Parse(text);

            var ret = new GetTimemapResponse();

            var mementoConstrainedLinks = rfc6690ParserResponse
                .ConstrainedLinks.Where(a => a.Params.ContainsKey("rel") && a.Params["rel"].Contains("memento"))
                .ToList();

            foreach (var item in mementoConstrainedLinks)
            {
                if (!Uri.TryCreate(item.Link, UriKind.Absolute, out Uri uri))
                {
                    continue;
                }

                var archiveMemento = new ArchiveMemento(uri);

                if (item.Params.TryGetValue("datetime", out string dateTimeString))
                {
                    if (DateTime.TryParse(dateTimeString, out DateTime parsedDateTime))
                    {
                        archiveMemento.DateTime = parsedDateTime;
                    }
                }

                if (item.Params.TryGetValue("rel", out string rel))
                {
                    archiveMemento.Rel = rel;
                }

                ret.ArchiveMementos.Add(archiveMemento);
            }

            ret.ArchiveMementos = ret.ArchiveMementos?
                .OrderBy(a => a.DateTime)
                .ToList();

            var sameFirstLastMemento = ret.ArchiveMementos?.SingleOrDefault(a => a.Rel.Equals("first last memento", StringComparison.InvariantCultureIgnoreCase));
            if (sameFirstLastMemento != null)
            {
                ret.FirstMemento = sameFirstLastMemento;
                ret.LastMemento = sameFirstLastMemento;
            }
            else
            {
                ret.FirstMemento = ret.ArchiveMementos?.SingleOrDefault(a => a.Rel.Equals("first memento", StringComparison.InvariantCultureIgnoreCase));
                ret.LastMemento = ret.ArchiveMementos?.SingleOrDefault(a => a.Rel.Equals("last memento", StringComparison.InvariantCultureIgnoreCase));
            }

            ret.DateFrom = ret.FirstMemento?.DateTime ?? ret.ArchiveMementos?.OrderBy(a => a.DateTime).First()?.DateTime;
            ret.DateUntil = ret.LastMemento?.DateTime ?? ret.ArchiveMementos?.OrderByDescending(a => a.DateTime).First()?.DateTime;

            return ret;
        }
    }
}
