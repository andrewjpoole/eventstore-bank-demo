#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ledger/LedgerApp/LedgerApp.csproj", "Ledger/LedgerApp/"]
RUN dotnet restore "Ledger/LedgerApp/LedgerApp.csproj"
COPY . .
WORKDIR "/src/Ledger/LedgerApp"
RUN dotnet build "LedgerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LedgerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LedgerApp.dll"]