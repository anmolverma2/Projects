using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PracticeAPI.Common;
using PracticeAPI.Configrations;
using PracticeAPI.Context;
using PracticeAPI.Logs;
using PracticeAPI.Model;
using PracticeAPI.Repositories;
using PracticeAPI.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File("Log/log.txt",rollingInterval:RollingInterval.Day).CreateLogger();
// Add services to the container.
//builder.Host.UseSerilog();
builder.Logging.AddSerilog();
builder.Services.AddControllers();
//builder.Services.AddControllers(options => options.ReturnHttpNotAcceptable = true).AddNewtonsoftJson().AddXmlSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DBCon")));

builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSetting"));

builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

builder.Services.AddEndpointsApiExplorer();

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JwtBearer:SecretKey"));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        //  ValidateIssuer = true,
        //  ValidIssuer = issuer

        //  ValidAudience = true,
        // ValidAudience = audience,
        ValidateAudience = false

    };
    //options.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        var authHeader = context.Request.Headers["Authorization"].ToString();
    //        if (!string.IsNullOrEmpty(authHeader) && !authHeader.StartsWith("Bearer "))
    //        {
    //            context.Token = authHeader; // Use token without "Bearer" prefix
    //        }
    //        return Task.CompletedTask;
    //    },
    //    OnAuthenticationFailed = context =>
    //    {
    //        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
    //        return Task.CompletedTask;
    //    },
    //    OnTokenValidated = context =>
    //    {
    //        Console.WriteLine($"Token validated for user: {context.Principal.Identity.Name}");
    //        return Task.CompletedTask;
    //    }
    //};
});
//.AddJwtBearer("LocatJwt", options =>
//{
//    //options.RequireHttpsMetadata = false;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ValidateIssuer = false,
//        //  ValidateIssuer = true,
//        //  ValidIssuer = issuer

//        //  ValidAudience = true,
//        // ValidAudience = audience,
//        ValidateAudience = false

//    };
//});


builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "Jwt authentication using bearer [token]",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header

            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
  
    options.AddPolicy("MyPolicy", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    options.AddPolicy("localPolicy", policy => policy.WithOrigins("http://localhost:4200/").AllowAnyHeader().AllowAnyMethod());

    options.AddPolicy("GooglePolicy", options => options.WithOrigins("https://google.com","https://gmail.com","https://googledrive.com"));
}
//policy.WithOrigins("https://localhost:4200")
);

builder.Services.AddTransient<Microsoft.Extensions.Logging.ILogger,LogFromServer>();
builder.Services.AddTransient<ICollegeRepository,CollegeRepository>();
builder.Services.AddTransient(typeof(ICommonRepository<>), typeof(CommonRepository<>));
builder.Services.AddTransient<IUserService,UserService>();
builder.Services.AddTransient<IEmailService,EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
