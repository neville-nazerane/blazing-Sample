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
using Microsoft.AspNetCore.Mvc;

namespace Api
{
    public static class Files
    {

        [FunctionName("uploadData")]
        public static async Task UploadData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uploadData/{filename}")] HttpRequest req,
            [Blob("files/{filename}", FileAccess.Write)] CloudBlockBlob blob,
            ILogger log)
        {
            log.LogInformation("Writing blob named " + blob.Name);
            blob.Properties.ContentType = req.Headers["type"];
            using var stream = await blob.OpenWriteAsync();
            await req.Body.CopyToAsync(stream);
            await stream.CommitAsync();
        }

        [FunctionName("listData")]
        public static async Task<IActionResult> ListData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req,
            [Blob("files", FileAccess.Write)]CloudBlobContainer container)
        {
            var files = container.ListBlobs().Select(c => c.Uri.AbsoluteUri).ToArray();
            return new OkObjectResult(files);
        }

        [FunctionName("deleteData")]
        public static async Task DeleteData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "deleteData/{filename}")] HttpRequest req,
            [Blob("files/{filename}", FileAccess.Write)] CloudBlockBlob blob)
        {
            await blob.DeleteAsync();
        }

    }
}
