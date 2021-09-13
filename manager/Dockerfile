# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet build -o out

FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY --from=build-env /app/out/ .

RUN dotnet dev-certs https
RUN dotnet dev-certs https --trust
EXPOSE 5001 5000
ENTRYPOINT ["dotnet", "manager.dll"]