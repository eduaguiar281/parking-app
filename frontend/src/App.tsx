import { useState } from "react";
import "./styles/tokens.css";
import "./styles/app.css";

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

function GridIcon() {
  return (
    <svg
      className="nav__icon"
      viewBox="0 0 24 24"
      width="16"
      height="16"
      fill="none"
      aria-hidden="true"
    >
      <rect x="3" y="3" width="7" height="7" rx="1.5" stroke="currentColor" strokeWidth="1.8" />
      <rect x="14" y="3" width="7" height="7" rx="1.5" stroke="currentColor" strokeWidth="1.8" />
      <rect x="3" y="14" width="7" height="7" rx="1.5" stroke="currentColor" strokeWidth="1.8" />
      <rect x="14" y="14" width="7" height="7" rx="1.5" stroke="currentColor" strokeWidth="1.8" />
    </svg>
  );
}

export default function App() {
  const [query, setQuery] = useState("");

  return (
    <div className="shell">
      <aside className="sidebar" aria-label="Menu">
        <div className="brand">
          <BrandMark />
          <div className="brand__text">
            <p className="brand__name">Parking App</p>
            <p className="brand__tag">Console</p>
          </div>
        </div>

        <p className="nav__section">Operação</p>
        <nav>
          <button
            type="button"
            className="nav__item nav__item--active"
            aria-current="page"
          >
            <GridIcon />
            <span>Vitrine</span>
          </button>
        </nav>
      </aside>

      <div className="workspace">
        <header className="topbar">
          <h1>Vitrine</h1>
        </header>

        <main className="content">
          <article className="card">
            <div className="card__header">
              <h2 className="card__title">Identidade visual</h2>
              <p className="card__lead">
                Vitrine da identidade visual. Esta tela não consulta o serviço.
              </p>
            </div>
            <div className="card__body">
              <label className="field">
                <span className="field__label">Pesquisar</span>
                <input
                  className="field__input"
                  value={query}
                  onChange={(event) => setQuery(event.target.value)}
                  placeholder="Escreva aqui"
                />
              </label>
              <button className="button" type="button">
                Saiba mais
              </button>
            </div>
          </article>
        </main>
      </div>
    </div>
  );
}
