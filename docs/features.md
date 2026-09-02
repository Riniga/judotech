## Features

### 1. Judotech.Core - Base Library for core functionalities used by more than one application


### 2. Judotech API - REST API for application shared access to data

### 3. Judotech.Web - The main website, loads submodules from Judotech.Web.*
The base for webpages in the platform, should contain templates and functionality to be imported and used in other webprojects.
NOT yet implmented, is under development. Functionality is listed here but will be implmented in each project, if not stated otherwise

#### 3.1. Judotech.Web -Login: Manages user logins and access
- A nodejs module, for the login functionality
- Includes UI for Login / Logout buttons, Display profile image, name and club
- As a user you get access from roles, we have the following roles for now: 
  1. judoka: View personal pages and profile
  2. coach: can add and edit trainingsessions, can mark participation in sessions 
  3. referee: can view referee pages
  4. admin: special pages for admin activities

### 4. Judotech.Web.Club - Athlete goto portal with personal pages, to track progress, view training, news etc
Pages for the athlete to view
* Startpage - Describe the site. Describe how to login or reset password. Links to login and reset.
* My Club: Display a club page, with maps, news, schedule and fees 
* Profile Page: Displays current user information, grade with date taken, goal, next level.....

### 5. Judotech.Web.Referee - A Website for referees to view there progress, scoring and competition to referee
* Startpage - Describe the site. 
* Login - Full functionality with reset and so onDescribe how to login or reset password. Links to login and reset.
* Domararvode - Formulär som skickast till tävlingsledare





### 6. Judotech.Web.Coach - A Website for the coach to plan and execute training sessions

### 1. Användarregistrering
- Möjliggör att skapa konto med e-post och lösenord.
- Krav: Validering, e-postbekräftelse.

### 2. Dokumentuppladdning
- Stöd för PDF, DOCX, och bilder.
- Krav: Maxstorlek 20 MB, virusskanning.




