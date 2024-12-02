FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . /app
WORKDIR /app
RUN dotnet tool restore
RUN dotnet build

WORKDIR /app/DepuChef.Api
RUN dotnet publish -c Development -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim as run
COPY --from=build /app/out /app/
WORKDIR /app

EXPOSE 8080
EXPOSE 8081
EXPOSE 5000
EXPOSE 5001
EXPOSE 6000
EXPOSE 6001

ENTRYPOINT [ "dotnet", "DepuChef.Api.dll" ]