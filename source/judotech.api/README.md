## Skapad med 
func init . --worker-runtime dotnetIsolated --target-framework net8.0
func new --name UserApi --template "HTTP trigger"

## Förutsättningar
.NET core 8.0 SDK
* dotnet --list-sdks
* winget install --id Microsoft.DotNet.SDK.8 --scope machine
Functions tool:
func installeras med: npm install -g azure-functions-core-tools@3 --unsafe-perm true

## CosmosDB
- Se ..\readme.md för att sätta upp cosmos db i azure och hämta nycklar

## Skapa local.settings.json
- Kopiera local.settings_sample.json till local.settings.json och uppdatera fälten

## Utvecklingsmiljöer (för test och utveckling)
- starta med: func start --csharp
- Debug genom att "Attach to process" -> välj func bland processer

## Anrop mot API
Testa API med hjälp av applikationen https://www.postman.com/