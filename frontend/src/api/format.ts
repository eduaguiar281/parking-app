const br = new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });

export function money(value: number | null | undefined) {
  return br.format(value ?? 0);
}

export function dateTime(value: string | null | undefined) {
  if (!value) {
    return "—";
  }
  return new Date(value).toLocaleString("pt-BR", { timeZone: "America/Sao_Paulo" });
}

export function dateOnly(value: string | null | undefined) {
  if (!value) {
    return "—";
  }
  const [year, month, day] = value.split("-");
  return `${day}/${month}/${year}`;
}

export function todayIso() {
  return new Date().toLocaleDateString("en-CA", { timeZone: "America/Sao_Paulo" });
}

export function durationFrom(entryAt: string, now = Date.now()) {
  const minutes = Math.max(0, Math.ceil((now - new Date(entryAt).getTime()) / 60000));
  const hours = Math.floor(minutes / 60);
  const rest = minutes % 60;
  return hours > 0 ? `${hours}h ${rest}min` : `${rest}min`;
}

export const vehicleLabel: Record<string, string> = {
  Car: "Carro",
  Motorcycle: "Moto",
  Both: "Carros e motos",
};

export const statusLabel: Record<string, string> = {
  Free: "Livre",
  Occupied: "Ocupada",
  Blocked: "Bloqueada",
  Maintenance: "Manutenção",
  Active: "Ativa",
  Inactive: "Inativa",
  Completed: "Concluída",
  Cancelled: "Cancelada",
  Open: "Aberto",
  Closed: "Fechado",
  Administrator: "Administrador",
  Operator: "Operador",
};

export const paymentLabel: Record<string, string> = {
  Cash: "Dinheiro",
  Pix: "Pix",
  DebitCard: "Débito",
  CreditCard: "Crédito",
  Other: "Outro",
};
