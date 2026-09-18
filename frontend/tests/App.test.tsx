import { screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import App from "../src/App";
import { jsonResponse } from "./helpers";
import { render } from "@testing-library/react";

describe("render_app_naoAutenticado_mostraLogin", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_app_naoAutenticado_mostraLogin", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue(jsonResponse({ message: "Sessão inválida." }, 401));
    render(<App />);
    expect(await screen.findByRole("heading", { name: "Entrar" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Entrar" })).toBeInTheDocument();
  });
});
