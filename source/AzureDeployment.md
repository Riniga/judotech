# Setup Microsoft Azure Environment and deployment
Öppna PowerShell (som Administratör)


## Azure Cli
1. az --version
2. az upgrade

## Skapa variabler
$TENANT="<Tenant ID found in Azure Entra ID - Properties>"
$SUBSCR= "<SUBSCRIPTION-NAMN-ELLER-ID>"
$RG=    "Judoka"
$LOC=   "swedencentral"
$APP=   "judotechapi"              
$STO=   "judoka$(Get-Random -Max 999999)"
$COSMOS="judokacosmosdb"

## Azure Account and login
1. az login                                 # Om det behövs --tenant $TENANT
2. az account set --subscription $SUBSCR    # Om det behövs
3. az account show

## Radera resursgrupp med ALLT
1. az group list
2. az lock list --resource-group $RG --output table
3. az lock delete --ids <LOCK_ID>
4. az group delete --name $RG --yes
5. az keyvault purge --name $RG --location $L

## Skapa resursgrupp och resurser
1. az group create -n $RG -l $LOC
2. az storage account create -g $RG -n $STO -l $LOC --sku Standard_LRS --kind StorageV2
3. az cosmosdb create --name $COSMOS --resource-group $RG
4. az functionapp create -g $RG -n $APP --consumption-plan-location $LOC --storage-account $STO --functions-version 4 --assign-identity

### App Service Plan - Not needed I think.....
1. az appservice plan create -g Judoka -n JudokaServicePlan
[//]: # sku {Premium_LRS, Premium_ZRS, Standard_GRS, Standard_GZRS, Standard_LRS, Standard_RAGRS, Standard_RAGZRS, Standard_ZRS}]

### Configure Cosmos DB
- az cosmosdb keys list --name judokacosmosdb --resource-group Judoka
- az cosmosdb show --name judokacosmosdb --resource-group Judoka


## Konfigurera Function App
1. az functionapp cors add -g $RG -n $APP --allowed-origins https://ms.portal.azure.com https://portal.azure.com
2. az functionapp cors add -g $RG -n $APP --allowed-origins http://localhost:5173
3. az functionapp cors add -g $RG -n $APP --allowed-origins https://judoka388503.z1.web.core.windows.net:80

### Ge Function App åtkomst till Storage
1. $MI = az functionapp identity show -g $RG -n $APP --query principalId -o tsv
2. $STO_ID = az storage account show -g $RG -n $STO --query id -o tsv
3. az role assignment create --assignee $MI --role "Storage Blob Data Contributor" --scope $STO_ID

## Sätt app-inställningar
1. $COSCON=az cosmosdb keys list -g Judoka -n judokacosmosdb --type connection-strings --query "connectionStrings[0].connectionString" -o tsv
2. az webapp config appsettings set -g $RG -n $APP --settings FUNCTIONS_EXTENSION_VERSION=~4 FUNCTIONS_WORKER_RUNTIME=dotnet-isolated   AzureWebJobsStorage__accountName=$STO AzureWebJobsStorage__credential=managedidentity CosmosConn=$COSCON 

Add:
    EndpointUrl="https://judokacosmosdb.documents.azure.com:443" PrimaryKey="<KEY>" DatabaseId="JudoDatabase" ContainerId="Users" LoginsContainerId="Logins" CompetitionsContainerId="Competitions"
Or: 
    func azure functionapp publish judotechapi --publish-local-settings -i

az webapp config appsettings list -g Judoka -n judotechapi -o table

## Deploy
1. dotnet publish -c Release -o .\publish 
2. tar -a -c -f artifact.zip -C publish .
3. az functionapp deployment source config-zip -g Judoka -n judotechapi --src .\artifact.zip

## Verifiera
1. az webapp config appsettings list -g $RG -n $APP --query "[?name=='FUNCTIONS_EXTENSION_VERSION' || name=='FUNCTIONS_WORKER_RUNTIME' || name=='AzureWebJobsStorage' || name=='PrimaryKey'] [].name" -o tsv
2. az functionapp show -g $RG -n $APP --query defaultHostName -o tsv
3. az functionapp function list -g $RG -n $APP -o table
4. az functionapp function keys list -g $RG -n $APP --function-name HashPassword --query default -o tsv
5. curl "https://judotechapi.azurewebsites.net/api/HashPassword?password=superhemlig&code=KEY"

### (4) Loggning m.m.

- Konfigurera Log: az webapp log config -g Judoka -n judotechapi --application-logging filesystem true --level information
- Kör logstream:   az webapp log tail -g Judoka -n judotechapi
- Starta om : az functionapp restart -g Judoka -n judotechapi
- Hämta hostname: az functionapp show -g Judoka -n judotechapi --query defaultHostName -o tsv
- Lista funktioner: az functionapp function list -g Judoka -n judotechapi -o table
- Hämta nyckel (för ReadUser): az functionapp function keys list -g Judoka -n judotechapi --function-name ReadUser --query "default" -o tsv