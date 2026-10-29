# Usa a imagem oficial do SDK do .NET 9 para compilar o projeto
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia o arquivo de projeto e restaura as dependências
COPY ["pimFinal.csproj", "./"]
RUN dotnet restore "pimFinal.csproj"

# Copia o resto do código e compila a versão de lançamento (Release)
COPY . .
WORKDIR "/src/"
RUN dotnet build "pimFinal.csproj" -c Release -o /app/build
RUN dotnet publish "pimFinal.csproj" -c Release -o /app/publish

# Usa a imagem mais leve do ASP.NET apenas para rodar o projeto pronto
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "pimFinal.dll"]