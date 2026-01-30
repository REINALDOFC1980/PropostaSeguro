using ContratacaoService.Application.Interfaces;
using ContratacaoService.Application.Services;
using ContratacaoService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);




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

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
