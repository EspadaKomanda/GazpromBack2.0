FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5072

ENV ASPNETCORE_URLS=http://+:5072

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["ImageAgregationService/ImageAgregationService/ImageAgregationService.csproj", "ImageAgregationService/ImageAgregationService/"]
RUN dotnet restore "ImageAgregationService/ImageAgregationService/ImageAgregationService.csproj"
COPY . .
WORKDIR "/src/ImageAgregationService/ImageAgregationService"
RUN dotnet build "ImageAgregationService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ImageAgregationService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageAgregationService.dll"]
