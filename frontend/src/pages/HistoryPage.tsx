import { FormEvent, useEffect, useState } from "react";
import { api, ApiError } from "../api/client";
import { dateTime, money, paymentLabel, vehicleLabel } from "../api/format";
import type { Stay } from "../components/ExitDialog";

export function HistoryPage() {
  const [items, setItems] = useState<Stay[]>([]);
  const [plate, setPlate] = useState("");
  const [error, setError] = useState("");

  async function search(event?: FormEvent) {
    event?.preventDefault();
    const query = plate ? `?plate=${encodeURIComponent(plate)}` : "";
    setItems(await api<Stay[]>(`/api/stays${query}`));
  }

  useEffect(() => {
    void search().catch((err: unknown) => setError(err instanceof ApiError ? err.message : "Falha ao carregar o histórico."));
  }, []);

  return (
    <div className="stack">
      <h1>Histórico</h1>
      {error ? <p className="banner banner--error">{error}</p> : null}
      <form className="card" onSubmit={(e) => void search(e)}>
        <label className="field">
          <span className="field__label">Placa</span>
          <input className="field__input" value={plate} onChange={(e) => setPlate(e.target.value)} placeholder="ABC-1234" />
        </label>
        <button className="button" type="submit">Filtrar</button>
      </form>
      <section className="card">
        {items.length === 0 ? (
          <p>Nenhuma estadia encontrada para os filtros informados.</p>
        ) : (
          <table className="table">
            <thead>
              <tr>
                <th>Placa</th>
                <th>Tipo</th>
                <th>Entrada</th>
                <th>Saída</th>
                <th>Valor</th>
                <th>Pagamento</th>
              </tr>
            </thead>
            <tbody>
              {items.map((stay) => (
                <tr key={stay.id}>
                  <td>{stay.plate}</td>
                  <td>{vehicleLabel[stay.vehicleType] ?? stay.vehicleType}</td>
                  <td>{dateTime(stay.entryAt)}</td>
                  <td>{dateTime(stay.exitAt)}</td>
                  <td>{money(stay.amountCharged)}</td>
                  <td>{stay.paymentMethod ? paymentLabel[stay.paymentMethod] ?? stay.paymentMethod : "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
    </div>
  );
}
