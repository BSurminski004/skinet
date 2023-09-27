using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(x => x.UseSqlite(
builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddEndpointsApiExplorer();
//Serwis ktory dodaje Swaggera,pozwala on na wyswietlanie listy dostepnych ednpointow oraz na wysylanie zapytan do nich
builder.Services.AddSwaggerGen();

//zmiena app jest instacja WebApplication, której opcje zostały skonfigurowane w linijkach wyżej 
var app = builder.Build();

//Service for automate migration applying


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //Moge uzywac Swaggera oraz jego UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

//moge uzywac przekierowania na httpsz http
app.UseHttpsRedirection();
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
