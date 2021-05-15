# ArchiveTodayClient

A basic example is as follows:

```csharp
var httpClient = new HttpClient();

var archiveTodayClient = new ArchiveTodayClient(httpClient);

var response = await archiveTodayClient.GetTimemapAsync(new Uri("https://www.bbc.co.uk/"));
```

Note: This client only supports retrieving the Timemap. Submitting new pages is not supported as it is forbidden to do so outside of the site. 