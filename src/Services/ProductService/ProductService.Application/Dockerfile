FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/TransactionService/TransactionService.API/TransactionService.API.csproj", "src/Services/TransactionService/TransactionService.API/"]
COPY ["src/Services/TransactionService/TransactionService.Application/TransactionService.Application.csproj", "src/Services/TransactionService/TransactionService.Application/"]
COPY ["src/Services/TransactionService/TransactionService.Domain/TransactionService.Domain.csproj", "src/Services/TransactionService/TransactionService.Domain/"]
COPY ["src/Services/TransactionService/TransactionService.Infrastructure/TransactionService.Infrastructure.csproj", "src/Services/TransactionService/TransactionService.Infrastructure/"]
RUN dotnet restore "src/Services/TransactionService/TransactionService.API/TransactionService.API.csproj"
COPY . .
WORKDIR "/src/src/Services/TransactionService/TransactionService.API"
RUN dotnet build "TransactionService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionService.API.dll"]