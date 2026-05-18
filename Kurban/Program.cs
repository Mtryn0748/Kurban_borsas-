using Microsoft.EntityFrameworkCore;
// KurbanlikContext hangi klasördeyse o namespace
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//-----------------------------------------------Builder-------------------------------------//
// aşağıdaki kod sayesinde uygulama başladığında veritabanı otomatik olarak oluşturulur ve varsa yeni migration'lar uygulanır

// DbContext'i sisteme tanıtıyoruz
builder.Services.AddDbContext<KurbanlikContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("baglan")));




builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login"; // Giriş yapmayanların yönlendirileceği sayfa
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // 20 dakika işlem yapılmazsa oturumu kapat
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
app.UseRouting();

app.UseAuthentication(); // "Sen kimsin?" (Kimlik Doğrulama)
app.UseAuthorization();  // "Bu sayfaya girmeye yetkin var mı?" (Yetkilendirme)

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
