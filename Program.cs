using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using apiIEI.Extractors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register extractors and ConexionBD as services
builder.Services.AddSingleton<MURextractor>();
builder.Services.AddSingleton<CVextractor>();
builder.Services.AddSingleton<CATextractor>();
builder.Services.AddSingleton<ConexionBD>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//// Configura el puerto en el que la aplicación escuchará las solicitudes
//var port = 7000; // Reemplaza con el puerto que desees
//builder.Configuration[WebHostDefaults.ServerUrlsKey] = $"http://localhost:{port}";

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


