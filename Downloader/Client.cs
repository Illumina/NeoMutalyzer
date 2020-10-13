using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Downloader
{
    public sealed class Client : IClient
    {
        private readonly HttpClient _httpClient;

        public Client(string hostName)
        {
            var baseUri = new Uri($"http://{hostName}");

            ServicePointManager.DefaultConnectionLimit                           = int.MaxValue;
            ServicePointManager.FindServicePoint(baseUri).ConnectionLeaseTimeout = 60 * 1000;

            _httpClient = new HttpClient { BaseAddress = baseUri };
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
        }

        public bool DownloadFile(RemoteFile file)
        {
            using HttpResponseMessage response =
                _httpClient.GetAsync(file.RemotePath, HttpCompletionOption.ResponseHeadersRead).Result;

            Console.WriteLine($"  - thread {Thread.CurrentThread.ManagedThreadId}, status code: {response.StatusCode}");
            if (response.StatusCode == HttpStatusCode.TooManyRequests) Thread.Sleep(5_000);
            if (!response.IsSuccessStatusCode) return false;

            Console.WriteLine($"  - downloading {file.Description}");

            Stream stream   = response.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var    fileInfo = new FileInfo(file.LocalPath);

            using FileStream fileStream = fileInfo.OpenWrite();
            stream.CopyTo(fileStream);
            
            Thread.Sleep(5_000);

            return true;
        }
    }
}