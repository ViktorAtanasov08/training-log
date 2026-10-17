using Microsoft.AspNetCore.DataProtection;
using TrainingLog.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<WorkoutLog>();
// Keep this local prototype's form-protection keys outside source control.
builder.Services.AddDataProtection().PersistKeysToFileSystem(
    new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".local-keys")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures("en-US")
    .AddSupportedUICultures("en-US"));
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Workouts}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
