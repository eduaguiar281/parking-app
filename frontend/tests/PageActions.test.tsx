import { fireEvent, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { adminUser, jsonResponse, renderApp } from "./helpers";
import { download } from "../src/api/client";
import { dateOnly, dateTime } from "../src/api/format";

const spots = [
  { id: "s1", code: "P-A-001", sectorId: "sec1", vehicleCategory: "Motorcycle", status: "Free" },
];
const sectors = [{ id: "sec1", name: "Pátio A", code: "PA", allowedCategories: "Motorcycle", status: "Active" }];

describe("render_paginas_coberturaDeAcoes", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_vagas_admin_listaEBloqueia", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input, init) => {
      const url = String(input);
      if (url.endsWith("/api/session")) return jsonResponse(adminUser);
      if (url.includes("/api/sectors")) return jsonResponse(sectors);
      if (url.includes("/api/spots")) {
        if ((init?.method ?? "GET") === "PUT") return jsonResponse({ ...spots[0], status: "Blocked" });
        return jsonResponse(spots);
      }
      return jsonResponse([]);
    });
    renderApp("/vagas");
    expect(await screen.findByText("P-A-001")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Bloquear" }));
    expect(await screen.findByText("Status da vaga atualizado.")).toBeInTheDocument();
  });

  it("render_usuarios_listaEInativa", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input, init) => {
      const url = String(input);
      if (url.endsWith("/api/session")) return jsonResponse(adminUser);
      if (url.includes("/deactivate")) return { ok: true, status: 204, text: async () => "", headers: new Headers() } as Response;
      if (url.includes("/api/users")) {
        return jsonResponse([{ id: "u1", fullName: "Op", login: "op", role: "Operator", status: "Active" }]);
      }
      return jsonResponse([]);
    });
    renderApp("/usuarios");
    expect(await screen.findByText("op")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Inativar" }));
    expect(await screen.findByText("Usuário inativado.")).toBeInTheDocument();
  });

  it("render_tarifas_inativaTabela", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input) => {
      const url = String(input);
      if (url.endsWith("/api/session")) return jsonResponse(adminUser);
      if (url.includes("/api/tariffs")) {
        return jsonResponse([
          {
            id: "t1",
            name: "Carro geral",
            vehicleType: "Car",
            firstHourAmount: 10,
            additionalHourAmount: 5,
            effectiveFrom: "2026-01-01",
            status: "Active",
          },
        ]);
      }
      return jsonResponse([]);
    });
    renderApp("/tarifas");
    expect(await screen.findByText("Carro geral")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Inativar" }));
    expect(await screen.findByText("Tarifa inativada.")).toBeInTheDocument();
  });

  it("render_caixa_aberto_fechaComConfirmacao", async () => {
    vi.spyOn(window, "confirm").mockReturnValue(true);
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input, init) => {
      const url = String(input);
      if (url.endsWith("/api/session")) return jsonResponse(adminUser);
      if (url.includes("/close")) {
        return jsonResponse({
          id: "c1",
          operationalDate: "2026-09-18",
          status: "Closed",
          openingAmount: 100,
          expectedAmount: 100,
          difference: 0,
          totalsByPaymentMethod: {},
        });
      }
      if (url.includes("/api/cash/current")) {
        return jsonResponse({
          id: "c1",
          operationalDate: "2026-09-18",
          status: "Open",
          openingAmount: 100,
          expectedAmount: 100,
          totalsByPaymentMethod: {},
        });
      }
      return jsonResponse([]);
    });
    renderApp("/caixa");
    expect(await screen.findByText(/Caixa aberto/)).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Fechar caixa" }));
    expect(await screen.findByText("Caixa fechado.")).toBeInTheDocument();
  });

  it("download_arquivo_disparaLink", async () => {
    const click = vi.fn();
    vi.spyOn(document, "createElement").mockReturnValue({ click, download: "", href: "" } as unknown as HTMLAnchorElement);
    vi.stubGlobal("URL", { createObjectURL: () => "blob:1", revokeObjectURL: () => undefined });
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      blob: async () => new Blob(["a"]),
      headers: new Headers(),
    } as Response);
    await download("/api/reports/cash.pdf?from=2026-01-01&to=2026-12-31", "caixa.pdf");
    expect(click).toHaveBeenCalled();
  });

  it("format_dataVazia_retornaTraco", () => {
    expect(dateOnly(undefined)).toBe("—");
    expect(dateTime(undefined)).toBe("—");
    expect(dateOnly("2026-09-18")).toBe("18/09/2026");
  });
});
