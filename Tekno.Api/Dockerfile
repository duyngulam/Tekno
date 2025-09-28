# ===== Stage 1: Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy sln + csproj để restore
COPY *.sln ./
COPY Tekno.Api/*.csproj Tekno.Api/
COPY Tekno.Domain/*.csproj Tekno.Domain/
COPY Tekno.Infrastructure/*.csproj Tekno.Infrastructure/
COPY Tekno.Application/*.csproj Tekno.Application/

RUN dotnet restore

# Copy toàn bộ source và build
COPY . .
WORKDIR /src/Tekno.Api
RUN dotnet publish -c Release -o /app/publish

# ===== Stage 2: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Chạy HTTP để tránh vấn đề certificate trong Docker
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

ENTRYPOINT ["dotnet", "Tekno.Api.dll"]
