import { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

const items = [
  { to: "/operacao", label: "Operação", admin: false },
  { to: "/vagas", label: "Vagas", admin: true },
  { to: "/tarifas", label: "Tarifas", admin: true },
  { to: "/caixa", label: "Caixa", admin: false },
  { to: "/historico", label: "Histórico", admin: false },
  { to: "/relatorios", label: "Relatórios", admin: true },
  { to: "/usuarios", label: "Usuários", admin: true },
];

export function AppNav() {
  const { user, logout } = useAuth();
  const [narrow, setNarrow] = useState(() => window.innerWidth <= 833);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    const onResize = () => setNarrow(window.innerWidth <= 833);
    window.addEventListener("resize", onResize);
    return () => window.removeEventListener("resize", onResize);
  }, []);

  const visible = items.filter((item) => !item.admin || user?.role === "Administrator");

  return (
    <header className="nav">
      <span className="nav__brand">Parking App</span>
      {narrow ? (
        <button className="nav__hamburger" type="button" aria-expanded={open} onClick={() => setOpen((v) => !v)}>
          Menu
        </button>
      ) : null}
      <nav className={narrow && !open ? "nav__links nav__links--hidden" : "nav__links"} aria-label="Principal">
        {visible.map((item) => (
          <NavLink key={item.to} to={item.to} className="nav__link" onClick={() => setOpen(false)}>
            {item.label}
          </NavLink>
        ))}
      </nav>
      <div className="nav__user">
        <span>{user?.fullName}</span>
        <button className="button button--ghost" type="button" onClick={() => void logout()}>
          Sair
        </button>
      </div>
    </header>
  );
}
