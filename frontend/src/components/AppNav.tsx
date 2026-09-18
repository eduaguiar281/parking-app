import { NavLink } from "react-router-dom";
import { useEffect, useState } from "react";
import { useAuth } from "../auth/AuthContext";

const items = [
  { to: "/operacao", label: "Operação", section: "Operação", admin: false },
  { to: "/caixa", label: "Caixa", section: "Operação", admin: false },
  { to: "/historico", label: "Histórico", section: "Operação", admin: false },
  { to: "/vagas", label: "Vagas", section: "Cadastros", admin: true },
  { to: "/tarifas", label: "Tarifas", section: "Cadastros", admin: true },
  { to: "/relatorios", label: "Relatórios", section: "Cadastros", admin: true },
  { to: "/usuarios", label: "Usuários", section: "Cadastros", admin: true },
];

function BrandMark() {
  return (
    <span className="brand__mark" aria-hidden="true">
      <svg viewBox="0 0 24 24" width="16" height="16" fill="none">
        <path
          d="M7 4h7.5a4.5 4.5 0 0 1 0 9H11v7H7V4Zm4 6h3.5a1.5 1.5 0 0 0 0-3H11v3Z"
          fill="currentColor"
        />
      </svg>
    </span>
  );
}

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
  const sections = [...new Set(visible.map((item) => item.section))];

  return (
    <aside className="sidebar" aria-label="Menu">
      <div className="brand">
        <BrandMark />
        <div className="brand__text">
          <p className="brand__name">Parking App</p>
          <p className="brand__tag">Console</p>
        </div>
      </div>
      {narrow ? (
        <button className="nav__hamburger" type="button" aria-expanded={open} onClick={() => setOpen((v) => !v)}>
          Menu
        </button>
      ) : null}
      <nav className={narrow && !open ? "nav__links nav__links--hidden" : "nav__links"} aria-label="Principal">
        {sections.map((section) => (
          <div key={section}>
            <p className="nav__section">{section}</p>
            {visible
              .filter((item) => item.section === section)
              .map((item) => (
                <NavLink
                  key={item.to}
                  to={item.to}
                  className={({ isActive }) => (isActive ? "nav__item nav__item--active" : "nav__item")}
                  onClick={() => setOpen(false)}
                >
                  {item.label}
                </NavLink>
              ))}
          </div>
        ))}
      </nav>
      <div className="nav__user">
        <span>{user?.fullName}</span>
        <button className="button button--ghost" type="button" onClick={() => void logout()}>
          Sair
        </button>
      </div>
    </aside>
  );
}
