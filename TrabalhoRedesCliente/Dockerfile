# Etapa 1: Usar a imagem oficial do .NET SDK para compilar o projeto
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore TrabalhoRedesCliente.sln
RUN dotnet publish -c Release -o /out TrabalhoRedesCliente.sln

# Etapa 2: Usar a imagem oficial do .NET Runtime para rodar o projeto
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "TrabalhoRedes.dll"]
