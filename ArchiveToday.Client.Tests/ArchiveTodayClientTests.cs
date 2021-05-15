using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveToday.Client.Tests
{
    /// <summary>
    /// Mocks the HttpClient to read from a snapshot copy of https://www.bbc.co.uk/
    /// </summary>
    [TestClass]
    public class ArchiveTodayClientTests
    {
        private ArchiveTodayClient _archiveTodayClient;

        [TestInitialize]
        public void Init()
        {
            var archiveExample = System.IO.File.ReadAllText("./files/archive-is-memento-example.txt");

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(archiveExample),
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            _archiveTodayClient = new ArchiveTodayClient(httpClient);
        }

        [TestMethod]
        public async Task Number_of_Mementos_Equals_403()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://www.bbc.co.uk/"));

            Assert.AreEqual(403, response.ArchiveMementos.Count);
        }

        [TestMethod]
        public async Task First_memento_link()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://www.bbc.co.uk/"));

            Assert.AreEqual(new Uri("http://archive.md/19961221203254/http://www.bbc.co.uk/"), response.FirstMemento.Uri);
        }

        [TestMethod]
        public async Task First_memento_datetime()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://www.bbc.co.uk/"));

            Assert.AreEqual(DateTime.Parse("Sat, 21 Dec 1996 20:32:54 GMT"), response.FirstMemento.DateTime);
        }

        [TestMethod]
        public async Task Last_memento_link()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://www.bbc.co.uk/"));

            Assert.AreEqual(new Uri("http://archive.md/20210429141600/http://www.bbc.co.uk/"), response.LastMemento.Uri);
        }

        [TestMethod]
        public async Task Last_memento_datetime()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://www.bbc.co.uk/"));

            Assert.AreEqual(DateTime.Parse("Thu, 29 Apr 2021 14:16:00 GMT"), response.LastMemento.DateTime);
        }

        [TestMethod]
        public async Task Check_link_for_chosen_date()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://www.bbc.co.uk/"));

            var memento = response.ArchiveMementos.SingleOrDefault(a => a.DateTime == DateTime.Parse("Sun, 31 May 2020 16:58:42 GMT"));

            Assert.AreEqual(new Uri("http://archive.md/20200531165842/https://www.bbc.co.uk/"), memento.Uri);
        }
    }
}
