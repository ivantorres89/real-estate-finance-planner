FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/RealEstateFinancePlanner.Domain/*.csproj src/RealEstateFinancePlanner.Domain/
COPY src/RealEstateFinancePlanner.Application/*.csproj src/RealEstateFinancePlanner.Application/
COPY src/RealEstateFinancePlanner.Infrastructure/*.csproj src/RealEstateFinancePlanner.Infrastructure/
COPY src/RealEstateFinancePlanner.Api/*.csproj src/RealEstateFinancePlanner.Api/
RUN dotnet restore src/RealEstateFinancePlanner.Api/RealEstateFinancePlanner.Api.csproj

COPY src/ src/
WORKDIR /src/src/RealEstateFinancePlanner.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "RealEstateFinancePlanner.Api.dll"]
