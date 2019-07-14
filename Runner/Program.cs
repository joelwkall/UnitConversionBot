using System;
using System.IO;
using Conversion;
using Conversion.Converters;
using Conversion.Scanners;
using Core;
using Imgur;
using Microsoft.Extensions.Configuration;

namespace ConsoleRunner
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var clientId = configuration.GetConnectionString("ImgurClientId");
            var clientSecret = configuration.GetConnectionString("ImgurClientSecret");
            var refreshToken = configuration.GetConnectionString("ImgurRefreshToken");

            var client = File.Exists("prod.txt")
                ? new LiveImgurConnection(clientId, clientSecret, refreshToken)
                : new LurkerImgurConnection(clientId, clientSecret, refreshToken);

            var analyzer = new ImgurAnalyzer(
                TextAnalyzer.Default,
                client
            );

            analyzer.Run();
        }
    }
}
