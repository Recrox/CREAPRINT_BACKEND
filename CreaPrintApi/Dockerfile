# Multi-stage Dockerfile for .NET9
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY CreaPrintApi/CreaPrintApi.csproj CreaPrintApi/
COPY CreaPrintCore/CreaPrintCore.csproj CreaPrintCore/
COPY CreaPrintDatabase/CreaPrintDatabase.csproj CreaPrintDatabase/
COPY CreaPrintConfiguration/CreaPrintConfiguration.csproj CreaPrintConfiguration/

RUN dotnet restore CreaPrintApi/CreaPrintApi.csproj

# Copy everything and publish
COPY . ./
WORKDIR /src/CreaPrintApi
RUN dotnet publish CreaPrintApi.csproj -c Release -o /app/publish /p:TrimUnusedDependencies=true

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE80

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "CreaPrintApi.dll"]
