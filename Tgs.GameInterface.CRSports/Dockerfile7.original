#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

#..
RUN sed -i 's/\[openssl_init\]/# \[openssl_init\]/g' /etc/ssl/openssl.cnf
RUN sed -i '$a\[openssl_init]' /etc/ssl/openssl.cnf
RUN sed -i '$a\providers = provider_sect' /etc/ssl/openssl.cnf
RUN sed -i '$a\ssl_conf = ssl_sect' /etc/ssl/openssl.cnf
  
RUN sed -i '$a\[provider_sect]' /etc/ssl/openssl.cnf
RUN sed -i '$a\default = default_sect' /etc/ssl/openssl.cnf
RUN sed -i '$a\legacy = legacy_sect' /etc/ssl/openssl.cnf
  
RUN sed -i '$a\[default_sect]' /etc/ssl/openssl.cnf
RUN sed -i '$a\activate = 1' /etc/ssl/openssl.cnf
 
RUN sed -i '$a\[legacy_sect]' /etc/ssl/openssl.cnf
RUN sed -i '$a\activate = 1' /etc/ssl/openssl.cnf
 
RUN sed -i '$a\[ssl_sect]' /etc/ssl/openssl.cnf
RUN sed -i '$a\system_default = system_default_sect' /etc/ssl/openssl.cnf
 
RUN sed -i '$a\[system_default_sect]' /etc/ssl/openssl.cnf
RUN sed -i '$a\CipherString = DEFAULT:@SECLEVEL=0' /etc/ssl/openssl.cnf
#..

USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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

COPY --from=build /etc/ssl/openssl.cnf /etc/ssl/openssl.cnf
COPY --from=build /usr/lib/ssl/openssl.cnf /usr/lib/ssl/openssl.cnf

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameInterface.CRSports.dll"]