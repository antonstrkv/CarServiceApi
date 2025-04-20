using DataAccess;
using Serilog;
using Serilog.Events;
using WebApi.Filters;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) 
    .MinimumLevel.Information()
    .WriteTo.Console() 
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(logEvent =>
            logEvent.Properties.ContainsKey("SourceContext") &&
            logEvent.Properties["SourceContext"].ToString().Contains("CarRequestLoggingMiddleware"))
        .WriteTo.File("Logs/car-requests-.txt", rollingInterval: RollingInterval.Day))
    .CreateLogger();

builder.Services.AddScoped<ExecutionTimingFilter>();


builder.Host.UseSerilog(); 


// Add services to the container.
builder.Services.AddDataAccess();
builder.Services.AddBussinessLogic();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost";
    options.InstanceName = "local";
});
var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<CarRequestLoggingMiddleware>();
app.Run();
