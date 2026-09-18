import { screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { adminUser, jsonResponse, renderApp } from "./helpers";

describe("render_operacao_419px_acoesUtilizaveis", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_operacao_419px_acoesUtilizaveis", async () => {
    Object.defineProperty(window, "innerWidth", { configurable: true, value: 419 });
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input) => {
      const url = String(input);
      if (url.endsWith("/api/session")) {
        return jsonResponse(adminUser);
      }
      if (url.includes("/dashboard")) {
        return jsonResponse({
          totalSpots: 0,
          occupied: 0,
          free: 0,
          blocked: 0,
          maintenance: 0,
          occupancyRate: 0,
          occupancyBySector: [],
          cashOpen: false,
          dayRevenue: 0,
          parked: [],
        });
      }
      return jsonResponse([]);
    });

    renderApp("/operacao");
    expect(await screen.findByRole("button", { name: "Menu" })).toBeInTheDocument();
    expect(await screen.findByRole("button", { name: "Registrar entrada" })).toHaveClass("button");
    expect(screen.getByRole("button", { name: "Sugerir vaga" })).toHaveClass("button");
  });
});
