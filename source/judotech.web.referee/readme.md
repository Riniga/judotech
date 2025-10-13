# Prerequisit

## Nodejs
Använd long-term support (LTS) är versionr med jämnt nummer, just nu 22.13.0 20.18.0 (conda install  nodejs"<21")
- conda install nodejs 
- Alternativt: conda install  nodejs"<23"

## Gulp
- npm install -g gulp
- npm install live-server -g

# Installation
- Installera: npm install


# Upgrading
npm outdated
npm update --save

# Prepare and Watch (in terminal 1)
- cd source\judotech.web 
- npm install
- gulp --environment <production | test | uat | development>
- gulp watch

# Starta Webserver (in terminal 2) 
- cd source\judotech.web 
- start live-server --port=4145 public


# Dependabot
Om dependabot klagar så här fixade jag det sist....

1. Hämta senaste versionen från github
2. Starta upp allt och tillse att det fungerar som det ska
3. Undersök var modulen kommer ifrån: npm ls js-yaml och/eller npm why js-yaml
4. Uppdatera modulen, eller hitta andra moduler etc.... 

5. Uppdatera samtliga paket...
6. Upgradera samtliga paket...








