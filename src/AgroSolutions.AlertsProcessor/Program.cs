using AgroSolutions.AlertsProcessor.Workers;
using AgroSolutions.Properties.Data.Repositories;
using AgroSolutions.Properties.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Options
builder.Services.Configure<InfluxOptions>(builder.Configuration.GetSection("Influx"));
builder.Services.Configure<JobOptions>(builder.Configuration.GetSection("Job"));

// Infra + App
builder.Services.AddSingleton<IReadingsRepository, InfluxReadingsRepository>();
builder.Services.AddSingleton<IFieldRepository, FieldRepository>();


var host = builder.Build();

using var scope = host.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<AlertsWorker>();

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
return await runner.RunAsync(lifetime.ApplicationStopping);