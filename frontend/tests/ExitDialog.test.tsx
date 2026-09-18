import { fireEvent, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { ExitDialog } from "../src/components/ExitDialog";
import { api } from "../src/api/client";
import { render } from "@testing-library/react";
import { jsonResponse } from "./helpers";

describe("client_e_saida", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("api_resposta204_retornaUndefined", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 204,
      text: async () => "",
      headers: new Headers(),
    } as Response);
    await expect(api("/api/session", { method: "DELETE" })).resolves.toBeUndefined();
  });

  it("render_saida_exibeResumoEConfirma", async () => {
    vi.spyOn(window, "confirm").mockReturnValue(true);
    vi.spyOn(globalThis, "fetch").mockImplementation(async (input, init) => {
      if ((init?.method ?? "GET") === "POST") {
        return jsonResponse({
          id: "s1",
          plate: "ABC-1234",
          vehicleType: "Car",
          sectorCode: "B",
          sectorName: "Pátio B",
          spotCode: "B-001",
          tariffName: "Carro",
          firstHourAmount: 10,
          additionalHourAmount: 5,
          entryAt: "2026-09-18T12:00:00Z",
          exitAt: "2026-09-18T12:20:00Z",
          durationMinutes: 20,
          amountCharged: 10,
          paymentMethod: "Pix",
          status: "Completed",
        });
      }
      return jsonResponse({
        id: "s1",
        plate: "ABC-1234",
        vehicleType: "Car",
        sectorCode: "B",
        sectorName: "Pátio B",
        spotCode: "B-001",
        tariffName: "Carro",
        firstHourAmount: 10,
        additionalHourAmount: 5,
        entryAt: "2026-09-18T12:00:00Z",
        proposedExitAt: "2026-09-18T12:20:00Z",
        proposedDurationMinutes: 20,
        proposedAmount: 10,
        status: "Active",
      });
    });

    render(<ExitDialog stayId="s1" onClose={() => undefined} />);
    expect(await screen.findByText("Registrar saída")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Confirmar pagamento" }));
    expect(await screen.findByText("Comprovante")).toBeInTheDocument();
  });
});
