FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

COPY api-build .

ENV ASPNETCORE_URLS=http://*:5001
CMD dotnet Landing.API.dll