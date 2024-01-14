FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5222

ENV ASPNETCORE_URLS=http://+:5222

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["SistemaVenta.AplicacionWeb/SistemaVenta.AplicacionWeb.csproj", "SistemaVenta.AplicacionWeb/"]
RUN dotnet restore "SistemaVenta.AplicacionWeb/SistemaVenta.AplicacionWeb.csproj"
COPY . .
WORKDIR "/src/SistemaVenta.AplicacionWeb"
RUN dotnet build "SistemaVenta.AplicacionWeb.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "SistemaVenta.AplicacionWeb.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaVenta.AplicacionWeb.dll"]
