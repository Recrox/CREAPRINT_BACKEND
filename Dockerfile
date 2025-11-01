# Multi-stage Dockerfile for .NET9 - root-level for fly.io
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY CreaPrintApi/*.csproj CreaPrintApi/
COPY CreaPrintCore/*.csproj CreaPrintCore/
COPY CreaPrintDatabase/*.csproj CreaPrintDatabase/
COPY CreaPrintConfiguration/*.csproj CreaPrintConfiguration/

RUN dotnet restore CreaPrintApi/CreaPrintApi.csproj

# Copy everything and publish
COPY . ./
WORKDIR /src/CreaPrintApi
RUN dotnet publish CreaPrintApi.csproj -c Release -o /app/publish /p:TrimUnusedDependencies=true

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
# Bind to port80 inside the container; Fly sets PORT env at runtime and we configure Kestrel in app if needed
ENV ASPNETCORE_URLS=http://+:80
# Expose default port (informational)
EXPOSE 80

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "CreaPrintApi.dll"]
