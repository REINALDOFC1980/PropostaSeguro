using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PropostaService.Api.Middlewares;
using PropostaService.Api.Validators;
using PropostaService.Application.Interfaces;
using PropostaService.Application.Services;
using PropostaService.Infrastructure.Database;
using PropostaService.Infrastructure.Repositories;
using PropostaService.Shared.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Configuração Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console() // log no console
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // log em arquivo diário
    .ReadFrom.Configuration(ctx.Configuration) // lê nível de log do appsettings.json
);

// Permite que o container aceite requisições externas
builder.WebHost.UseUrls("http://0.0.0.0:80");

// Serializa enums como string
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext SQL Server
builder.Services.AddDbContext<PropostaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injeção de dependência
builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<PropostaServiceApp>();

builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();



// Configuração do FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<PropostaValidator>();
    });

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services);


//Aplica migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PropostaDbContext>();
    db.Database.Migrate();
}

// Swagger sempre habilitado no container
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PropostaService API V1");
});

// Middleware de log antes do middleware de exceção
app.UseMiddleware<RequestLoggingMiddleware>();

// Middleware global de exceção
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<IdempotencyMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
