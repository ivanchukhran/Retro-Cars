using RetroCarsWebApp.Services;
using Microsoft.ML;
using RetroCarsWebApp.Classification;

var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(
    new UserService("Users.json")
);
builder.Services.AddSingleton(
    new CarService("Cars.json")
);


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
    {
        option.IdleTimeout = TimeSpan.FromMinutes(30);
        option.Cookie.Name = ".RetroCars.Session";
    }
);

builder.Services.AddSingleton<MLContext>();
builder.Services.AddSingleton<CarModelTrainingService>();
builder.Services.AddSingleton<PredictionService>(provider =>
{
    var context = provider.GetService<MLContext>();
    var carModelTrainingService = provider.GetService<CarModelTrainingService>();
    var modelPath = "/home/ivan/RiderProjects/CarClassifier/Data/car-model.zip";
    return new PredictionService(context, carModelTrainingService, modelPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();


app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();