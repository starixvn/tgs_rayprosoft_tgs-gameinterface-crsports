#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameInterface.CRSports/Tgs.GameInterface.CRSports.csproj", "Tgs.GameInterface.CRSports/"]
COPY ["Tgs.GameInterface.CRSports.Core/Tgs.GameInterface.CRSports.Core.csproj", "Tgs.GameInterface.CRSports.Core/"]
COPY ["Tgs.GameInterface.CRSports.Protos/Tgs.GameInterface.CRSports.Protos.csproj", "Tgs.GameInterface.CRSports.Protos/"]
RUN dotnet restore "./Tgs.GameInterface.CRSports/Tgs.GameInterface.CRSports.csproj"
COPY . .
WORKDIR "/src/Tgs.GameInterface.CRSports"
RUN dotnet build "./Tgs.GameInterface.CRSports.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameInterface.CRSports.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameInterface.CRSports.dll"]