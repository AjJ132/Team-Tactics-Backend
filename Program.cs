using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamTacticsBackend.Database;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//-------------------------------------------------------------
//To swap between SQL Server and PostgreSQL, comment out the other and uncomment the one you want to use and then go edit Team-Tactics-Backend.csproj and comment out the Npgsql and uncomment the SQL Server package
//if you would like to use the postgresql server, start docker and then run the autorun.sh or autrun.bat file in Database/Docker folder of this project. username and password details are in the docker file
//!!IMPORTANT!!

//US: DBA , PS: Capstone123 <- - this is the password for the database server
// builder.Services.AddDbContextFactory<TeamTacticsDBContext>(options =>
//     options.UseSqlServer(
//         configuration.GetConnectionString("AzureSQLConnection")));

builder.Services.AddDbContextFactory<TeamTacticsDBContext>(options =>
options.UseNpgsql(
configuration.GetConnectionString("DefaultConnection"),
npgsqlOptionsAction: npgsqlOptions =>
{
    npgsqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorCodesToAdd: null);
}));

//!!IMPORTANT!!
//-------------------------------------------------------------

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
