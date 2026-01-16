# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy backend source (mantém estrutura src/backend/)
COPY src/backend/ ./src/backend/

# Copy Onix.Framework source
COPY Onix.Framework/ ./Onix.Framework/

# Restore and build
WORKDIR /app/src/backend/WebApi/Onix.Writebook.WebApi
RUN dotnet restore "Onix.Writebook.WebApi.csproj"
RUN dotnet build "Onix.Writebook.WebApi.csproj" -c Release -o /app/build
RUN dotnet publish "Onix.Writebook.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Onix.Writebook.WebApi.dll"]
