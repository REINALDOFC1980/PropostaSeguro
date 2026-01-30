using ContratacaoService.Domain.Entities;
using ContratacaoService.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ContratacaoService.Worker
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Aguarda RabbitMQ ficar disponível
            await WaitForRabbitMqAsync(stoppingToken);

            var queueName = _configuration["RabbitMQ:QueueName"];

            _channel!.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"Mensagem recebida: {message}");

                    // 1️⃣ Cria o escopo para pegar o DbContext
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ContratacaoDbContext>();

                    // 2️⃣ Converte a mensagem para o modelo
                    // Supondo que a mensagem seja JSON
                    var contratacao = System.Text.Json.JsonSerializer.Deserialize<ContratacaoModel>(message);

                    if (contratacao != null)
                    {
                        // 3️⃣ Define valores adicionais, se necessário
                        contratacao.Id = Guid.NewGuid();
                        contratacao.CriadoEm = DateTime.UtcNow;

                        // 4️⃣ Adiciona ao DbSet e salva no banco
                        dbContext.Contratacoes.Add(contratacao);
                        await dbContext.SaveChangesAsync();
                    }

                    // 5️⃣ Confirma entrega da mensagem
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao processar mensagem: " + ex.Message);
                    _channel.BasicNack(ea.DeliveryTag, false, true); // reenvia para a fila
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        // ================================
        // Aguarda RabbitMQ ficar pronto
        // ================================
        private async Task WaitForRabbitMqAsync(CancellationToken stoppingToken)
        {
            var host = _configuration["RabbitMQ:HostName"];
            var user = _configuration["RabbitMQ:UserName"];
            var pass = _configuration["RabbitMQ:Password"];
            var port = int.Parse(_configuration["RabbitMQ:Port"]);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = host,
                        UserName = user,
                        Password = pass,
                        Port = port,
                        DispatchConsumersAsync = true // ⚡ modo async
                    };

                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    Console.WriteLine("RabbitMQ conectado com sucesso");
                    break;
                }
                catch
                {
                    Console.WriteLine("RabbitMQ ainda não está pronto... tentando novamente em 5s");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}