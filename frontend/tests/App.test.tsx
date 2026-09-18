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

    expect(screen.getByRole("complementary", { name: "Menu" })).toBeInTheDocument();
    expect(screen.getByRole("heading", { level: 1 })).toHaveTextContent(
      "Vitrine",
    );
    expect(screen.getByRole("button", { name: "Vitrine" })).toHaveAttribute(
      "aria-current",
      "page",
    );
    expect(screen.getByRole("article")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Saiba mais" })).toBeInTheDocument();
    expect(screen.getByRole("textbox", { name: "Pesquisar" })).toBeInTheDocument();
    expect(fetchSpy).not.toHaveBeenCalled();
  });
});
