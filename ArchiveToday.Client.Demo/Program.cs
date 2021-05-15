using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArchiveToday.Client.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();

            var archiveTodayClient = new ArchiveTodayClient(httpClient);

            var response = await archiveTodayClient.GetTimemapAsync(new Uri("https://www.bbc.co.uk/"));
        }
    }
}