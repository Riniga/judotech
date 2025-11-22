import './App.css'
import { Button, Card } from "@judotech/ui";


function App() {

  return (
    <div className="min-h-screen bg-slate-100">
      <header className="px-6 py-4 bg-white shadow flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-slate-800">Athlete Portal</h1>
          <p className="text-sm text-slate-500">athlete.judotech.se</p>
        </div>
        <Button variant="secondary">Logga in</Button>
      </header>

      <main className="p-6 space-y-4">
        <Card>
          <h2 className="text-lg font-semibold mb-2">Mina träningar</h2>
          <p className="text-sm text-slate-600">
            Här kommer en tabell med kommande pass.
          </p>
        </Card>

        <Card>
          <h2 className="text-lg font-semibold mb-2">Statistik</h2>
          <p className="text-sm text-slate-600">
            Här kan vi senare lägga grafer och annan interaktiv data.
          </p>
        </Card>
      </main>
    </div>
  )
}

export default App
