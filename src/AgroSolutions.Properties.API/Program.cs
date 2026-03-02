using AgroSolutions.AlertsProcessor.Workers;
using AgroSolutions.Properties.API.Middleware;
using AgroSolutions.Properties.Application.Interfaces;
using AgroSolutions.Properties.Application.Services;
using AgroSolutions.Properties.Data.Repositories;
using AgroSolutions.Properties.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Options
builder.Services.Configure<InfluxOptions>(builder.Configuration.GetSection(nameof(InfluxOptions)));
builder.Services.Configure<JobOptions>(builder.Configuration.GetSection(nameof(JobOptions)));

// ===== SERVICES =====

builder.Services.AddControllers();

// Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AgroSolutions Property Management API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Digite: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//DI
string conStr = builder.Configuration.GetConnectionString("Properties") ?? string.Empty;

builder.Services.AddDbContext<PropertiesContext>(options =>
    options.UseSqlServer(conStr));

builder
    .Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder
    .Services.AddScoped<ICultureRepository, CultureRepository>();
builder
    .Services.AddScoped<IFieldRepository, FieldRepository>();

builder
    .Services.AddScoped<IPropertyService, PropertyService>();
builder
    .Services.AddScoped<ICultureService, CultureService>();
builder
    .Services.AddScoped<IFieldService, FieldService>();
builder
    .Services.AddScoped<IGenerateAlertService, GenerateAlertService>();


// Infra + App
builder.Services.AddScoped<IReadingsRepository, InfluxReadingsRepository>();
builder.Services.AddScoped<IFieldRepository, FieldRepository>();




// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health Checks
builder.Services.AddHealthChecks();
builder.Services.AddHostedService<AlertsWorker>();
var app = builder.Build();


// ===== APLICAR MIGRATIONS E SEED =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PropertiesContext>();
    db.Database.EnsureCreated();
    db.Database.Migrate();
}

// ===== MIDDLEWARE =====

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseJwtAuthentication();

app.MapControllers();
app.MapHealthChecks("/healthz");

app.Run();
