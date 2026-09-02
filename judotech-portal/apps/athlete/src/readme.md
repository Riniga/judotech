# judo athletes web site
View your current progress, events and training

## Structure

apps/trainer/src/
├─ main.tsx
├─ App.tsx
├─ app/
│  ├─ routes/          # React Router-konfiguration
│  ├─ layout/          # sidlayout, topbar, sidomeny (kan bygga på @judo/ui)
│  └─ features/        # större funktionsområden
│     ├─ schedule/
│     ├─ grading/
│     └─ attendance/
├─ components/         # små app-specifika komponenter
├─ styles/             # ev. global css/tailwind setup
└─ lib/                # app-specifik hjälp-logik

