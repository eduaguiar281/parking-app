import { fireEvent, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { adminUser, jsonResponse, renderApp } from "./helpers";

describe("render_historico_filtraPorPlaca", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_historico_filtraPorPlaca", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input) => {
      const url = String(input);
      if (url.endsWith("/api/session")) {
        return jsonResponse(adminUser);
      }
      if (url.includes("/api/stays?plate=ABC-1234")) {
        return jsonResponse([
          {
            id: "1",
            plate: "ABC-1234",
            vehicleType: "Car",
            sectorCode: "B",
            sectorName: "Pátio B",
            spotCode: "B-001",
            tariffName: "Carro",
            firstHourAmount: 10,
            additionalHourAmount: 5,
            entryAt: "2026-09-18T12:00:00Z",
            exitAt: "2026-09-18T13:00:00Z",
            amountCharged: 10,
            paymentMethod: "Pix",
            status: "Completed",
          },
        ]);
      }
      if (url.includes("/api/stays")) {
        return jsonResponse([]);
      }
      return jsonResponse({}, 404);
    });

    renderApp("/historico");
    expect(await screen.findByText("Nenhuma estadia encontrada para os filtros informados.")).toBeInTheDocument();
    fireEvent.change(screen.getByLabelText("Placa"), { target: { value: "ABC-1234" } });
    fireEvent.click(screen.getByRole("button", { name: "Filtrar" }));
    expect(await screen.findByText("ABC-1234")).toBeInTheDocument();
  });
});
