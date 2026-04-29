using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Trace;
using PocketDDD.Server.DB;
using PocketDDD.Server.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource(Worker.ActivitySourceName)
        .AddSqlClientInstrumentation())
    .UseOtlpExporter();

builder.Services.AddDbContext<PocketDDDContext>(options =>
    options.UseSqlServer("name=ConnectionStrings:PocketDDDContext"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
