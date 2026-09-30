FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos csproj para restaurar dependencias
COPY ["BUFFET VEGETARIANO TIEN LAN ZU SHI/BUFFET VEGETARIANO TIEN LAN ZU SHI.csproj", "BUFFET VEGETARIANO TIEN LAN ZU SHI/"]
COPY ["TIEN LAN ZU SHI.Business/TIEN LAN ZU SHI.Business.csproj", "TIEN LAN ZU SHI.Business/"]
COPY ["TIEN LAN ZU SHI.Core/TIEN LAN ZU SHI.Core.csproj", "TIEN LAN ZU SHI.Core/"]
COPY ["TIEN LAN ZU SHI.DataAccess/TIEN LAN ZU SHI.DataAccess.csproj", "TIEN LAN ZU SHI.DataAccess/"]

# Restaurar dependencias
RUN dotnet restore "BUFFET VEGETARIANO TIEN LAN ZU SHI/BUFFET VEGETARIANO TIEN LAN ZU SHI.csproj"

# Copiar el resto del codigo
COPY . .
WORKDIR "/src/BUFFET VEGETARIANO TIEN LAN ZU SHI"
RUN dotnet publish "BUFFET VEGETARIANO TIEN LAN ZU SHI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BUFFET VEGETARIANO TIEN LAN ZU SHI.dll"]