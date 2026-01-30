# PropostaSeguro - Microserviços de Contratação e Propostas

Projeto de exemplo de microserviços em .NET 6/7, utilizando:

- ASP.NET Core Web API
- Worker Service
- Entity Framework Core (SQL Server)
- RabbitMQ para mensageria
- Docker e Docker Compose
- FluentValidation, Serilog, Swagger

O sistema consiste em três serviços principais:

1. **PropostaService.API** – gerencia as propostas.
2. **ContratacaoService.API** – gerencia as contratações.
3. **ContratacaoService.Worker** – consome mensagens do RabbitMQ e processa contratações.

---

## Estrutura do Projeto

```
├── PropostaService.Api
├── PropostaService.Application
├── PropostaService.Infrastructure
├── ContratacaoService.Api
├── ContratacaoService.Application
├── ContratacaoService.Infrastructure
├── ContratacaoService.Worker
└── docker-compose.yml
```

- Cada microserviço possui seu próprio **DbContext** e banco de dados.
- O Worker Service escuta uma fila RabbitMQ para processar contratações.
- RabbitMQ e SQL Server rodam via Docker (ou podem ser configurados localmente).

---

## Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [.NET SDK 6 ou 7](https://dotnet.microsoft.com/download)
- SQL Server (local ou container)
- Visual Studio 2022 ou VS Code

---

## Rodando via Docker (recomendado)

1. Limpar containers e volumes antigos:

```bash
docker compose down -v
```

2. Construir imagens:

```bash
docker compose build --no-cache
```

3. Subir todos os serviços:

```bash
docker compose up
```

O `docker-compose.yml` já define:

- `sqlserver` → SQL Server 2022
- `rabbitmq` → RabbitMQ 3 com Management UI (`http://localhost:15672`)
- `propostaservice` → API de Propostas
- `contratacao-api` → API de Contratações
- `contratacao-worker` → Worker Service

---

## Rodando localmente (sem Docker)

1. Configure os **appsettings.json** de cada projeto:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR_SQL;Database=PropostasDb;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True;"
},
"RabbitMQ": {
  "Host": "localhost",
  "User": "guest",
  "Password": "guest",
  "Queue": "propostas-criadas",
  "Port": 5672
}
```

> Atenção: os nomes de fila e host devem bater com os que estão definidos nos Workers.

2. Rodar cada projeto pelo Visual Studio ou CLI:

```bash
dotnet run --project PropostaService.Api
dotnet run --project ContratacaoService.Api
dotnet run --project ContratacaoService.Worker
```

---

## RabbitMQ

- Host: `rabbitmq` (no Docker) ou `localhost` (local)
- Porta AMQP: `5672`
- Management UI: `http://localhost:15672`  
  - Usuário padrão: `guest`
  - Senha: `guest`
- Fila usada: `propostas-criadas`

---

## SQL Server

- Container: `sqlserver` (porta 1433)
- Banco de dados:
  - `PropostasDb` → para PropostaService
  - `ContratacoesDb` → para ContratacaoService
- Usuário: `sa`  
- Senha: `Proposta123!` (Docker)  

> As migrations são aplicadas automaticamente no startup dos serviços.

---

## Observações

- **Dependências**: O Worker Service depende do SQL Server e do RabbitMQ, o docker-compose já aguarda os serviços estarem disponíveis.
- **Logs**: Serilog escreve logs no console e em arquivos `Logs/log-*.txt`.
- **Swagger**:
  - PropostaService API: `http://localhost:5000/swagger`
  - ContratacaoService API: `http://localhost:5001/swagger`
- **Limpeza de volumes**: Para reiniciar o banco do zero, use `docker compose down -v`.

---

## Comandos úteis Docker

```bash
docker ps            # listar containers ativos
docker logs -f NOME  # acompanhar logs de um container
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Proposta123!   # acessar SQL Server container