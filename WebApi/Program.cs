using Application;
using Authorization;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Persistence;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Реєстрація шарів
builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddApplication();
builder.Services.AddAuthorizationServices();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "streaming-auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.EventsType = typeof(AppCookieEvents);
    });

const string frontendOrigin = "_frontendOrigin";

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(frontendOrigin, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors(frontendOrigin);
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler(_ => { });
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
