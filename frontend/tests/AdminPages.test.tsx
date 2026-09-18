import { fireEvent, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { adminUser, jsonResponse, renderApp } from "./helpers";

describe("render_cadastrosAdmin_carregaListas", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_caixa_semAberto_mostraAbrir", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input) => {
      const url = String(input);
      if (url.endsWith("/api/session")) return jsonResponse(adminUser);
      if (url.includes("/api/cash/current")) return jsonResponse({ code: "no_open_cash", message: "Nenhum caixa aberto." }, 404);
      return jsonResponse([]);
    });
    renderApp("/caixa");
    expect(await screen.findByRole("heading", { name: "Caixa" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Abrir caixa" })).toBeInTheDocument();
  });

  it("render_tarifas_listaVazia_mostraEstado", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input) => {
      const url = String(input);
      if (url.endsWith("/api/session")) return jsonResponse(adminUser);
      return jsonResponse([]);
    });
    renderApp("/tarifas");
    expect(await screen.findByText("Nenhuma tarifa cadastrada.")).toBeInTheDocument();
  });

  it("render_usuarios_admin_mostraFormulario", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input) => {
      const url = String(input);
      if (url.endsWith("/api/session")) return jsonResponse(adminUser);
      if (url.includes("/api/users")) return jsonResponse([]);
      return jsonResponse([]);
    });
    renderApp("/usuarios");
    expect(await screen.findByRole("heading", { name: "Usuários" })).toBeInTheDocument();
    fireEvent.change(screen.getByLabelText("Nome"), { target: { value: "Op" } });
  });

  it("render_relatorios_admin_mostraAuditoria", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input) => {
      const url = String(input);
      if (url.endsWith("/api/session")) return jsonResponse(adminUser);
      if (url.includes("/api/reports/cash")) return jsonResponse({ items: [] });
      if (url.includes("/api/reports/stays")) return jsonResponse({ entryCount: 0, exitCount: 0, totalRevenue: 0 });
      if (url.includes("/api/audit")) {
        return jsonResponse([
          {
            id: "a1",
            occurredAt: "2026-09-18T12:00:00Z",
            userName: "Admin",
            action: "entry",
            entityType: "ParkingStay",
            entityId: "1",
            changedData: "{}",
          },
        ]);
      }
      return jsonResponse([]);
    });
    renderApp("/relatorios");
    expect(await screen.findByTestId("audit-section")).toBeInTheDocument();
    expect(await screen.findByText("entry")).toBeInTheDocument();
  });
});
