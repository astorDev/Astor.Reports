FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["PickPoint.Reports.WebApi/PickPoint.Reports.WebApi.csproj", "PickPoint.Reports.WebApi/"]
COPY ["PickPoint.Reports.Protocol/PickPoint.Reports.Protocol.csproj", "PickPoint.Reports.Protocol/"]
COPY ["PickPoint.Reports.Csv/PickPoint.Reports.Csv.csproj", "PickPoint.Reports.Csv/"]
COPY ["PickPoint.Reports.Data/PickPoint.Reports.Data.csproj", "PickPoint.Reports.Data/"]
RUN dotnet restore -s "https://api.nuget.org/v3/index.json" -s "http://nuget.pickpoint.lcl/nuget" "PickPoint.Reports.WebApi/PickPoint.Reports.WebApi.csproj"
COPY . .
WORKDIR "/src/PickPoint.Reports.WebApi"
RUN dotnet build "PickPoint.Reports.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PickPoint.Reports.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
RUN ln -sf /usr/share/zoneinfo/Europe/Moscow /etc/localtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet PickPoint.Reports.WebApi.dll --environment=$dotnet