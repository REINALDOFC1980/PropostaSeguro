# ===========================
# Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia apenas os csproj (cache)
COPY ./ContratacaoService.Api/*.csproj ./ContratacaoService.Api/
COPY ./ContratacaoService.Application/*.csproj ./ContratacaoService.Application/
COPY ./ContratacaoService.Domain/*.csproj ./ContratacaoService.Domain/
COPY ./ContratacaoService.Infrastructure/*.csproj ./ContratacaoService.Infrastructure/

# Restore
RUN dotnet restore ./ContratacaoService.Api/ContratacaoService.Api.csproj

# Copia todo o código
COPY . .

# Publish
RUN dotnet publish ./ContratacaoService.Api/ContratacaoService.Api.csproj -c Release -o /app/publish

# ===========================
# Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "ContratacaoService.Api.dll"]
