# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

#http
EXPOSE 5077 

#https
EXPOSE 7077 

# Copy project files
COPY BlogSystem.API/BlogSystem.API.csproj BlogSystem.API/
COPY BlogSystem.Application/BlogSystem.Application.csproj BlogSystem.Application/
COPY BlogSystem.Domain/BlogSystem.Domain.csproj BlogSystem.Domain/
COPY BlogSystem.Infrastructure/BlogSystem.Infrastructure.csproj BlogSystem.Infrastructure/

# Restore dependencies
RUN dotnet restore BlogSystem.API/BlogSystem.API.csproj

# Copy all source code
COPY . .

# Build and publish the app
WORKDIR /src/BlogSystem.API
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage

COPY --from=build /app/publish .
COPY ./aspnetapp.pfx /https/aspnetapp.pfx

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Run the application
ENTRYPOINT ["dotnet", "BlogSystem.API.dll"]

# docker run -d --name blog-container --env-file .env -p 5077:5077 -p 7077:7077 blog-api