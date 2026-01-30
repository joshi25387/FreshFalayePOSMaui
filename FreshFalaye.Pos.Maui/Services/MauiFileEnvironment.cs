using FreshFalaye.Pos.Shared.Abstractions;

namespace FreshFalaye.Pos.Maui.Services
{
    public class MauiFileEnvironment : IFileEnvironment
    {
        public string ImagesPath =>
            Path.Combine(FileSystem.AppDataDirectory, "images", "products");

        public string BlazorImagesPath =>
            Path.Combine(
                FileSystem.AppDataDirectory,
                "wwwroot",
                "images",
                "products");
    }
}
