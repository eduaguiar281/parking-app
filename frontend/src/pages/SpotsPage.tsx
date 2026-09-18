import { FormEvent, useEffect, useState } from "react";
import { api, ApiError } from "../api/client";
import { statusLabel, vehicleLabel } from "../api/format";

type Sector = {
  id: string;
  name: string;
  code: string;
  description?: string | null;
  allowedCategories: string;
  status: string;
};
type Spot = {
  id: string;
  code: string;
  sectorId: string;
  vehicleCategory: string;
  status: string;
};

export function SpotsPage() {
  const [sectors, setSectors] = useState<Sector[]>([]);
  const [spots, setSpots] = useState<Spot[]>([]);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const [sectorForm, setSectorForm] = useState({ name: "", code: "", allowedCategories: "Car", status: "Active" });
  const [batch, setBatch] = useState({ sectorId: "", prefix: "P-A-", start: 1, end: 3 });

  async function load() {
    setSectors(await api<Sector[]>("/api/sectors"));
    setSpots(await api<Spot[]>("/api/spots"));
  }

  useEffect(() => {
    void load().catch((err: unknown) => setError(err instanceof ApiError ? err.message : "Falha ao carregar vagas."));
  }, []);

  async function createSector(event: FormEvent) {
    event.preventDefault();
    try {
      await api("/api/sectors", { method: "POST", body: JSON.stringify(sectorForm) });
      setMessage("Setor cadastrado.");
      setSectorForm({ name: "", code: "", allowedCategories: "Car", status: "Active" });
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível cadastrar o setor.");
    }
  }

  async function createBatch(event: FormEvent) {
    event.preventDefault();
    try {
      await api("/api/spots/batch", { method: "POST", body: JSON.stringify(batch) });
      setMessage("Lote de vagas criado.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível criar o lote.");
    }
  }

  async function setSpotStatus(spot: Spot, status: string) {
    try {
      await api(`/api/spots/${spot.id}`, {
        method: "PUT",
        body: JSON.stringify({ ...spot, status }),
      });
      setMessage("Status da vaga atualizado.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível alterar a vaga.");
    }
  }

  return (
    <div className="stack">
      <h1>Vagas</h1>
      {error ? <p className="banner banner--error">{error}</p> : null}
      {message ? <p className="banner banner--ok">{message}</p> : null}

      <form className="card" onSubmit={(e) => void createSector(e)}>
        <h2>Novo setor</h2>
        <div className="form-grid">
          <label className="field">
            <span className="field__label">Nome</span>
            <input className="field__input" value={sectorForm.name} onChange={(e) => setSectorForm({ ...sectorForm, name: e.target.value })} />
          </label>
          <label className="field">
            <span className="field__label">Código</span>
            <input className="field__input" value={sectorForm.code} onChange={(e) => setSectorForm({ ...sectorForm, code: e.target.value })} />
          </label>
          <label className="field">
            <span className="field__label">Categorias</span>
            <select className="field__input" value={sectorForm.allowedCategories} onChange={(e) => setSectorForm({ ...sectorForm, allowedCategories: e.target.value })}>
              <option value="Car">Carros</option>
              <option value="Motorcycle">Motos</option>
              <option value="Both">Carros e motos</option>
            </select>
          </label>
        </div>
        <button className="button" type="submit">Cadastrar setor</button>
      </form>

      <form className="card" onSubmit={(e) => void createBatch(e)}>
        <h2>Lote de vagas</h2>
        <div className="form-grid">
          <label className="field">
            <span className="field__label">Setor</span>
            <select className="field__input" value={batch.sectorId} onChange={(e) => setBatch({ ...batch, sectorId: e.target.value })}>
              <option value="">Selecione</option>
              {sectors.map((s) => (
                <option key={s.id} value={s.id}>{s.code} — {s.name}</option>
              ))}
            </select>
          </label>
          <label className="field">
            <span className="field__label">Prefixo</span>
            <input className="field__input" value={batch.prefix} onChange={(e) => setBatch({ ...batch, prefix: e.target.value })} />
          </label>
          <label className="field">
            <span className="field__label">De</span>
            <input className="field__input" type="number" value={batch.start} onChange={(e) => setBatch({ ...batch, start: Number(e.target.value) })} />
          </label>
          <label className="field">
            <span className="field__label">Até</span>
            <input className="field__input" type="number" value={batch.end} onChange={(e) => setBatch({ ...batch, end: Number(e.target.value) })} />
          </label>
        </div>
        <button className="button" type="submit">Gerar lote</button>
      </form>

      <section className="card">
        <h2>Mapa</h2>
        {spots.length === 0 ? <p>Nenhuma vaga cadastrada.</p> : (
          <div className="spot-grid">
            {spots.map((spot) => (
              <article key={spot.id} className={`spot-tile spot-tile--${spot.status.toLowerCase()}`}>
                <strong>{spot.code}</strong>
                <span>{vehicleLabel[spot.vehicleCategory] ?? spot.vehicleCategory}</span>
                <span>{statusLabel[spot.status] ?? spot.status}</span>
                {spot.status === "Free" || spot.status === "Blocked" || spot.status === "Maintenance" ? (
                  <div className="actions">
                    <button className="button button--ghost" type="button" onClick={() => void setSpotStatus(spot, "Blocked")}>Bloquear</button>
                    <button className="button button--ghost" type="button" onClick={() => void setSpotStatus(spot, "Maintenance")}>Manutenção</button>
                    <button className="button button--ghost" type="button" onClick={() => void setSpotStatus(spot, "Free")}>Liberar</button>
                  </div>
                ) : null}
              </article>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
