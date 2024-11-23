using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using API_for_Uploading_Large_Files;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<S3Service>();
//builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions("AWS"));
// Register IAmazonS3 client
//builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = builder.Configuration.GetSection("AWS");
    var credentials = new BasicAWSCredentials(config["AccessKey"], config["SecretKey"]);
    var region = RegionEndpoint.GetBySystemName(config["Region"]);

    return new AmazonS3Client(credentials, region);
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FileUploadTrack}/{action=Index}/{id?}");

app.UseAuthorization();

app.MapRazorPages();

app.Run();
