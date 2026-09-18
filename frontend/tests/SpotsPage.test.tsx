import { screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { jsonResponse, operatorUser, renderApp } from "./helpers";

describe("render_vagas_operadorNaoAcessa", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_vagas_operadorNaoAcessa", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue(jsonResponse(operatorUser));
    renderApp("/vagas");
    expect(await screen.findByText("Acesso recusado")).toBeInTheDocument();
    expect(screen.queryByRole("heading", { name: "Vagas" })).not.toBeInTheDocument();
  });
});
