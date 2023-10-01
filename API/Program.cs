using API.Extensions;
using API.Helpers;
using API.Middleware;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(x => x.UseSqlite(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplicationServices();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//zmiena app jest instacja WebApplication, której opcje zostały skonfigurowane w linijkach wyżej 
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseSwaggerDocumentation();

// Configure the HTTP request pipeline only for Development environment
if (app.Environment.IsDevelopment())
{
    //Moge uzywac Swaggera oraz jego UI
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

//moge uzywac przekierowania na https z http
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try{
        var context = services.GetRequiredService<StoreContext>();
        context.Database.Migrate();
        await StoreContextSeed.SeedAsync(context, loggerFactory);
    }
    catch (Exception ex){
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occured during migration!");
    }
}

app.MapControllers();

app.Run();
