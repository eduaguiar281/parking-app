import { FormEvent, useEffect, useState } from "react";
import { api, ApiError } from "../api/client";
import { statusLabel } from "../api/format";

type UserRow = {
  id: string;
  fullName: string;
  login: string;
  role: string;
  status: string;
};

export function UsersPage() {
  const [users, setUsers] = useState<UserRow[]>([]);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const [form, setForm] = useState({
    fullName: "",
    login: "",
    password: "",
    role: "Operator",
    status: "Active",
  });

  async function load() {
    setUsers(await api<UserRow[]>("/api/users"));
  }

  useEffect(() => {
    void load().catch((err: unknown) => setError(err instanceof ApiError ? err.message : "Falha ao carregar usuários."));
  }, []);

  async function create(event: FormEvent) {
    event.preventDefault();
    try {
      await api("/api/users", { method: "POST", body: JSON.stringify(form) });
      setMessage("Usuário criado.");
      setForm({ fullName: "", login: "", password: "", role: "Operator", status: "Active" });
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível criar o usuário.");
    }
  }

  async function toggle(user: UserRow) {
    const path = user.status === "Active" ? "deactivate" : "activate";
    try {
      await api(`/api/users/${user.id}/${path}`, { method: "POST" });
      setMessage(user.status === "Active" ? "Usuário inativado." : "Usuário reativado.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível alterar o status.");
    }
  }

  return (
    <div className="stack">
      <h1>Usuários</h1>
      {error ? <p className="banner banner--error">{error}</p> : null}
      {message ? <p className="banner banner--ok">{message}</p> : null}
      <form className="card" onSubmit={(e) => void create(e)}>
        <h2>Novo usuário</h2>
        <div className="form-grid">
          <label className="field">
            <span className="field__label">Nome</span>
            <input className="field__input" value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} />
          </label>
          <label className="field">
            <span className="field__label">Login</span>
            <input className="field__input" value={form.login} onChange={(e) => setForm({ ...form, login: e.target.value })} />
          </label>
          <label className="field">
            <span className="field__label">Senha</span>
            <input className="field__input" type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
          </label>
          <label className="field">
            <span className="field__label">Perfil</span>
            <select className="field__input" value={form.role} onChange={(e) => setForm({ ...form, role: e.target.value })}>
              <option value="Operator">Operador</option>
              <option value="Administrator">Administrador</option>
            </select>
          </label>
        </div>
        <button className="button" type="submit">Criar</button>
      </form>
      <section className="card">
        {users.length === 0 ? <p>Nenhum usuário cadastrado.</p> : (
          <table className="table">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Login</th>
                <th>Perfil</th>
                <th>Status</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td>{user.fullName}</td>
                  <td>{user.login}</td>
                  <td>{statusLabel[user.role] ?? user.role}</td>
                  <td>{statusLabel[user.status] ?? user.status}</td>
                  <td>
                    <button className="button button--ghost" type="button" onClick={() => void toggle(user)}>
                      {user.status === "Active" ? "Inativar" : "Reativar"}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
    </div>
  );
}
