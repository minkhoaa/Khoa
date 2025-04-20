using Foodify_DoAn.Data;
using Foodify_DoAn.Model;
using Foodify_DoAn.Repository;
using Foodify_DoAn.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Net;
using System.Text;
using FluentEmail.Core;
using FluentEmail.Smtp;
using System.Threading.RateLimiting;
using Sprache;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

DotNetEnv.Env.Load();

builder.Services.AddCors(option => option.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Configuration["ConnectionStrings:MyDB"] = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__MYDB");

builder.Services.AddDbContext<FoodifyContext>(option => option.UseNpgsql(builder.Configuration.GetConnectionString("MyDB")));
builder.Services.AddIdentity<TaiKhoan, VaiTro>()
    .AddEntityFrameworkStores<FoodifyContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IAccountRepository, AccountManager>();
builder.Services.AddScoped<IEmailRepository, EmailService>();


builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSingleton(option => {
    var settings = option.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(
        settings.CloudName,
        settings.ApiKey,
        settings.ApiSecret
        );
    return new Cloudinary(account);

});


// Update the AutoMapper configuration to use an instance of DbMapper instead of the type itself.
builder.Services.AddAutoMapper(config => config.AddProfile(new DbMapper()));
builder.Services.AddScoped<IRecipeRepository, RecipeService>(); 
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.RequireHttpsMetadata = false;
    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
    };
});
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddStackExchangeRedisCache(options => options.Configuration = builder.Configuration.GetConnectionString("Redis"));

var emailConfig = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();

builder.Services
    .AddFluentEmail(emailConfig.SenderEmail, "Your Name")
    .AddSmtpSender(new SmtpClient(emailConfig.SmtpServer)
    {
        Port = emailConfig.SmtpPort,
        Credentials = new NetworkCredential(emailConfig.SenderEmail, emailConfig.SenderPassword),
        EnableSsl = emailConfig.EnableSSL,
    });
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new() { Title = "Foodify API", Version = "v1" });

    // Ánh xạ IFormFile thành string/binary
    option.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    // Giữ OperationFilter nếu cần
    option.OperationFilter<FileUploadOperationFilter>();
});

var app = builder.Build();


var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors();
app.Run();
