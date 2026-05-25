# ===== Stage 1: Dependencies Restore =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

# Copy only project files for dependency restoration
COPY *.sln ./
COPY Tekno.Api/*.csproj Tekno.Api/
COPY Tekno.Domain/*.csproj Tekno.Domain/
COPY Tekno.Infrastructure/*.csproj Tekno.Infrastructure/
COPY Tekno.Application/*.csproj Tekno.Application/
COPY Tests/Domain.Tests/*.csproj Tests/Domain.Tests/
COPY Tests/Application.Tests/*.csproj Tests/Application.Tests/

RUN dotnet restore

# ===== Stage 2: Build =====
FROM restore AS build
WORKDIR /src

# Copy entire source (excluding files in .dockerignore)
COPY . .

WORKDIR /src/Tekno.Api
RUN dotnet publish -c Release -o /app/publish \
    && find /app/publish -name "*.pdb" -delete \
    && find /app/publish -name "*.dbg" -delete

# ===== Stage 3: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-ltsc2022 AS final

WORKDIR /app

# Optimized copy - only published output
COPY --from=build /app/publish .

# Environment configuration
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_ENABLE_DIAGNOSTIC_SOURCES=false

EXPOSE 8080

# Healthcheck
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD powershell -Command try { (Invoke-WebRequest http://localhost:8080/health -UseBasicParsing).StatusCode -eq 200 } catch { exit 1 }

ENTRYPOINT ["dotnet", "Tekno.Api.dll"]
