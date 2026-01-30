using ContratacaoService.Application.Interfaces;
using ContratacaoService.Application.Services;
using ContratacaoService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Permite que o container aceite requisições externas
builder.WebHost.UseUrls("http://0.0.0.0:80");


// Controllers
builder.Services.AddControllers();

builder.Services.AddScoped<IContratacaoService, ContratacaoServiceApp>();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (banco do microserviço Contratação)
builder.Services.AddDbContext<ContratacaoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services);

// Swagger sempre habilitado no container
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContratacaoService API V1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
