# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY Foodify_DoAn/*.csproj ./Foodify_DoAn/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /src/Foodify_DoAn
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Foodify_DoAn.dll"]