using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Salik_Bug_Tracker_API.Data;
using Salik_Bug_Tracker_API.Data.Repository.IRepository;
using Salik_Bug_Tracker_API.Data.Repository;
using Salik_Bug_Tracker_API.Models;
using System;
using System.Text;
using Salik_Bug_Tracker_API.Models.Helpers;
using Salik_Bug_Tracker_API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Logging.ClearProviders();

builder.Services.AddMemoryCache();

builder.Host.UseSerilog((context, loggerConfig) => {
    loggerConfig
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console()
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .Enrich.With<ActivityEnricher>()
    .WriteTo.File("log.txt");
   
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),

    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["JWT:Issuer"],

    ValidateAudience = true,
    ValidAudience = builder.Configuration["JWT:Audience"],

    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})           //Add JWT Bearer
.AddJwtBearer(options =>
{
 options.SaveToken = true;
 options.RequireHttpsMetadata = false;
 options.TokenValidationParameters = tokenValidationParameters;

    
});
builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
    options.Filters.Add(new AuthorizeFilter());
})
.AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


var apiVersionDescriptionProvider =builder.Services.BuildServiceProvider()
  .GetService<IApiVersionDescriptionProvider>();

builder.Services.AddSwaggerGen(C =>
{
    C.EnableAnnotations();

    C.SwaggerDoc("v1", new OpenApiInfo { Title = "SalikBugTracker.API", Version = "v1" });
    C.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    C.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    C.IncludeXmlComments(xmlPath);




});




builder.Services.AddApiVersioning(options => { options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.ReportApiVersions = true;
});
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
var app = builder.Build();

app.UseApiVersioning();
await EnsureDb(app.Services, app.Logger);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(C=>C.SwaggerEndpoint("/swagger/v1/swagger.json", "Bug Tracker.API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await using var scope = app.Services.CreateAsyncScope();
using var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
await EnsureDb(app.Services, app.Logger);

//seeding the roles to the database if it didnt exist 
AppDbInitializer.SeedRolesToDb(app).Wait();

app.Run();

async Task EnsureDb(IServiceProvider services, Microsoft.Extensions.Logging.ILogger logger)
{
    using var db = services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (db.Database.IsRelational())
    {
        logger.LogInformation("Ensuring database exists and is up to date at connection string '{connectionString}'", connectionString);
        //await db.Database.EnsureCreatedAsync();
        await db.Database.MigrateAsync();
    }
}


// Make the implicit Program class public so test projects can access it
public partial class Program { }