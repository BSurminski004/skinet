using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(x => x.UseSqlite(
    builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Serwis ktory dodaje Swaggera,pozwala on na wyswietlanie listy dostepnych ednpointow oraz na wysylanie zapytan do nich
builder.Services.AddSwaggerGen();

//zmiena app jest instacja WebApplication, której opcje zostały skonfigurowane w linijkach wyżej 
var app = builder.Build();

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

app.MapControllers();

app.Run();
