import { fireEvent, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { adminUser, jsonResponse, renderApp } from "./helpers";

describe("render_painel_listaEstacionadosEFiltroPlaca", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_painel_listaEstacionadosEFiltroPlaca", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input) => {
      const url = String(input);
      if (url.endsWith("/api/session")) {
        return jsonResponse(adminUser);
      }
      if (url.includes("/api/operations/dashboard")) {
        return jsonResponse({
          totalSpots: 2,
          occupied: 1,
          free: 1,
          blocked: 0,
          maintenance: 0,
          occupancyRate: 0.5,
          occupancyBySector: [],
          cashOpen: true,
          dayRevenue: 0,
          parked: [
            {
              id: "stay-1",
              plate: "ABC-1234",
              vehicleType: "Car",
              sectorCode: "B",
              sectorName: "Pátio B",
              spotCode: "B-001",
              tariffName: "Carro",
              firstHourAmount: 10,
              additionalHourAmount: 5,
              entryAt: new Date().toISOString(),
              status: "Active",
            },
          ],
        });
      }
      if (url.includes("/api/sectors")) {
        return jsonResponse([]);
      }
      if (url.includes("/api/spots")) {
        return jsonResponse([]);
      }
      return jsonResponse({}, 404);
    });

    renderApp("/operacao");
    expect(await screen.findByText("ABC-1234")).toBeInTheDocument();
    fireEvent.change(screen.getByLabelText("Buscar placa"), { target: { value: "ZZZ" } });
    expect(screen.getByText("Nenhum veículo estacionado.")).toBeInTheDocument();
  });
});
