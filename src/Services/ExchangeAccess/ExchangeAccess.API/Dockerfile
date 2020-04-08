FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 61059
EXPOSE 44359

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Services/ExchangeAccess/ExchangeAccess.API/ExchangeAccess.API.csproj src/Services/ExchangeAccess/ExchangeAccess.API/
RUN dotnet restore src/Services/ExchangeAccess/ExchangeAccess.API/ExchangeAccess.API.csproj
COPY . .
WORKDIR /src/src/Services/ExchangeAccess/ExchangeAccess.API
RUN dotnet build ExchangeAccess.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ExchangeAccess.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ExchangeAccess.API.dll"]