import { screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { jsonResponse, operatorUser, renderApp } from "./helpers";

describe("render_usuarios_operadorRecebeRecusa", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_usuarios_operadorRecebeRecusa", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue(jsonResponse(operatorUser));
    renderApp("/usuarios");
    expect(await screen.findByText("Acesso recusado")).toBeInTheDocument();
    expect(screen.queryByRole("heading", { name: "Usuários" })).not.toBeInTheDocument();
  });
});
