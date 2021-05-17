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
    /// Mocks the HttpClient to read from same-first-last-memento.txt
    /// </summary>
    [TestClass]
    public class ArchiveTodaySameFirstLastMementoTests
    {
        private ArchiveTodayClient _archiveTodayClient;

        [TestInitialize]
        public void Init()
        {
            var archiveExample = System.IO.File.ReadAllText("./files/same-first-last-memento.txt");

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
        public async Task Number_of_Mementos_Equals_1()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://test.localdev"));

            Assert.AreEqual(1, response.ArchiveMementos.Count);
        }

        [TestMethod]
        public async Task First_memento_link()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://test.localdev"));

            Assert.AreEqual(new Uri("http://archive.md/20210507123456/https://test.localdev"), response.FirstMemento.Uri);
        }

        [TestMethod]
        public async Task First_memento_datetime()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://test.localdev"));

            Assert.AreEqual(DateTime.Parse("Fri, 07 May 2021 12:34:56 GMT"), response.FirstMemento.DateTime);
        }

        [TestMethod]
        public async Task Last_memento_link()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://test.localdev"));

            Assert.AreEqual(new Uri("http://archive.md/20210507123456/https://test.localdev"), response.LastMemento.Uri);
        }

        [TestMethod]
        public async Task Last_memento_datetime()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://test.localdev"));

            Assert.AreEqual(DateTime.Parse("Fri, 07 May 2021 12:34:56 GMT"), response.LastMemento.DateTime);
        }

        [TestMethod]
        public async Task Check_link_for_chosen_date()
        {
            var response = await _archiveTodayClient.GetTimemapAsync(new Uri("https://test.localdev"));

            var memento = response.ArchiveMementos.SingleOrDefault(a => a.DateTime == DateTime.Parse("Fri, 07 May 2021 12:34:56 GMT"));

            Assert.AreEqual(new Uri("http://archive.md/20210507123456/https://test.localdev"), memento.Uri);
        }
    }
}
