// Infrastructure/RabbitMQ/RabbitMQService.cs
using Microsoft.Extensions.Configuration;
using PropostaService.Application.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMQService : IRabbitMQService
{
    private readonly IConfiguration _config;

    public RabbitMQService(IConfiguration config)
    {
        _config = config;
    }

    public void EnviarProposta(PropostaEnviadaEvent proposta)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMQ:HostName"] ?? "localhost",
            UserName = _config["RabbitMQ:UserName"] ?? "guest",
            Password = _config["RabbitMQ:Password"] ?? "guest",
            Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672")
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var queueName = _config["RabbitMQ:QueueName"] ?? "propostas_queue";

        channel.QueueDeclare(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var json = JsonSerializer.Serialize(proposta);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: "",
                             routingKey: queueName,
                             basicProperties: null,
                             body: body);
    }
}
