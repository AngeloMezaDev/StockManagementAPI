FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/ProductService/ProductService.API/ProductService.API.csproj", "src/Services/ProductService/ProductService.API/"]
COPY ["src/Services/ProductService/ProductService.Application/ProductService.Application.csproj", "src/Services/ProductService/ProductService.Application/"]
COPY ["src/Services/ProductService/ProductService.Domain/ProductService.Domain.csproj", "src/Services/ProductService/ProductService.Domain/"]
COPY ["src/Services/ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "src/Services/ProductService/ProductService.Infrastructure/"]
RUN dotnet restore "src/Services/ProductService/ProductService.API/ProductService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/ProductService/ProductService.API"
RUN dotnet build "ProductService.API.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/src/Services/ProductService/ProductService.API"
RUN dotnet publish "ProductService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.API.dll"]