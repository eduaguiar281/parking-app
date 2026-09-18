import { useState } from "react";
import "./styles/tokens.css";
import "./styles/app.css";

export default function App() {
  const [query, setQuery] = useState("");

  return (
    <main className="page">
      <h1>Parking App</h1>
      <article className="card">
        <p className="card__lead">
          Vitrine da identidade visual. Esta tela não consulta o serviço.
        </p>
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
      </article>
    </main>
  );
}
