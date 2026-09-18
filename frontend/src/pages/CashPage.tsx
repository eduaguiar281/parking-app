import { FormEvent, useEffect, useState } from "react";
import { api, ApiError } from "../api/client";
import { dateOnly, money, todayIso } from "../api/format";
import { useAuth } from "../auth/AuthContext";

type Cash = {
  id: string;
  operationalDate: string;
  status: string;
  openingAmount: number;
  expectedAmount: number;
  informedClosingAmount?: number | null;
  difference?: number | null;
  totalsByPaymentMethod: Record<string, number>;
};

export function CashPage() {
  const { user } = useAuth();
  const [cash, setCash] = useState<Cash | null>(null);
  const [openingAmount, setOpeningAmount] = useState(100);
  const [informed, setInformed] = useState(0);
  const [movement, setMovement] = useState({ type: "Supply", amount: 0, reason: "" });
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  async function load() {
    try {
      setCash(await api<Cash>("/api/cash/current"));
    } catch (err) {
      if (err instanceof ApiError && err.status === 404) {
        setCash(null);
        return;
      }
      throw err;
    }
  }

  useEffect(() => {
    void load().catch((err: unknown) => setError(err instanceof ApiError ? err.message : "Falha ao carregar o caixa."));
  }, []);

  async function openCash(event: FormEvent) {
    event.preventDefault();
    try {
      await api("/api/cash", {
        method: "POST",
        body: JSON.stringify({ operationalDate: todayIso(), openingAmount }),
      });
      setMessage("Caixa aberto.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível abrir o caixa.");
    }
  }

  async function closeCash(event: FormEvent) {
    event.preventDefault();
    if (!cash || !window.confirm("Confirmar fechamento do caixa do dia?")) {
      return;
    }
    try {
      const closed = await api<Cash>(`/api/cash/${cash.id}/close`, {
        method: "POST",
        body: JSON.stringify({ informedAmount: informed, notes: "" }),
      });
      setCash(closed);
      setMessage("Caixa fechado.");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível fechar o caixa.");
    }
  }

  async function reopen() {
    if (!cash || !window.confirm("Reabrir este caixa? A correção só pode ser sangria, suprimento ou ajuste.")) {
      return;
    }
    const reason = window.prompt("Motivo da reabertura:") ?? "";
    if (!reason) {
      return;
    }
    try {
      await api(`/api/cash/${cash.id}/reopen`, { method: "POST", body: JSON.stringify({ reason }) });
      setMessage("Caixa reaberto.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível reabrir.");
    }
  }

  async function addMovement(event: FormEvent) {
    event.preventDefault();
    if (!cash) {
      return;
    }
    try {
      await api(`/api/cash/${cash.id}/movements`, { method: "POST", body: JSON.stringify(movement) });
      setMessage("Movimento lançado.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível lançar o movimento.");
    }
  }

  return (
    <div className="stack">
      <h1>Caixa</h1>
      {error ? <p className="banner banner--error">{error}</p> : null}
      {message ? <p className="banner banner--ok">{message}</p> : null}
      {cash ? (
        <section className="card">
          <h2>Caixa {cash.status === "Open" ? "aberto" : "fechado"}</h2>
          <p>Data operacional: {dateOnly(cash.operationalDate)}</p>
          <p>Abertura: {money(cash.openingAmount)}</p>
          <p>Esperado: {money(cash.expectedAmount)}</p>
          {cash.difference != null ? <p>Diferença: {money(cash.difference)}</p> : null}
          {cash.status === "Open" ? (
            <form onSubmit={(e) => void closeCash(e)}>
              <label className="field">
                <span className="field__label">Valor informado no fechamento</span>
                <input className="field__input" type="number" step="0.01" value={informed} onChange={(e) => setInformed(Number(e.target.value))} />
              </label>
              <button className="button" type="submit">Fechar caixa</button>
            </form>
          ) : user?.role === "Administrator" ? (
            <button className="button" type="button" onClick={() => void reopen()}>Reabrir caixa</button>
          ) : null}
        </section>
      ) : null}
      {!cash || cash.status === "Closed" ? (
        <form className="card" onSubmit={(e) => void openCash(e)}>
          <h2>Abrir caixa do dia</h2>
          {!cash ? <p>Nenhum caixa aberto.</p> : null}
          <label className="field">
            <span className="field__label">Valor de abertura</span>
            <input className="field__input" type="number" step="0.01" value={openingAmount} onChange={(e) => setOpeningAmount(Number(e.target.value))} />
          </label>
        <button className="button" type="submit">Abrir caixa</button>
        </form>
      ) : null}
      {cash?.status === "Open" && user?.role === "Administrator" ? (
        <form className="card" onSubmit={(e) => void addMovement(e)}>
          <h2>Sangria, suprimento ou ajuste</h2>
          <div className="form-grid">
            <label className="field">
              <span className="field__label">Tipo</span>
              <select className="field__input" value={movement.type} onChange={(e) => setMovement({ ...movement, type: e.target.value })}>
                <option value="Supply">Suprimento</option>
                <option value="Bleed">Sangria</option>
                <option value="Adjustment">Ajuste</option>
              </select>
            </label>
            <label className="field">
              <span className="field__label">Valor</span>
              <input className="field__input" type="number" step="0.01" value={movement.amount} onChange={(e) => setMovement({ ...movement, amount: Number(e.target.value) })} />
            </label>
            <label className="field">
              <span className="field__label">Motivo</span>
              <input className="field__input" value={movement.reason} onChange={(e) => setMovement({ ...movement, reason: e.target.value })} />
            </label>
          </div>
          <button className="button" type="submit">Lançar</button>
        </form>
      ) : null}
    </div>
  );
}
