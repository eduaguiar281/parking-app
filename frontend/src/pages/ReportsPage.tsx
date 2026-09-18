import { FormEvent, useEffect, useState } from "react";
import { api, ApiError, download } from "../api/client";
import { dateOnly, dateTime, money, todayIso } from "../api/format";

type AuditEvent = {
  id: string;
  occurredAt: string;
  userName: string;
  action: string;
  entityType: string;
  entityId: string;
  changedData: string;
};

export function ReportsPage() {
  const [from, setFrom] = useState(todayIso());
  const [to, setTo] = useState(todayIso());
  const [cash, setCash] = useState<{ items: { operationalDate: string; expectedAmount: number; difference?: number | null }[] } | null>(null);
  const [stays, setStays] = useState<{ entryCount: number; exitCount: number; totalRevenue: number } | null>(null);
  const [audit, setAudit] = useState<AuditEvent[]>([]);
  const [error, setError] = useState("");

  async function load(event?: FormEvent) {
    event?.preventDefault();
    const q = `from=${from}&to=${to}`;
    const [cashReport, stayReport, events] = await Promise.all([
      api<{ items: { operationalDate: string; expectedAmount: number; difference?: number | null }[] }>(`/api/reports/cash?${q}`),
      api<{ entryCount: number; exitCount: number; totalRevenue: number }>(`/api/reports/stays?${q}`),
      api<AuditEvent[]>("/api/audit"),
    ]);
    setCash(cashReport);
    setStays(stayReport);
    setAudit(events);
  }

  useEffect(() => {
    void load().catch((err: unknown) => setError(err instanceof ApiError ? err.message : "Falha ao carregar relatórios."));
  }, []);

  return (
    <div className="stack">
      <h1>Relatórios</h1>
      {error ? <p className="banner banner--error">{error}</p> : null}
      <form className="card" onSubmit={(e) => void load(e)}>
        <div className="form-grid">
          <label className="field">
            <span className="field__label">De</span>
            <input className="field__input" type="date" value={from} onChange={(e) => setFrom(e.target.value)} />
          </label>
          <label className="field">
            <span className="field__label">Até</span>
            <input className="field__input" type="date" value={to} onChange={(e) => setTo(e.target.value)} />
          </label>
        </div>
        <div className="actions">
          <button className="button" type="submit">Atualizar</button>
          <button className="button button--ghost" type="button" onClick={() => window.print()}>Imprimir</button>
          <button className="button button--ghost" type="button" onClick={() => void download(`/api/reports/cash.pdf?from=${from}&to=${to}`, "caixa.pdf")}>PDF caixa</button>
          <button className="button button--ghost" type="button" onClick={() => void download(`/api/reports/cash.csv?from=${from}&to=${to}`, "caixa.csv")}>CSV caixa</button>
          <button className="button button--ghost" type="button" onClick={() => void download(`/api/reports/stays.pdf?from=${from}&to=${to}`, "estadias.pdf")}>PDF estadias</button>
          <button className="button button--ghost" type="button" onClick={() => void download(`/api/reports/stays.csv?from=${from}&to=${to}`, "estadias.csv")}>CSV estadias</button>
        </div>
      </form>
      <section className="card">
        <h2>Caixa</h2>
        {cash?.items.length ? cash.items.map((item) => (
          <p key={item.operationalDate}>{dateOnly(item.operationalDate)} · esperado {money(item.expectedAmount)} · diferença {money(item.difference)}</p>
        )) : <p>Nenhum caixa no período.</p>}
      </section>
      <section className="card">
        <h2>Estadias</h2>
        {stays ? (
          <p>Entradas {stays.entryCount} · saídas {stays.exitCount} · receita {money(stays.totalRevenue)}</p>
        ) : (
          <p>Nenhuma estadia no período.</p>
        )}
      </section>
      <section className="card" data-testid="audit-section">
        <h2>Auditoria</h2>
        {audit.length === 0 ? <p>Nenhum evento de auditoria.</p> : (
          <table className="table">
            <thead>
              <tr>
                <th>Quando</th>
                <th>Quem</th>
                <th>Ação</th>
                <th>Dados</th>
              </tr>
            </thead>
            <tbody>
              {audit.map((event) => (
                <tr key={event.id}>
                  <td>{dateTime(event.occurredAt)}</td>
                  <td>{event.userName}</td>
                  <td>{event.action}</td>
                  <td><code>{event.changedData}</code></td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
    </div>
  );
}
