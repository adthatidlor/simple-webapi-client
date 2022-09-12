using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace WebAPIClient
{
    class Program
    {
        private static HttpClientHandler clientHandler = newHandler();
        private static readonly HttpClient client = new HttpClient(clientHandler, true);

        static HttpClientHandler newHandler()
        {
            return new HttpClientHandler
            {
                Proxy = HttpClient.DefaultProxy,
                UseDefaultCredentials = false
            };
        }
        static async Task Main(string[] args)
        {
            Thread.Sleep(3000);
            Console.WriteLine("user Proxy: {0} ", clientHandler.UseProxy);
            Console.WriteLine("{0} : proxy", clientHandler.Proxy);
            WebProxy pp = new WebProxy();
            try
            {
                var repositories = await ProcessRepositories();

                foreach (var repo in repositories)
                {
                    Console.WriteLine("Repo: {0}, Url: {2}",
                    repo.Name,
                    repo.Description,
                    repo.GitHubHomeUrl,
                    repo.Homepage,
                    repo.Watchers);
                    Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
        private static async Task<List<Repository>> ProcessRepositories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            var streamTask = client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
            var repositories = await JsonSerializer.DeserializeAsync<List<Repository>>(await streamTask);
            return repositories;
        }
    }
    public class Repository
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("html_url")]
        public Uri GitHubHomeUrl { get; set; }

        [JsonPropertyName("homepage")]
        public Uri Homepage { get; set; }

        [JsonPropertyName("watchers")]
        public int Watchers { get; set; }
    }
}