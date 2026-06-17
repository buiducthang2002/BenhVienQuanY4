# ===== Stage 1: Build =====
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj và restore trước để tận dụng cache
COPY APP.csproj ./
RUN dotnet restore APP.csproj

# Copy toàn bộ source và publish
COPY . .
RUN dotnet publish APP.csproj -c Release -o /app/publish /p:UseAppHost=false

# ===== Stage 2: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Lắng nghe trên cổng 8080 (HTTP) trong container
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "APP.dll"]
