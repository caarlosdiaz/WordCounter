FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WordCounterInterview/WordCounterInterview.csproj", "WordCounterInterview/"]
RUN dotnet restore "WordCounterInterview/WordCounterInterview.csproj"
COPY . .
WORKDIR "/src/WordCounterInterview"
RUN dotnet build "./WordCounterInterview.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WordCounterInterview.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WordCounterInterview.dll"]
