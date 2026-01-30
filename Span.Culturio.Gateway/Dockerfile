# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Span.Culturio.Gateway/Span.Culturio.Gateway.csproj", "Span.Culturio.Gateway/"]
RUN dotnet restore "Span.Culturio.Gateway/Span.Culturio.Gateway.csproj"

COPY . .
WORKDIR "/src/Span.Culturio.Gateway"
RUN dotnet build "Span.Culturio.Gateway.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Span.Culturio.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Span.Culturio.Gateway.dll"]
