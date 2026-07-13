# Usa la imagen oficial del SDK de .NET 10 para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia el archivo csproj y restaura las dependencias
COPY ["PescaderiaApi.csproj", "./"]
RUN dotnet restore "PescaderiaApi.csproj"

# Copia el resto del código y compila la aplicación
COPY . .
RUN dotnet publish "PescaderiaApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Usa la imagen ligera de ASP.NET 10 para correr la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto por defecto de ASP.NET Core 8+ (8080)
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "PescaderiaApi.dll"]
