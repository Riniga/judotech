## Skapad med 
func init . --worker-runtime dotnetIsolated --target-framework net8.0
func new --name UserApi --template "HTTP trigger"

## Förutsättningar
1. [Konfigurera utvecklingsmiljö](../DevelopmentEnvironment.md)
2. [Konfigurera azure](../AzureDeployment.md)2

## Skapa local.settings.json
- Kopiera local.settings_sample.json till local.settings.json och uppdatera fälten

## Utvecklingsmiljöer (för test och utveckling)
- starta med: func start --csharp
- Debug genom att "Attach to process" -> välj func bland processer

## Anrop mot API
Testa API med hjälp av applikationen [Postman](https://www.postman.com/)