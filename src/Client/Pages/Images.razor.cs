using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Client.Pages
{
    public partial class Images
    {

        const string format = "image/png";

        private string imageDataUrl;
        private IBrowserFile selectedFile;

        string[] files;

        [Inject]
        public HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await PullListAsync();
        }

        private async Task OnImageUploadedAsync(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
            var resizedImageFile = await e.File.RequestImageFileAsync(format, 100, 100);
            var buffer = new byte[resizedImageFile.Size];
            await resizedImageFile.OpenReadStream().ReadAsync(buffer);
            imageDataUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
        }

        private async Task UploadToBlobAsync()
        {
            using var stream = selectedFile.OpenReadStream();
            var content = new StreamContent(stream);
            content.Headers.Add("type", selectedFile.ContentType);
            await HttpClient.PostAsync($"api/uploadData/{selectedFile.Name}", content);
            await PullListAsync();
        }

        async Task PullListAsync()
        {
            files = await HttpClient.GetFromJsonAsync<string[]>("api/listData");
        }

        async Task DeleteAsync(string file)
        {
            var name = file.Split("\\/".ToCharArray()).Last();
            await HttpClient.DeleteAsync($"api/deleteData/{name}");
            await PullListAsync();
        }

    }
}
