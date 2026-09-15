import { useState } from "react";
import "./styles/tokens.css";
import "./styles/app.css";

export default function App() {
  const [heraldName, setHeraldName] = useState("");

  return (
    <main className="page">
      <h1>Parking App</h1>
      <article className="card">
        <p className="card__lead">
          Vitrine da identidade visual. Esta tela não consulta o serviço.
        </p>
        <label className="field">
          <span className="field__label">Nome do arauto</span>
          <input
            className="field__input"
            value={heraldName}
            onChange={(event) => setHeraldName(event.target.value)}
            placeholder="Escreva aqui"
          />
        </label>
        <button className="button" type="button">
          Confirmar
        </button>
      </article>
    </main>
  );
}
