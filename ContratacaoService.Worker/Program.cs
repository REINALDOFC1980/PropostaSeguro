using ContratacaoService.Infrastructure.Database;
using ContratacaoService.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// ============================
// DbContext (RESILIENTE PARA DOCKER)
// ============================
builder.Services.AddDbContext<ContratacaoDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql =>
        {
            // Retry automático quando o SQL ainda estiver iniciando
            sql.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);

            // Timeout alto para CREATE DATABASE / ALTER DATABASE
            sql.CommandTimeout(180);
        }));

// ============================
// RabbitMQ Consumer
// ============================
builder.Services.AddHostedService<RabbitMqConsumer>();

var host = builder.Build();



// ============================
// RUN WORKER
// ============================
host.Run();
