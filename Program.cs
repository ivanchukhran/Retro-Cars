using RetroCarsWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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