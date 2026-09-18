import { fireEvent, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { jsonResponse, renderApp } from "./helpers";

describe("render_login_exibeCamposERecusaGenerica", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_login_exibeCamposERecusaGenerica", async () => {
    vi.spyOn(globalThis, "fetch").mockImplementation(async (_input, init) => {
      if ((init?.method ?? "GET") === "POST") {
        return jsonResponse({ code: "invalid_credentials", message: "Login ou senha inválidos." }, 401);
      }
      return jsonResponse({ message: "Sessão inválida." }, 401);
    });

    renderApp("/login");

    expect(await screen.findByLabelText("Login")).toBeInTheDocument();
    expect(screen.getByLabelText("Senha")).toBeInTheDocument();
    fireEvent.change(screen.getByLabelText("Login"), { target: { value: "admin" } });
    fireEvent.change(screen.getByLabelText("Senha"), { target: { value: "errada" } });
    fireEvent.click(screen.getByRole("button", { name: "Entrar" }));
    expect(await screen.findByText("Login ou senha inválidos.")).toBeInTheDocument();
  });
});
