FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
ADD . .
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "FewBox.Service.Auth.dll"]