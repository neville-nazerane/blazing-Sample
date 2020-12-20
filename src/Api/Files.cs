using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage.Blob;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Api
{
    public static class Files
    {

        [FunctionName("uploadData")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uploadData/{filename}")] HttpRequest req,
            [Blob("files/{filename}", FileAccess.Write)] CloudBlockBlob blob,
            ILogger log)
        {
            blob.Properties.ContentType = req.Headers["type"];
            using var stream = await blob.OpenWriteAsync();
            await req.Body.CopyToAsync(stream);
            await stream.CommitAsync();
        }
    }
}
