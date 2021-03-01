FROM mcr.microsoft.com/dotnet/core/aspnet:5.0

WORKDIR /app

COPY admin-build .

ENV ASPNETCORE_URLS=http://*:5003
CMD dotnet AdminPanel.dll