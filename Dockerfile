FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
ADD . .
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "FewBox.Service.Auth.dll"]