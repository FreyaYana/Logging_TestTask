using Microsoft.AspNetCore.Authentication;
using Serilog;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using LoggingMicroservice.Infrastructure.Options;
using LoggingMicroservice.Logic.Abstract;
using LoggingMicroservice.Logic;

// Конфигурация Serilog с использованием значений из конфигурации
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var logFilePath = configuration["LoggingInfo:LogFileDirectory"] + configuration["LoggingInfo:LogFilePath"];
var logFileSizeLimitBytes = configuration.GetValue<long>("LoggingInfo:LogFileSizeLimitBytes");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        path: logFilePath ?? "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: logFileSizeLimitBytes,
        rollOnFileSizeLimit: true)
    .CreateLogger();

Log.Information("Starting up");

try
{
    builder.Services.AddSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    });


    builder.Services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAuthenticatedUser", policy =>
            policy.RequireAuthenticatedUser());
    });

    builder.Services.AddSwaggerGen(options =>
    {
        var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        options.IncludeXmlComments(xmlPath);

        options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                Description = "BasicAuthentication header using the Bearer scheme."
            });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Basic"
                        }
                    },
                    new string[] {}
                }
            });

        // Описание метода GetLogFile
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Test API",
            Version = "v1",
            Description = "API to get client IP address and log file content",
            Contact = new OpenApiContact
            {
                Name ="Iana V",
                Email = "freyayana@gmail.com"
            }
        });
    });

    builder.Services.Configure<LoggingInfoOptions>(configuration.GetSection("LoggingInfo"));
    builder.Services.Configure<BasicAuthenticationOptions>(configuration.GetSection("BasicAuthentication"));

    builder.Services.AddScoped<ILogFileProvider, LatestLogFileProider>();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseForwardedHeaders();
    // app.UseHttpsRedirection(); for ssl
    // app.UseRouting(); for mw
    app.UseAuthorization();
    app.UseAuthentication();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(opt => {
            opt.DisplayRequestDuration();
        });
    }

    app.MapControllers();

    app.Run();  
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    await Log.CloseAndFlushAsync(); // if we would using in the feature sec etc..
}


