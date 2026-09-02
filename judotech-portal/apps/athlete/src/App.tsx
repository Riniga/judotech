import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { AppLayout } from "@judotech/ui"; // eller var du exporterar den
import Dashboard from "./pages/Dashboard"; // din första sida

export default function App() {
  return (
    <Router>
      <Routes>
        {/* Dashboard Layout */}
        <Route element={<AppLayout />}>
          <Route index path="/" element={<Dashboard />} />
          {/* fler routes här senare */}
        </Route>

        {/* Fallback / 404 (frivilligt just nu) */}
        {/* <Route path="*" element={<NotFound />} /> */}
      </Routes>
    </Router>
  );
}
