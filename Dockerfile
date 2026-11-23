
# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project file
COPY TouristSafetySystem.csproj ./

# Restore dependencies
RUN dotnet restore TouristSafetySystem.csproj

# Copy remaining source files
COPY . .

# Publish application
RUN dotnet publish TouristSafetySystem.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

# Render's default web service port
EXPOSE 10000

# Start application and support Render's PORT variable
CMD ["sh", "-c", "mkdir -p /var/data && ln -sf /var/data/TouristSafety.db /app/TouristSafety.db && exec dotnet TouristSafetySystem.dll --urls http://0.0.0.0:${PORT:-10000}"]
