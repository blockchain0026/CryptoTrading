FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54270
EXPOSE 44398

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Services/TrendAnalysis/TrendAnalysis.API/TrendAnalysis.API.csproj src/Services/TrendAnalysis/TrendAnalysis.API/
RUN dotnet restore src/Services/TrendAnalysis/TrendAnalysis.API/TrendAnalysis.API.csproj
COPY . .
WORKDIR /src/src/Services/TrendAnalysis/TrendAnalysis.API
RUN dotnet build TrendAnalysis.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TrendAnalysis.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TrendAnalysis.API.dll"]
