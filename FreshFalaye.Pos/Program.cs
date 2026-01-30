using Blazored.LocalStorage;
using FreshFalaye.Pos.Shared.Abstractions;
using FreshFalaye.Pos.Shared.Data;
using FreshFalaye.Pos.Shared.Helpers;
using FreshFalaye.Pos.Shared.Models;
using FreshFalaye.Pos.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Radzen;

var builder = WebApplication.CreateBuilder(args);
var urls = builder.Configuration["Urls"];

if (!string.IsNullOrEmpty(urls))
{
    builder.WebHost.UseUrls(urls);
}

builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddSingleton<ApiSettings>(sp =>
    sp.GetRequiredService<IOptions<ApiSettings>>().Value);

// 🔐 Client ONLY for token exchange (NO handler)
builder.Services.AddHttpClient("ApiAuth", (sp,client) =>
{
    var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});

builder.Services.AddTransient<AuthTokenHandler>();
builder.Services.AddHttpClient("Api", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
}).AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddDbContext<PosDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PosDb")));




// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddRadzenComponents();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();


builder.Services.AddScoped<PosApiService>();
builder.Services.AddScoped<PosSyncService>();
builder.Services.AddScoped<BillNumberService>();
builder.Services.AddScoped<PosSaleService>();

builder.Services.AddSingleton<ApiTokenStore>();

builder.Services.AddScoped<PosAuthService>();
builder.Services.AddScoped<IdleTimeoutService>();



builder.Services.AddScoped<ILocalAuthStore, BlazoredAuthStore>();

var app = builder.Build();




using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();

    if (!db.LocalUsers.Any())
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var pwd = Convert.ToBase64String(
            sha.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes("1234")));

        db.LocalUsers.Add(new LocalUser
        {
            Username = "admin",
            PasswordHash = pwd,
            PinHash = SecurityHelper.Hash("1234"),
            Role = "Cashier",
            IsActive = true
        });
       

        db.SaveChanges();
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
