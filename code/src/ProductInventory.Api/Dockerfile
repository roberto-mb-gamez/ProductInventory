FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /source
COPY Directory.Build.props Directory.Packages.props ./
COPY src/ProductInventory.Api/ProductInventory.Api.csproj src/ProductInventory.Api/
COPY src/ProductInventory.Application/ProductInventory.Application.csproj src/ProductInventory.Application/
COPY src/ProductInventory.Domain/ProductInventory.Domain.csproj src/ProductInventory.Domain/
COPY src/ProductInventory.Infrastructure/ProductInventory.Infrastructure.csproj src/ProductInventory.Infrastructure/
RUN dotnet restore src/ProductInventory.Api/ProductInventory.Api.csproj
COPY src/ src/
RUN dotnet publish src/ProductInventory.Api/ProductInventory.Api.csproj -c $BUILD_CONFIGURATION --no-restore -o /app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "ProductInventory.Api.dll"]
