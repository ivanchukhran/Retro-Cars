using RetroCarsWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
const string connectionString = "mongodb://root:example@localhost:27017";

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<UserService>(
    new UserService(connectionString, "Users")
    );
builder.Services.AddSingleton<CarService>(
    new CarService(connectionString, "Cars")
    );
builder.Services.AddSingleton<SessionService>(
    new SessionService(connectionString, "Sessions")
    );

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();