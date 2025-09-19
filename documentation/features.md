## Funktioner


### 1. Judotech.Core - Base Library for core functionalities used by more than one application

### 2. Judotech API - REST API for application shared access to data


### 3. Judotech.Web - The main website, loads submodules from Judotech.Web.*
A basic website to with a basic welcome page

### 4. Judotech.Web.Login - Manages user logins and access
- A nodejs module, for the login functionality
- Includes UI for Login / Logout buttons, Display profile image, name and club
- As a user you get access from roles, we have the following roles for now: 
  1. judoka: View personal pages and profile
  2. coach: can add and edit trainingsessions, can mark participation in sessions 
  3. referee: can view referee pages
  4. admin: special pages for admin activities

### 5. Judotech.Web.Athlete - Athlete personal pages. Track progress, view training, news etc
Pages for the athlete to view
Profilepage: Displays current grade nad date taken, goal

### 6. Judotech.Web.Coach - A Website for the coach to plan and execute training sessions

### 1. Användarregistrering
- Möjliggör att skapa konto med e-post och lösenord.
- Krav: Validering, e-postbekräftelse.

### 2. Dokumentuppladdning
- Stöd för PDF, DOCX, och bilder.
- Krav: Maxstorlek 20 MB, virusskanning.




