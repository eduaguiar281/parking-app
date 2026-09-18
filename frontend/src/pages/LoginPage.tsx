import { FormEvent, useState } from "react";
import { Navigate } from "react-router-dom";
import { ApiError } from "../api/client";
import { useAuth } from "../auth/AuthContext";

export function LoginPage() {
  const { user, loading, login } = useAuth();
  const [loginName, setLoginName] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [pending, setPending] = useState(false);

  if (!loading && user) {
    return <Navigate to="/operacao" replace />;
  }

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setError("");
    setPending(true);
    try {
      await login(loginName, password);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Login ou senha inválidos.");
    } finally {
      setPending(false);
    }
  }

  return (
    <main className="page">
      <h1>Entrar</h1>
      <form className="card" onSubmit={(e) => void onSubmit(e)}>
        <p className="card__lead">Acesso ao pátio e ao caixa do dia.</p>
        {error ? <p className="banner banner--error">{error}</p> : null}
        <label className="field">
          <span className="field__label">Login</span>
          <input className="field__input" name="login" autoComplete="username" value={loginName} onChange={(e) => setLoginName(e.target.value)} />
        </label>
        <label className="field">
          <span className="field__label">Senha</span>
          <input className="field__input" name="password" type="password" autoComplete="current-password" value={password} onChange={(e) => setPassword(e.target.value)} />
        </label>
        <button className="button" type="submit" disabled={pending}>
          Entrar
        </button>
      </form>
    </main>
  );
}
