using Azure.Storage.Blobs;
using FreshFalaye.Pos.Shared.Abstractions;

namespace FreshFalaye.Pos.Shared.Helpers
{
    public class PosImageDownloader
    {
        private readonly IFileEnvironment _env;
        private readonly HttpClient _http;

        public PosImageDownloader(IFileEnvironment env, HttpClient http)
        {
            _env = env;
            _http = http;
        }

        
        public async Task<string?> DownloadImageAsync(string imageUrl, string fileName)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            try
            {
                //// Resolve platform-specific images directory
                //var imagesFolder = _env.ImagesPath;
                //Directory.CreateDirectory(imagesFolder);

                //var localFilePath = Path.Combine(imagesFolder, fileName);

                //// If already downloaded, reuse
                //if (File.Exists(localFilePath))
                //    return localFilePath;

                //// 🔥 Plain HTTP download (NO Azure SDK)
                //using var response = await _http.GetAsync(imageUrl);
                //if (!response.IsSuccessStatusCode)
                //    return null;

                //await using var input = await response.Content.ReadAsStreamAsync();
                //await using var output = File.Create(localFilePath);
                //await input.CopyToAsync(output);

                //return localFilePath;

                // 1️⃣ Raw download folder (NOT rendered directly)
                Directory.CreateDirectory(_env.ImagesPath);
                var downloadedFilePath = Path.Combine(_env.ImagesPath, fileName);

                // 2️⃣ Blazor-served folder (THIS is what <img> uses)
                Directory.CreateDirectory(_env.BlazorImagesPath);
                var blazorFilePath = Path.Combine(_env.BlazorImagesPath, fileName);

                // 3️⃣ If already available for Blazor, reuse
                if (File.Exists(blazorFilePath))
                {
                    return blazorFilePath;// $"images/products/{fileName}";
                }

                // 4️⃣ Download only if missing
                if (!File.Exists(downloadedFilePath))
                {
                    using var response = await _http.GetAsync(imageUrl);
                    if (!response.IsSuccessStatusCode)
                        return null;

                    await using var input = await response.Content.ReadAsStreamAsync();
                    await using var output = File.Create(downloadedFilePath);
                    await input.CopyToAsync(output);
                }

                // 5️⃣ Copy into Blazor folder
                File.Copy(downloadedFilePath, blazorFilePath, overwrite: true);

                // 6️⃣ Return Blazor-safe relative URL
                return blazorFilePath;// $"images/products/{fileName}";
            }
            catch
            {
                // POS must NEVER crash due to image failure
                return null;
            }
        }
    }
}
