import { FormEvent, useEffect, useMemo, useState } from "react";
import { api, ApiError } from "../api/client";
import { durationFrom, money, vehicleLabel } from "../api/format";
import { useAuth } from "../auth/AuthContext";
import { ExitDialog, type Stay } from "../components/ExitDialog";

type Dashboard = {
  totalSpots: number;
  occupied: number;
  free: number;
  blocked: number;
  maintenance: number;
  occupancyRate: number;
  occupancyBySector: { sectorCode: string; occupancyRate: number }[];
  cashOpen: boolean;
  cashOperationalDate?: string | null;
  dayRevenue: number;
  parked: Stay[];
};

type Sector = { id: string; name: string; code: string; allowedCategories: string; status: string };
type Spot = { id: string; code: string; sectorId: string; vehicleCategory: string; status: string };

export function OperationPage() {
  const { user } = useAuth();
  const [dash, setDash] = useState<Dashboard | null>(null);
  const [sectors, setSectors] = useState<Sector[]>([]);
  const [spots, setSpots] = useState<Spot[]>([]);
  const [plate, setPlate] = useState("");
  const [vehicleType, setVehicleType] = useState("Car");
  const [sectorId, setSectorId] = useState("");
  const [spotId, setSpotId] = useState("");
  const [filter, setFilter] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [exitId, setExitId] = useState<string | null>(null);
  const [now, setNow] = useState(Date.now());

  async function load() {
    const [nextDash, nextSectors, nextSpots] = await Promise.all([
      api<Dashboard>("/api/operations/dashboard"),
      api<Sector[]>("/api/sectors"),
      api<Spot[]>("/api/spots"),
    ]);
    setDash(nextDash);
    setSectors(nextSectors);
    setSpots(nextSpots);
  }

  useEffect(() => {
    void load().catch((err: unknown) => setError(err instanceof ApiError ? err.message : "Falha ao carregar o painel."));
    const timer = window.setInterval(() => setNow(Date.now()), 30000);
    return () => window.clearInterval(timer);
  }, []);

  const compatibleSectors = sectors.filter(
    (s) => s.status === "Active" && (s.allowedCategories === "Both" || s.allowedCategories === vehicleType),
  );
  const compatibleSpots = spots.filter(
    (s) =>
      s.status === "Free" &&
      s.vehicleCategory === vehicleType &&
      (!sectorId || s.sectorId === sectorId),
  );

  const parked = useMemo(() => {
    const list = dash?.parked ?? [];
    const q = filter.trim().toUpperCase();
    return q ? list.filter((s) => s.plate.includes(q)) : list;
  }, [dash, filter]);

  async function suggest() {
    try {
      const spot = await api<Spot>(
        `/api/spots/suggest?vehicleType=${vehicleType}${sectorId ? `&sectorId=${sectorId}` : ""}`,
      );
      setSectorId(spot.sectorId);
      setSpotId(spot.id);
      setMessage(`Vaga sugerida: ${spot.code}`);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não há vaga compatível.");
    }
  }

  async function enter(event: FormEvent) {
    event.preventDefault();
    setError("");
    setMessage("");
    try {
      await api("/api/stays", {
        method: "POST",
        body: JSON.stringify({ plate, vehicleType, sectorId, spotId }),
      });
      setPlate("");
      setMessage("Entrada registrada.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível registrar a entrada.");
    }
  }

  async function cancelStay(id: string) {
    const reason = window.prompt("Motivo do cancelamento da estadia ativa:");
    if (!reason) {
      return;
    }
    if (!window.confirm("Cancelar esta estadia ativa? A vaga será liberada sem pagamento.")) {
      return;
    }
    try {
      await api(`/api/stays/${id}/cancel`, { method: "POST", body: JSON.stringify({ reason }) });
      setMessage("Estadia cancelada.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível cancelar.");
    }
  }

  return (
    <div className="stack">
      <h1>Operação</h1>
      {error ? <p className="banner banner--error">{error}</p> : null}
      {message ? <p className="banner banner--ok">{message}</p> : null}
      {dash ? (
        <section className="metrics">
          <article className="metric">
            <span>Cadastradas</span>
            <strong>{dash.totalSpots}</strong>
          </article>
          <article className="metric">
            <span>Ocupadas</span>
            <strong>{dash.occupied}</strong>
          </article>
          <article className="metric">
            <span>Livres</span>
            <strong>{dash.free}</strong>
          </article>
          <article className="metric">
            <span>Bloqueadas</span>
            <strong>{dash.blocked}</strong>
          </article>
          <article className="metric">
            <span>Manutenção</span>
            <strong>{dash.maintenance}</strong>
          </article>
          <article className="metric">
            <span>Ocupação</span>
            <strong>{Math.round(dash.occupancyRate * 100)}%</strong>
          </article>
          <article className="metric">
            <span>Caixa</span>
            <strong>{dash.cashOpen ? "Aberto" : "Fechado"}</strong>
          </article>
          <article className="metric">
            <span>Receita do dia</span>
            <strong>{money(dash.dayRevenue)}</strong>
          </article>
        </section>
      ) : (
        <p>Carregando painel…</p>
      )}

      <form className="card" onSubmit={(e) => void enter(e)}>
        <h2>Registrar entrada</h2>
        <div className="form-grid">
          <label className="field">
            <span className="field__label">Placa</span>
            <input className="field__input" value={plate} onChange={(e) => setPlate(e.target.value)} placeholder="ABC-1234" />
          </label>
          <label className="field">
            <span className="field__label">Tipo</span>
            <select className="field__input" value={vehicleType} onChange={(e) => { setVehicleType(e.target.value); setSectorId(""); setSpotId(""); }}>
              <option value="Car">Carro</option>
              <option value="Motorcycle">Moto</option>
            </select>
          </label>
          <label className="field">
            <span className="field__label">Setor</span>
            <select className="field__input" value={sectorId} onChange={(e) => { setSectorId(e.target.value); setSpotId(""); }}>
              <option value="">Selecione</option>
              {compatibleSectors.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.code} — {s.name}
                </option>
              ))}
            </select>
          </label>
          <label className="field">
            <span className="field__label">Vaga</span>
            <select className="field__input" value={spotId} onChange={(e) => setSpotId(e.target.value)}>
              <option value="">Selecione</option>
              {compatibleSpots.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.code}
                </option>
              ))}
            </select>
          </label>
        </div>
        <div className="actions">
          <button className="button" type="submit">
            Registrar entrada
          </button>
          <button className="button button--ghost" type="button" onClick={() => void suggest()}>
            Sugerir vaga
          </button>
        </div>
      </form>

      <section className="card">
        <h2>Estacionados</h2>
        <label className="field">
          <span className="field__label">Buscar placa</span>
          <input className="field__input" value={filter} onChange={(e) => setFilter(e.target.value)} placeholder="ABC-1234" />
        </label>
        {parked.length === 0 ? (
          <p>Nenhum veículo estacionado.</p>
        ) : (
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>Placa</th>
                  <th>Tipo</th>
                  <th>Setor</th>
                  <th>Vaga</th>
                  <th>Permanência</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {parked.map((stay) => (
                  <tr key={stay.id}>
                    <td>{stay.plate}</td>
                    <td>{vehicleLabel[stay.vehicleType] ?? stay.vehicleType}</td>
                    <td>{stay.sectorCode}</td>
                    <td>{stay.spotCode}</td>
                    <td>{durationFrom(stay.entryAt, now)}</td>
                    <td>
                      <button className="button" type="button" onClick={() => setExitId(stay.id)}>
                        Registrar saída
                      </button>
                      {user?.role === "Administrator" ? (
                        <button className="button button--ghost" type="button" onClick={() => void cancelStay(stay.id)}>
                          Cancelar estadia
                        </button>
                      ) : null}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
      {exitId ? <ExitDialog stayId={exitId} onClose={() => { setExitId(null); void load(); }} /> : null}
    </div>
  );
}
