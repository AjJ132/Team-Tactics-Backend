using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Team_Tactics_Backend.Database;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//US: DBA , PS: Capstone123 <- - this is the password for the database server
builder.Services.AddDbContextFactory<TeamTacticsDBContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection")));

//Ensure created 

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<TeamTacticsDBContext>();

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
// })
// .AddCookie(options =>
// {
//     options.LoginPath = "/login"; // Your login path
//     options.LogoutPath = "/logout"; // Your logout path

//     // Set SameSite and Secure Policy
//     options.Cookie.Name = "Authorization";
//     options.Cookie.SameSite = SameSiteMode.None; // Set SameSite to None
//     options.Cookie.HttpOnly = true; // Set HttpOnly to true
//     options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Set SecurePolicy to Always
//     options.ExpireTimeSpan = TimeSpan.FromDays(5); // Set cookie expiration
// });


builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
    builder =>
    {
        builder.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<TeamTacticsDBContext>();
        context.Database.EnsureCreated();
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("MyAllowSpecificOrigins");


app.MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
