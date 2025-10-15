# Development Environment

## Local Development Machine Requirements
winget install --id Git.Git --scope machine
winget install --id Microsoft.VisualStudioCode --scope machine
winget install --id Microsoft.PowerShell --scope machine
winget install --id 7zip.7zip --scope machine
winget install --id Google.Chrome --scope machine
winget install --id Docker.DockerDesktop --scope machine
winget install --id Notepad++.Notepad++ --scope machine
winget install --id WinMerge.WinMerge --scope machine
winget install --id Microsoft.PowerToys --scope machine
winget install --id Microsoft.AzureCLI --scope machine
winget install --id Microsoft.Azure.StorageExplorer --scope machine
winget install --id CondaForge.Mambaforge
   1. conda config --add channels conda-forge
   2. conda config --add channels pytorch
   3. conda config --set channel_priority strict
   4. conda install -n base -c conda-forge mamba


winget install Postman.Postman # Only user scope

### Dotnet 8.0
winget install --id Microsoft.DotNet.SDK.8 --scope machine
dotnet --list-sdks


# Development Environment
Using Anaconda to create a virtual developemnt environment.
1. conda update --all
2. conda search python
3. conda create -n judotech python=3 nodejs=24
4. conda activate judotech

## Modules
1. npm install -g azure-functions-core-tools@3 --unsafe-perm true
2. npm install -g gulp