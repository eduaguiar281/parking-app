import { useEffect, useState } from "react";
import { api, ApiError } from "../api/client";
import { dateTime, money, paymentLabel, vehicleLabel } from "../api/format";

export type Stay = {
  id: string;
  plate: string;
  vehicleType: string;
  sectorCode: string;
  sectorName: string;
  spotCode: string;
  tariffName: string;
  firstHourAmount: number;
  additionalHourAmount: number;
  entryAt: string;
  exitAt?: string | null;
  durationMinutes?: number | null;
  amountCharged?: number | null;
  paymentMethod?: string | null;
  status: string;
};

type Preview = Stay & {
  proposedExitAt: string;
  proposedDurationMinutes: number;
  proposedAmount: number;
};

export function ExitDialog({ stayId, onClose }: { stayId: string; onClose: (done: boolean) => void }) {
  const [preview, setPreview] = useState<Preview | null>(null);
  const [paymentMethod, setPaymentMethod] = useState("Pix");
  const [error, setError] = useState("");
  const [receipt, setReceipt] = useState<Stay | null>(null);

  useEffect(() => {
    api<Preview>(`/api/stays/${stayId}/exit-preview`)
      .then(setPreview)
      .catch((err: unknown) => setError(err instanceof ApiError ? err.message : "Falha ao carregar saída."));
  }, [stayId]);

  async function confirm() {
    if (!window.confirm("Confirmar saída e pagamento desta estadia?")) {
      return;
    }
    try {
      const paid = await api<Stay>(`/api/stays/${stayId}/exit`, {
        method: "POST",
        body: JSON.stringify({ paymentMethod }),
      });
      setReceipt(paid);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Não foi possível registrar a saída.");
    }
  }

  const data = receipt ?? preview;

  return (
    <div className="dialog" role="dialog" aria-labelledby="exit-title">
      <article className="card">
        <h2 id="exit-title">{receipt ? "Comprovante" : "Registrar saída"}</h2>
        {error ? <p className="banner banner--error">{error}</p> : null}
        {data ? (
          <dl className="summary">
            <div>
              <dt>Placa</dt>
              <dd>{data.plate}</dd>
            </div>
            <div>
              <dt>Tipo</dt>
              <dd>{vehicleLabel[data.vehicleType] ?? data.vehicleType}</dd>
            </div>
            <div>
              <dt>Setor / vaga</dt>
              <dd>
                {data.sectorName} · {data.spotCode}
              </dd>
            </div>
            <div>
              <dt>Tarifa</dt>
              <dd>
                {data.tariffName} ({money(data.firstHourAmount)} / {money(data.additionalHourAmount)})
              </dd>
            </div>
            <div>
              <dt>Entrada</dt>
              <dd>{dateTime(data.entryAt)}</dd>
            </div>
            <div>
              <dt>Saída</dt>
              <dd>{dateTime(receipt?.exitAt ?? preview?.proposedExitAt)}</dd>
            </div>
            <div>
              <dt>Permanência</dt>
              <dd>{receipt?.durationMinutes ?? preview?.proposedDurationMinutes} min</dd>
            </div>
            <div>
              <dt>Valor</dt>
              <dd>{money(receipt?.amountCharged ?? preview?.proposedAmount ?? 0)}</dd>
            </div>
          </dl>
        ) : (
          <p>Carregando resumo…</p>
        )}
        {!receipt ? (
          <>
            <label className="field">
              <span className="field__label">Forma de pagamento</span>
              <select className="field__input" value={paymentMethod} onChange={(e) => setPaymentMethod(e.target.value)}>
                {Object.entries(paymentLabel).map(([value, label]) => (
                  <option key={value} value={value}>
                    {label}
                  </option>
                ))}
              </select>
            </label>
            <div className="actions">
              <button className="button" type="button" onClick={() => void confirm()}>
                Confirmar pagamento
              </button>
              <button className="button button--ghost" type="button" onClick={() => onClose(false)}>
                Cancelar
              </button>
            </div>
          </>
        ) : (
          <div className="actions">
            <button className="button" type="button" onClick={() => window.print()}>
              Imprimir comprovante
            </button>
            <button className="button button--ghost" type="button" onClick={() => onClose(true)}>
              Fechar
            </button>
          </div>
        )}
      </article>
    </div>
  );
}
