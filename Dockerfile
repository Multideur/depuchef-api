FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY . /app
WORKDIR /app
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool restore
RUN dotnet build

WORKDIR /app/DepuChef.Api
RUN dotnet publish -c Development -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim as run
COPY --from=build /app/out /app/
WORKDIR /app

EXPOSE 5000
EXPOSE 5001
EXPOSE 6000
EXPOSE 6001

ENTRYPOINT [ "dotnet", "DepuChef.Api.dll" ]