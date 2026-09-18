import { BrowserRouter, Navigate, Outlet, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./auth/AuthContext";
import { AppNav } from "./components/AppNav";
import { RequireAuth, RequireRole } from "./components/RequireRole";
import { CashPage } from "./pages/CashPage";
import { HistoryPage } from "./pages/HistoryPage";
import { LoginPage } from "./pages/LoginPage";
import { OperationPage } from "./pages/OperationPage";
import { ReportsPage } from "./pages/ReportsPage";
import { SpotsPage } from "./pages/SpotsPage";
import { TariffsPage } from "./pages/TariffsPage";
import { UsersPage } from "./pages/UsersPage";
import "./styles/tokens.css";
import "./styles/app.css";

function Shell() {
  return (
    <div className="shell">
      <AppNav />
      <div className="workspace">
        <Outlet />
      </div>
    </div>
  );
}

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<RequireAuth />}>
        <Route element={<Shell />}>
          <Route path="/" element={<Navigate to="/operacao" replace />} />
          <Route path="/operacao" element={<OperationPage />} />
          <Route path="/caixa" element={<CashPage />} />
          <Route path="/historico" element={<HistoryPage />} />
          <Route element={<RequireRole admin />}>
            <Route path="/vagas" element={<SpotsPage />} />
            <Route path="/tarifas" element={<TariffsPage />} />
            <Route path="/relatorios" element={<ReportsPage />} />
            <Route path="/usuarios" element={<UsersPage />} />
          </Route>
        </Route>
      </Route>
      <Route path="*" element={<Navigate to="/operacao" replace />} />
    </Routes>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
}
