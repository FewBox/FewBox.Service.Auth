FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY . .
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "FewBox.Service.Auth.dll"]