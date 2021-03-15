FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Astor.Reports.WebApi/Astor.Reports.WebApi.csproj", "Astor.Reports.WebApi/"]
COPY ["Astor.Reports.Protocol/Astor.Reports.Protocol.csproj", "Astor.Reports.Protocol/"]
COPY ["Astor.Reports.Domain/Astor.Reports.Domain.csproj", "Astor.Reports.Domain/"]
COPY ["Astor.Reports.Data/Astor.Reports.Data.csproj", "Astor.Reports.Data/"]
RUN dotnet restore -s "https://api.nuget.org/v3/index.json" "Astor.Reports.WebApi/Astor.Reports.WebApi.csproj"
COPY . .
WORKDIR "/src/Astor.Reports.WebApi"
RUN dotnet build "Astor.Reports.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Astor.Reports.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
RUN ln -sf /usr/share/zoneinfo/Europe/Moscow /etc/localtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet Astor.Reports.WebApi.dll --ConnectionStrings:Mongo=$MONGO_CONNECTIONSTRING