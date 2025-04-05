using DataAccess;
using BusinessLogic;

var builder = WebApplication.CreateBuilder(args);

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
app.Run();
