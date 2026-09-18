import { FormEvent, useEffect, useState } from "react";
import { api, ApiError } from "../api/client";
import { money, statusLabel, todayIso, vehicleLabel } from "../api/format";

type Tariff = {
  id: string;
  name: string;
  vehicleType: string;
  sectorId?: string | null;
  firstHourAmount: number;
  additionalHourAmount: number;
  effectiveFrom: string;
  effectiveTo?: string | null;
  status: string;
};
type Sector = { id: string; name: string; code: string };

export function TariffsPage() {
  const [tariffs, setTariffs] = useState<Tariff[]>([]);
  const [sectors, setSectors] = useState<Sector[]>([]);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const empty = {
    name: "",
    vehicleType: "Car",
    sectorId: "",
    firstHourAmount: 10,
    additionalHourAmount: 5,
    effectiveFrom: todayIso(),
    status: "Active",
  };
  const [form, setForm] = useState(empty);

  async function load() {
    setTariffs(await api<Tariff[]>("/api/tariffs"));
    setSectors(await api<Sector[]>("/api/sectors"));
  }

  useEffect(() => {
    void load().catch((err: unknown) => setError(err instanceof ApiError ? err.message : "Falha ao carregar tarifas."));
  }, []);

  async function save(event: FormEvent) {
    event.preventDefault();
    if (!window.confirm("Confirmar cadastro ou alteração desta tarifa?")) {
      return;
    }
    try {
      await api("/api/tariffs", {
        method: "POST",
        body: JSON.stringify({ ...form, sectorId: form.sectorId || null }),
      });
      setMessage("Tarifa gravada.");
      setForm(empty);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível gravar a tarifa.");
    }
  }

  async function deactivate(id: string) {
    try {
      await api(`/api/tariffs/${id}/deactivate`, { method: "POST" });
      setMessage("Tarifa inativada.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível inativar.");
    }
  }

  return (
    <div className="stack">
      <h1>Tarifas</h1>
      {error ? <p className="banner banner--error">{error}</p> : null}
      {message ? <p className="banner banner--ok">{message}</p> : null}
      <form className="card" onSubmit={(e) => void save(e)}>
        <h2>Nova tabela</h2>
        <div className="form-grid">
          <label className="field">
            <span className="field__label">Nome</span>
            <input className="field__input" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
          </label>
          <label className="field">
            <span className="field__label">Tipo</span>
            <select className="field__input" value={form.vehicleType} onChange={(e) => setForm({ ...form, vehicleType: e.target.value })}>
              <option value="Car">Carro</option>
              <option value="Motorcycle">Moto</option>
            </select>
          </label>
          <label className="field">
            <span className="field__label">Setor (opcional)</span>
            <select className="field__input" value={form.sectorId} onChange={(e) => setForm({ ...form, sectorId: e.target.value })}>
              <option value="">Geral</option>
              {sectors.map((s) => (
                <option key={s.id} value={s.id}>{s.code} — {s.name}</option>
              ))}
            </select>
          </label>
          <label className="field">
            <span className="field__label">Primeira hora</span>
            <input className="field__input" type="number" step="0.01" value={form.firstHourAmount} onChange={(e) => setForm({ ...form, firstHourAmount: Number(e.target.value) })} />
          </label>
          <label className="field">
            <span className="field__label">Hora adicional</span>
            <input className="field__input" type="number" step="0.01" value={form.additionalHourAmount} onChange={(e) => setForm({ ...form, additionalHourAmount: Number(e.target.value) })} />
          </label>
          <label className="field">
            <span className="field__label">Vigência</span>
            <input className="field__input" type="date" value={form.effectiveFrom} onChange={(e) => setForm({ ...form, effectiveFrom: e.target.value })} />
          </label>
        </div>
        <button className="button" type="submit">Salvar tarifa</button>
      </form>
      <section className="card">
        <h2>Tabelas</h2>
        {tariffs.length === 0 ? <p>Nenhuma tarifa cadastrada.</p> : (
          <table className="table">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Tipo</th>
                <th>Valores</th>
                <th>Status</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {tariffs.map((t) => (
                <tr key={t.id}>
                  <td>{t.name}</td>
                  <td>{vehicleLabel[t.vehicleType] ?? t.vehicleType}</td>
                  <td>{money(t.firstHourAmount)} / {money(t.additionalHourAmount)}</td>
                  <td>{statusLabel[t.status] ?? t.status}</td>
                  <td>
                    {t.status === "Active" ? (
                      <button className="button button--ghost" type="button" onClick={() => void deactivate(t.id)}>Inativar</button>
                    ) : null}
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
