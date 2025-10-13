## Roadmap

### v1.0.0-alpha (Current development)
* Development: 1.0.0-alpha.1
    * Core: 1.0.0-alpha.1
    * API: 1.0.0-alpha.1
    * Referree: 1.0.0-alpha.1
* Test: 1.0.0-beta.1 - NOT RELEASED
* UAT: 1.0.0-RC.1 - NOT RELEASED
* Production: 1.0 (2025.04) - NOT RELEASED

### v1.0.0-beta (Next)
En första applikation är klar, resten är under utveckling
✅ Core - Base Library for core functionalities used by more than one application
✅ API - REST API for application shared access to data
🔄 Judotech.Web - The main website, loads submodules from Judotech.Web.*
🔄 Judotech.Web.Login - Manages user logins and access
🔄 Judotech.Web.Club - Athlete goto portal for personal pages, track progress, view training, news etc
🔄 Judotech.Web.Referee - A Website for referees to view there progress, scoring and competition to referee
⬜ Judotech.Web.Coach - A Website for the coach to plan and execute training sessions
⬜ Judotech.Web.Calendar - Display events in all divisions, enable registration
⬜ Athlete.App v1.0.0-beta - A Mobile App for the athletet, to track progress, get news and more
⬜ Results.Web - Display event results, news and more
⬜ Scoreboard.App - An application to track the result in an ongoing match
⬜ Competition.Server - A Server to manage a competition
⬜ Streaming.Server - A Server to manage and store stringing data from streamers
⬜ Streaming.App - An Application to stream data to the server
⬜ Care.App - An Application for referee to review the stream
⬜ Intercom.App - An Application to communicate


### Versioning (copy from readme.md)
Every application has a version number (Major.Minor.Patch) indicating it's current status:
* Major: Breaking changes
* Minor: New features (non-breaking)
* Patch: Bug fixes, performance improvements

To indicate the stability, we add:
⬜ alpha: Under development, unstable and incomplete
🔄 beta: Complete, but unstable - in testenvifonment
✅ RC: Release Candidate, should be stable