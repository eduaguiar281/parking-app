import { render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import App from "../src/App";

describe("render_vitrineInicial_exibeTituloCardBotaoECampo", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_vitrineInicial_exibeTituloCardBotaoECampo", () => {
    const fetchSpy = vi.spyOn(globalThis, "fetch");

    render(<App />);

    expect(screen.getByRole("heading", { level: 1 })).toHaveTextContent(
      "Parking App",
    );
    expect(screen.getByRole("article")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Confirmar" })).toBeInTheDocument();
    expect(screen.getByRole("textbox", { name: "Nome do arauto" })).toBeInTheDocument();
    expect(fetchSpy).not.toHaveBeenCalled();
  });
});
