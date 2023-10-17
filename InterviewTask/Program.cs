using InterviewTask;
using InterviewTask.DataBase;
using InterviewTask.Middlewares;
using InterviewTask.Services;
using InterviewTask.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection(StorageOptions.Title));
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IFriendshipsService, FriendshipsService>();
builder.Services.AddTransient<IUsersService, UsersService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContextPool<UsersContext>(x => { x.UseNpgsql(connectionString); });

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Basic Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();