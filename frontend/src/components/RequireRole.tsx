import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export function RequireAuth() {
  const { user, loading } = useAuth();
  if (loading) {
    return <p className="page">Carregando…</p>;
  }
  if (!user) {
    return <Navigate to="/login" replace />;
  }
  return <Outlet />;
}

export function RequireRole({ admin }: { admin?: boolean }) {
  const { user } = useAuth();
  if (admin && user?.role !== "Administrator") {
    return (
      <section className="card">
        <h1>Acesso recusado</h1>
        <p className="card__lead">Você não tem permissão para acessar esta página.</p>
      </section>
    );
  }
  return <Outlet />;
}
