import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import "@judotech/ui/styles/index.css";

import App from './App.tsx'
import { ThemeProvider }from "@judotech/ui"; 

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ThemeProvider>
        <App />
    </ThemeProvider>
  </StrictMode>,
)
