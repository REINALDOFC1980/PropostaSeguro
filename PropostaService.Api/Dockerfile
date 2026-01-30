# ===========================
# Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia apenas os csproj primeiro (para cache)
COPY ./PropostaService.Api/*.csproj ./PropostaService.Api/
COPY ./PropostaService.Application/*.csproj ./PropostaService.Application/
COPY ./PropostaService.Domain/*.csproj ./PropostaService.Domain/
COPY ./PropostaService.Infrastructure/*.csproj ./PropostaService.Infrastructure/

# Restaura pacotes
RUN dotnet restore ./PropostaService.Api/PropostaService.Api.csproj

# Copia todo o código
COPY . .

# Publica
RUN dotnet publish ./PropostaService.Api/PropostaService.Api.csproj -c Release -o /app/publish

# ===========================
# Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "PropostaService.Api.dll"]
