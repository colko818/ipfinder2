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
ENV WORKER_NODE_SERVICE_SERVICE_HOST=172.20.0.2
ENV WORKER_NODE_SERVICE_SERVICE_PORT=2112
EXPOSE 5000 5001
ENTRYPOINT ["dotnet", "manager.dll"]