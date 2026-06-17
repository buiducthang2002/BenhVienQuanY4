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

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /app/publish .

# Lắng nghe trên $PORT do Railway cấp (mặc định 8080 khi chạy local)
ENTRYPOINT ["sh", "-c", "dotnet APP.dll --urls=http://0.0.0.0:${PORT:-8080}"]
