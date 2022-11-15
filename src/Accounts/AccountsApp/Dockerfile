#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["accounts-api/accounts-api.csproj", "accounts-api/"]
RUN dotnet restore "accounts-api/accounts-api.csproj"
COPY . .
WORKDIR "/src/accounts-api"
RUN dotnet build "accounts-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "accounts-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "accounts-api.dll"]