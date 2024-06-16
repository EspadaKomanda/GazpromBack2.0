FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5095

ENV ASPNETCORE_URLS=http://+:5095

USER app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["BackGazprom.csproj", "./"]
RUN dotnet restore "BackGazprom.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "BackGazprom.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "BackGazprom.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackGazprom.dll"]
