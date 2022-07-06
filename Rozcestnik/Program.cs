//https://www.youtube.com/watch?v=BWa7Mu-oMHk&ab_channel=TonySpencer

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Services pro login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Defaultní authentizace = cookie (musím pøedat schéma)
    .AddCookie(options => {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Home/Index";

        // Po 60s neaktivitì se smaže cookie a je vyžadováno nové pøihlášení
        options.ExpireTimeSpan = TimeSpan.FromSeconds(60);
        options.Cookie.MaxAge = options.ExpireTimeSpan; 
        options.SlidingExpiration = true;

        /*
        options.Events = new CookieAuthenticationEvents()
        {
            OnSignedIn = async context => // Lze pøidat i další
            {
                await = Task.CompletedTask;
            }
        };
        */
    }); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
