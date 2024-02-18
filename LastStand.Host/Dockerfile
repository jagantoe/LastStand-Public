#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 11111
EXPOSE 30000

ENV ASPNETCORE_URLS=http://+:8000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LastStand.Host/LastStand.Host.csproj", "LastStand.Host/"]
COPY ["LastStand.ApiBase/LastStand.APIBase.csproj", "LastStand.ApiBase/"]
COPY ["LastStand.Dashboard/LastStand.Dashboard.csproj", "LastStand.Dashboard/"]
COPY ["LastStand.Grains/LastStand.Grains.csproj", "LastStand.Grains/"]
COPY ["LastStand.Game/LastStand.Game.csproj", "LastStand.Game/"]
RUN dotnet restore "./LastStand.Host/./LastStand.Host.csproj"
COPY . .
WORKDIR "/src/LastStand.Host"
RUN dotnet build "./LastStand.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LastStand.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LastStand.Host.dll"]