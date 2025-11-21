
Skapat projektet manuellt med projektfil, Program, dockerfile och .dockerignore


Kör med: dotnet run --project UsersApi.csproj
Öppna Swagger: http://localhost:5183/swagger (port varierar)



## Bygg container

docker build -t users-api:local ./UsersApi
