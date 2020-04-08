FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54790
EXPOSE 44320

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Services/Investing/Investing.API/Investing.API.csproj src/Services/Investing/Investing.API/
RUN dotnet restore src/Services/Investing/Investing.API/Investing.API.csproj
COPY . .
WORKDIR /src/src/Services/Investing/Investing.API
RUN dotnet build Investing.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Investing.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Investing.API.dll"]