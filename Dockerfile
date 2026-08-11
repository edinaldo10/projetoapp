# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo do projeto (como o contexto já é a pasta dotnet/, copiamos direto)
COPY ["cloud-application.csproj", "./"]
RUN dotnet restore "cloud-application.csproj"

# Copia todo o resto do código fonte
COPY . .
RUN dotnet publish "cloud-application.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio de Runtime (Final)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN useradd -u 1000 appuser && chown -R appuser /app
USER appuser
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "cloud-application.dll"]