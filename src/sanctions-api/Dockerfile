#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["sanctions-api/sanctions-api.csproj", "sanctions-api/"]
RUN dotnet restore "sanctions-api/sanctions-api.csproj"
COPY . .
WORKDIR "/src/sanctions-api"
RUN dotnet build "sanctions-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "sanctions-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sanctions-api.dll"]