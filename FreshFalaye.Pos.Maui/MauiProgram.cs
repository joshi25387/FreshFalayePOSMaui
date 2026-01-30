using FreshFalaye.Pos.Maui.Services;
using FreshFalaye.Pos.Shared.Abstractions;
using FreshFalaye.Pos.Shared.Data;
using FreshFalaye.Pos.Shared.Helpers;
using FreshFalaye.Pos.Shared.Models;
using FreshFalaye.Pos.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;
using Radzen;


namespace FreshFalaye.Pos.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            QuestPDF.Settings.License = LicenseType.Community;
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<AuthTokenHandler>();

            // 🔹 HTTP clients (Azure API)
            builder.Services.AddHttpClient("Api", client =>
            {
                client.BaseAddress = new Uri("https://freshfalaye-api-bjgkd4bdgaaaebd2.centralindia-01.azurewebsites.net");
            }).AddHttpMessageHandler<AuthTokenHandler>();

            builder.Services.AddHttpClient("ApiAuth", client =>
            {
                client.BaseAddress = new Uri("https://freshfalaye-api-bjgkd4bdgaaaebd2.centralindia-01.azurewebsites.net");
            });

            // 🔹 Platform services
            builder.Services.AddSingleton<IFileEnvironment, MauiFileEnvironment>();
            builder.Services.AddSingleton<ILocalAuthStore, MauiAuthStore>();

            // 🔹 Image downloader
            builder.Services.AddHttpClient<PosImageDownloader>();

            // 🔹 POS services
            builder.Services.AddScoped<PosApiService>();
            builder.Services.AddScoped<PosAuthService>();
            builder.Services.AddScoped<PosSyncService>();
            builder.Services.AddSingleton<ApiTokenStore>();
            builder.Services.AddScoped<BillNumberService>();
            builder.Services.AddScoped<PosSaleService>();

            #if WINDOWS
            builder.Services.AddSingleton<WindowsThermalPrinter>();
            builder.Services.AddScoped<IReceiptPrinter, MauiReceiptPrinter>();
            #endif

            builder.Services.AddScoped<ReceiptPdfService>();

            builder.Services.AddRadzenComponents();

            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();

            builder.Services.AddSingleton(new ApiSettings
            {
                BaseUrl = "https://your-api-base-url"
            });

            // 🔹 Local SQL Server DB
            builder.Services.AddDbContext<PosDbContext>(options =>
                options.UseSqlServer(
                    "Server=DESKTOP-V3QC17D\\SQLEXPRESS;Database=FreshFalayePosDb;Trusted_Connection=True;TrustServerCertificate=True"));



            return builder.Build();
        }
    }
}
