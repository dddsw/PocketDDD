using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using PocketDDD.Server.DB;
using PocketDDD.Server.Model.DBModel;
using PocketDDD.Server.Services;
using PocketDDD.Server.WebAPI;
using PocketDDD.Server.WebAPI.Authentication;

var corsPolicy = "corsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation())
    .UseOtlpExporter();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy,
        builder =>
        {
            builder.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});

builder.Services.AddDbContext<PocketDDDContext>(
    options => options.UseSqlServer("name=ConnectionStrings:PocketDDDContext")
);

builder.Services.AddMemoryCache();

builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<EventDataService>();
builder.Services.AddScoped<PrizeDrawService>();
builder.Services.AddScoped<SpeakersService>();

builder.Services.AddHttpClient<SessionizeService>();

builder.Services.AddHostedService<UpdateFromSessionizeBackgroundService>();

builder.Services.AddAuthentication()
    .AddScheme<UserIsRegisteredOptions, UserIsRegisteredAuthHandler>(UserIsRegisteredAuthHandler.SchemeName, null);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<PocketDDDContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PocketDDDContext>();
    await db.Database.MigrateAsync();

    if (!await db.EventDetail.AnyAsync())
    {
        db.EventDetail.Add(new EventDetail { SessionizeId = "dev-event", Version = 0 });
        await db.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicy);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz");

app.Run();