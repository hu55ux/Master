FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proyekt fayllarını kopyalayırıq
COPY ["Master.API/Master.API.csproj", "Master.API/"]
COPY ["Master.Application/Master.Application.csproj", "Master.Application/"]
COPY ["Master.Domain/Master.Domain.csproj", "Master.Domain/"]
COPY ["Master.Infrastructure/Master.Infrastructure.csproj", "Master.Infrastructure/"]

# Restore prosesi
RUN dotnet publish "Master.API.csproj" -c Release -o /app/publish \
    --no-restore \
    -r linux-x64 \
    --self-contained false \
    /p:UseAppHost=false

# Qalan bütün kodları kopyalayırıq
COPY . .
WORKDIR "/src/Master.API"

# Build və Publish
RUN dotnet publish "Master.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Master.API.dll"]
