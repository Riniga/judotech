# Uppsättning av Azure Function i Azure
Använda PowerShell i Admin

## Skapa variabler
$RG="Judoka"
$LOC="swedencentral"
$APP="judotechapi"              
$STO="judoka$(Get-Random -Max 999999)"
$COSMOS="judokacosmosdb"

## Prep
az login
az account set --subscription "<SUBSCRIPTION-NAMN-ELLER-ID>"

## Radera resursgrupp med ALLT
Kolla om lås finns: az lock list --resource-group $RG --output table
Radera : az group delete --name $RG --yes
Frigör namn: az keyvault purge --name $RG --location $LOC

## Skapa resursgrupp och resurser
az group create -n $RG -l $LOC
az storage account create -g $RG -n $STO -l $LOC --sku Standard_LRS --kind StorageV2
az cosmosdb create --name $COSMOS --resource-group $RG
az functionapp create -g $RG -n $APP --consumption-plan-location $LOC --storage-account $STO --functions-version 4 --assign-identity


## Konfigurera Function App
az functionapp cors add -g $RG -n $APP --allowed-origins https://ms.portal.azure.com https://portal.azure.com
az functionapp cors add -g $RG -n $APP --allowed-origins http://localhost:5173







## Ge Function App åtkomst till Storage
$MI = az functionapp identity show -g $RG -n $APP --query principalId -o tsv
$STO_ID = az storage account show -g $RG -n $STO --query id -o tsv
az role assignment create --assignee $MI --role "Storage Blob Data Contributor" --scope $STO_ID

## Sätt app-inställningar
$COSCON=az cosmosdb keys list -g Judoka -n judokacosmosdb --type connection-strings --query "connectionStrings[0].connectionString" -o tsv
az webapp config appsettings set -g $RG -n $APP --settings FUNCTIONS_EXTENSION_VERSION=~4 FUNCTIONS_WORKER_RUNTIME=dotnet-isolated   AzureWebJobsStorage__accountName=$STO AzureWebJobsStorage__credential=managedidentity CosmosConn=$COSCON 

Add:
    EndpointUrl="https://judokacosmosdb.documents.azure.com:443" PrimaryKey="<KEY>" DatabaseId="JudoDatabase" ContainerId="Users" LoginsContainerId="Logins" CompetitionsContainerId="Competitions"
Or: 
    az webapp config appsettings list -g Judoka -n judotechapi -o table

func azure functionapp publish judotechapi --publish-local-settings -i

## Deploy
dotnet publish -c Release -o .\publish 
tar -a -c -f artifact.zip -C publish .
az functionapp deployment source config-zip -g Judoka -n judotechapi --src .\artifact.zip

## Verifiera
az webapp config appsettings list -g $RG -n $APP --query "[?name=='FUNCTIONS_EXTENSION_VERSION' || name=='FUNCTIONS_WORKER_RUNTIME' || name=='AzureWebJobsStorage' || name=='CosmosConn'] [].name" -o tsv
az functionapp show -g $RG -n $APP --query defaultHostName -o tsv
az functionapp function list -g $RG -n $APP -o table
az functionapp function keys list -g $RG -n $APP --function-name HashPassword --query default -o tsv
curl "https://judotechapi.azurewebsites.net/api/HashPassword?password=superhemlig&code=KEY"


### (4) Loggning m.m.

- Konfigurera Log: az webapp log config -g Judoka -n judotechapi --application-logging filesystem true --level information
- Kör logstream:   az webapp log tail -g Judoka -n judotechapi
- Starta om : az functionapp restart -g Judoka -n judotechapi
- Hämta hostname: az functionapp show -g Judoka -n judotechapi --query defaultHostName -o tsv
- Lista funktioner: az functionapp function list -g Judoka -n judotechapi -o table
- Hämta nyckel (för ReadUser): az functionapp function keys list -g Judoka -n judotechapi --function-name ReadUser --query "default" -o tsv