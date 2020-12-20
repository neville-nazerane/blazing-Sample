using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Client.Pages
{
    public partial class Images
    {

        const string format = "image/png";

        private string imageDataUrl;
        private IBrowserFile selectedFile;

        [Inject]
        public HttpClient HttpClient { get; set; }

        private async Task OnImageUploaded(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
            var resizedImageFile = await e.File.RequestImageFileAsync(format, 100, 100);
            var buffer = new byte[resizedImageFile.Size];
            await resizedImageFile.OpenReadStream().ReadAsync(buffer);
            imageDataUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
        }

        private async Task UploadToBlob()
        {
            using var stream = selectedFile.OpenReadStream();
            var content = new StreamContent(stream);
            content.Headers.Add("type", selectedFile.ContentType);
            await HttpClient.PostAsync($"api/uploadData/{selectedFile.Name}", content);
        }

    }
}
