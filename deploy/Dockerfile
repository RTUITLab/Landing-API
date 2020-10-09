FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

COPY api-build .

ENV ASPNETCORE_URLS=http://*:5000
CMD dotnet Landing.API.dll