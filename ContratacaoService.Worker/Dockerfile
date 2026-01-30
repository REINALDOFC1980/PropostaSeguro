# ===========================
# Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia apenas os csproj (cache)
COPY ./ContratacaoService.Worker/*.csproj ./ContratacaoService.Worker/
COPY ./ContratacaoService.Application/*.csproj ./ContratacaoService.Application/
COPY ./ContratacaoService.Domain/*.csproj ./ContratacaoService.Domain/
COPY ./ContratacaoService.Infrastructure/*.csproj ./ContratacaoService.Infrastructure/

# Restore
RUN dotnet restore ./ContratacaoService.Worker/ContratacaoService.Worker.csproj

# Copia todo o código
COPY . .

# Publish
RUN dotnet publish ./ContratacaoService.Worker/ContratacaoService.Worker.csproj -c Release -o /app/publish

# ===========================
# Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContratacaoService.Worker.dll"]
