FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Astor.Reports.EventsPublisher/Astor.Reports.EventsPublisher.csproj", "Astor.Reports.EventsPublisher/"]
COPY ["Astor.Reports.Protocol/Astor.Reports.Protocol.csproj", "Astor.Reports.Protocol/"]
RUN dotnet restore "Astor.Reports.EventsPublisher/Astor.Reports.EventsPublisher.csproj"
COPY . .
WORKDIR "/src/Astor.Reports.EventsPublisher"
RUN dotnet build "Astor.Reports.EventsPublisher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Astor.Reports.EventsPublisher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet Astor.Reports.EventsPublisher.dll \ 
    --ConnectionStrings:Rabbit=$RABBIT_CONNECTIONSTRING  \
    --ConnectionStrings:Webapi=$WABAPI_URL \
    --ExchangePrefix=$EXCHANGEPREFIX