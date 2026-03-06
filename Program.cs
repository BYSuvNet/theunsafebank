using Microsoft.EntityFrameworkCore;
using theunsafebank.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BankContext>(options =>
    options.UseSqlite("Data Source=bank.db"));

// Add session support (insecure - no encryption, just basic storage)
builder.Services.AddDistributedMemoryCache(); // In-memory cache for sessions
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true; // INSECURE!
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BankContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
// }

// INSECURE!
// app.UseHttpsRedirection();

app.UsePathBase("/bank"); //Eftersom siten servas under suvnet.se/bank

app.UseRouting();
app.UseSession(); // FIXME: Session is not properly configured, and not used in controllers
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
