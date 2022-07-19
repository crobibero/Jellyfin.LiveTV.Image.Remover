using Jellyfin.Sdk;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var sdkSettings = new SdkClientSettings
{
    BaseUrl = config["Jellyfin:Uri"],
    AccessToken = config["Jellyfin:ApiKey"]
};

var httpClient = new HttpClient();
ILiveTvClient liveTvClient = new LiveTvClient(sdkSettings, httpClient);
IImageClient imageClient = new ImageClient(sdkSettings, httpClient);

var channelResult = await liveTvClient.GetLiveTvChannelsAsync();

foreach (var channel in channelResult.Items)
{
    try
    {
        var imageInfos = await imageClient.GetItemImageInfosAsync(channel.Id);
        foreach (var image in imageInfos)
        {
            try
            {
                Console.WriteLine($"Deleting {image.ImageType} image for {channel.Id} ({channel.Name})");
                await imageClient.DeleteItemImageAsync(channel.Id, image.ImageType);
            }
            catch (ImageException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

        }
    }
    catch (ImageException ex)
    {
        Console.Error.WriteLine(ex.Message);
    }
}
