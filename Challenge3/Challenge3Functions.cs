using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Challenge3
{
    public class Challenge3Functions
    {
        [FunctionName("SaveImagesFromGithubRepository")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "github/push")] HttpRequest request,
            [CosmosDB(
                databaseName: "25DaysOfServerless",
                collectionName: "SecretSantaImages",
                ConnectionStringSetting = "ConnectionStringCosmosDb")] IAsyncCollector<Image> outputCollector)
        {
            var payload = await GetRequestPayload(request);

            var contentsUrl = ((Newtonsoft.Json.Linq.JValue)payload.repository.contents_url).ToObject<string>();

            contentsUrl = contentsUrl.Substring(0, contentsUrl.LastIndexOf("/", StringComparison.Ordinal));

            foreach (var commit in payload.commits)
            {
                var added = ((Newtonsoft.Json.Linq.JArray)commit.added).ToObject<List<string>>();

                await Task.WhenAll(added?.Where(x => x.EndsWith(".png"))
                    .Select(relativeUrl => contentsUrl + "/" + relativeUrl)
                    .Select(x => new Image
                    {
                        ImageUrl = x
                    })
                    .Select(x => outputCollector.AddAsync(x)));
            }

            return new OkResult();
        }

        private static async Task<dynamic> GetRequestPayload(HttpRequest req)
        {
            using var streamReader = new StreamReader(req.Body);

            var requestBody = await streamReader.ReadToEndAsync();

            return JsonConvert.DeserializeObject(requestBody);
        }
    }
}
