FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY BookApp.Solution/BookApp.Api/BookApp.Api.csproj BookApp.Api/
COPY BookApp.Solution/BookApp.Application/BookApp.Application.csproj BookApp.Application/
COPY BookApp.Solution/BookApp.Domain/BookApp.Domain.csproj BookApp.Domain/
COPY BookApp.Solution/BookApp.Infrastructure/BookApp.Infrastructure.csproj BookApp.Infrastructure/
RUN dotnet restore BookApp.Api/BookApp.Api.csproj

COPY BookApp.Solution/ .
WORKDIR /src/BookApp.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookApp.Api.dll"]
