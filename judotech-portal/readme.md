# Judotech Portal Workspaces

This is the root for our workspaces and contains common stuff

## Catalog structure
judo-portal/
├─ apps/
│  ├─ public/          # Besökare
│  │  └─ src/
│  ├─ trainer/         # Tränare
│  │  └─ src/
│  ├─ athlete/         # Tränande
│  │  └─ src/
│  └─ referee/         # Domare
│     └─ src/
│
├─ packages/
│  ├─ ui/              # Gemensamt UI-bibliotek (knappar, tabeller, layout, tema)
│  │  ├─ src/
│  │  ├─ ├─ components/
│  │  │  │  ├─ Table/
│  │  │  │  ├─ Button/
│  │  │  │  ├─ Card/
│  │  │  │  └─ Layout/
│  │  │  ├─ theme/
│  │  │  │  ├─ colors.ts
│  │  │  │  └─ typography.ts
│  │  │  └─ index.ts
│  │  └─ package.json
│  ├─ core/            # Domänlogik, typer, hooks, API-klienter
│  │  packages/core/
│  │  ├─ src/
│  │  │  ├─ api/
│  │  │  │  ├─ httpClient.ts
│  │  │  │  └─ judoApi.ts        // t.ex. anrop till ditt backend
│  │  │  ├─ hooks/
│  │  │  │  ├─ useAuth.ts
│  │  │  │  └─ useCurrentUser.ts
│  │  │  ├─ models/
│  │  │  │  ├─ User.ts
│  │  │  │  └─ TrainingSession.ts
│  │  │  └─ index.ts
│  │  └─ package.json
│  └─ config/          # Delad tsconfig, eslint, tailwind-config etc.
│
├─ package.json        # workspaces / pnpm/yarn workspace
├─ tsconfig.base.json
└─ README.md


## Todo:
Använd UI från ui: import { Table, Layout, Button } from '@judo/ui';
lägga till Tailwind för snyggare tabell-UI, eller
Bygga med     "build": "turbo build"  // om du vill ta det steget


## Skapa site
npm create vite@latest apps/athlete -- --template react-ts
npm install -D tailwindcss postcss autoprefixer --workspace @judotech/athlete




## Run site(s)
npm install
npm npm run dev:athlete

