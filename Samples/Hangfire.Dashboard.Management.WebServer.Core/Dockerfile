FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["nuget.config", "Hangfire.Dashboard.Management.WebServer/"]
COPY ["Hangfire.Dashboard.Management/Hangfire.Dashboard.Management.csproj", "Hangfire.Dashboard.Management/"]
COPY ["HangfireJobTask/HangfireJobTask.csproj", "HangfireJobTask/"]
COPY ["Hangfire.Dashboard.Management.WebServer/Hangfire.Dashboard.Management.WebServer.csproj", "Hangfire.Dashboard.Management.WebServer/"]
RUN dotnet restore "Hangfire.Dashboard.Management.WebServer/Hangfire.Dashboard.Management.WebServer.csproj"
COPY . .
WORKDIR "/src/Hangfire.Dashboard.Management.WebServer"
RUN dotnet build "Hangfire.Dashboard.Management.WebServer.csproj" -c Release -o /app --configfile "nuget.config"

FROM build AS publish
RUN dotnet publish "Hangfire.Dashboard.Management.WebServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Hangfire.Dashboard.Management.WebServer.dll"]