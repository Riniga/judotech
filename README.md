# JudoTech
JudoTech is a comprehensive ecosystem designed to manage all technical functionalities for our community. It serves as a unified platform for handling memberships, rankings, competition systems, result boards, and much more. 

## Purpose and Vision
JudoTech is built to streamline and enhance the experience for everyone involved in our community. Whether you are a competitor, organizer, or supporter, JudoTech provides the tools needed for seamless interaction and efficient management. Our goal is to create a system that is precise, easy to use, and continuously improved through collaboration.

## Get Involved
Whether you are a developer, designer, or user, your input is valuable in shaping JudoTech. Together, we will build an efficient, transparent, and robust system that serves our community's needs.
Stay connected, contribute, and let’s develop JudoTech into the best system possible!

### Versioning
Every application has a version number (Major.Minor.Patch) indicating it's current status:
* Major: Breaking changes
* Minor: New features (non-breaking)
* Patch: Bug fixes, performance improvements

To indicate the stability, we add:
* alpha: Under development, unstable and incomplete
* beta: Complete, but unstable - in testenvifonment
* RC: Release Candidate, should be stable

## Documentation
/documentation/
├── features.md
├── functional-requirements.md
├── non-functional-requirements.md
├── roadmap.md (documentation/roadmap.md)
└── changelog.md


## Project structure
- Source code in folder: "source"
- Every application, minimal with basic functionality for its purpose, well documented with its source

## Tools
- Visual Studio Code
- Visual Studio Code Extencion: Azure Resource Manager (ARM) Tools
- Azure CLI: https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?source=recommendations&tabs=azure-cli
* Miniconda (https://docs.anaconda.com/miniconda/install/)

## Environment

### Docker image 
1. Install Docker: winget install --id Docker.DockerDesktop
2. Start Docker!
3. Build Container: 
   3.1 cd devcontiner
   3.2 docker build -t my-dev-container .
4. Connect to container with sourcecode: 
   4.1 cd ..\source
   4.2 docker run -it --rm -v %cd%:/workspace -w /workspace my-dev-container
5. Uppdatera linux Ubunto: 
   5.1 sudo apt-get update
   5.2 sudo apt-get dist-upgrade


### Dotnet 8.0
sudo apt-get update
sudo apt-get install -y wget curl apt-transport-https software-properties-common gnupg lsb-release unzip
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
List current dotnet versoin: dotnet --list-sdks

### Dotnet 3.1
mkdir -p ~/dotnet3
cd ~/dotnet3
wget https://builds.dotnet.microsoft.com/dotnet/Sdk/3.1.426/dotnet-sdk-3.1.426-linux-x64.tar.gz
tar -zxf dotnet-sdk-3.1.426-linux-x64.tar.gz
rm dotnet-sdk-3.1.426-linux-x64.tar.gz

Enable 8.0:
- export DOTNET_ROOT=/usr/share/dotnet
- export PATH=$DOTNET_ROOT:$PATH

Enable 3.1: 
- export DOTNET_ROOT=$HOME/dotnet3
- export PATH=$DOTNET_ROOT:$PATH

Kolla SDK: dotnet --list-sdks

### Miniconda
cd /tmp
wget https://repo.anaconda.com/miniconda/Miniconda3-latest-Linux-x86_64.sh -O miniconda.sh
bash miniconda.sh -b -p $HOME/miniconda
export PATH="$HOME/miniconda/bin:$PATH"
rm /tmp/miniconda.sh



I use Anaconda to create a virtual developemnt environment.
* conda update --all
* conda search python
* conda config --add channels conda-forge

* conda create -n judotech python=3 nodejs=24
* conda activate judotech
* Script: judotech_env.bat

## Modules
* Azure Cli(Not sure, I have it installed globaly): conda install azure-cli-core
* Install in machine: winget install --exact --id Microsoft.AzureCLI


## Azure
### Azure Cli
- az --version
- az upgrade

### Azure Account
- Logion using dialog: az login --tenant <Tenant ID found in Azure Entra ID - Properties>
- Show my account details: az account show

### Resource Group
- List resource groups: az group list
- Create a new group: az group create --name Judoka --location swedencentral
- To remove the grouo and all content: az group delete --name Judoka --yes --no-wait 

### App Service Plan 
- Create an app service plan in the environment: az appservice plan create -g Judoka -n JudokaServicePlan

### Storage Account
- az storage account create -n JudokaStorage -g Judoka -l swedencentral --sku Standard_LRS
- To retrieve existing connectionstring: az storage account show-connection-string --name JudokaStorage --resource-group Judoka

[//]: # sku {Premium_LRS, Premium_ZRS, Standard_GRS, Standard_GZRS, Standard_LRS, Standard_RAGRS, Standard_RAGZRS, Standard_ZRS}]

### Cosmos DB
- az cosmosdb create --name judokacosmosdb --resource-group Judoka
- az cosmosdb keys list --name judokacosmosdb --resource-group Judoka
- az cosmosdb show --name judokacosmosdb --resource-group Judoka