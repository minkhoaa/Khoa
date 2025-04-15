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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(option => option.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddDbContext<FoodifyContext>(option => option.UseNpgsql(builder.Configuration.GetConnectionString("MyDB")));
builder.Services.AddIdentity<TaiKhoan, VaiTro>()
    .AddEntityFrameworkStores<FoodifyContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IAccountRepository, AccountManager>();
builder.Services.AddScoped<IEmailRepository, EmailService>();


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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
