FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

COPY admin-build .

ENV ASPNETCORE_URLS=http://*:5003
CMD dotnet AdminPanel.dll