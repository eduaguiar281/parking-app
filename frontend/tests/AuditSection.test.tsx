import { screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { jsonResponse, operatorUser, renderApp } from "./helpers";

describe("render_auditoria_operadorNaoAcessa", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("render_auditoria_operadorNaoAcessa", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue(jsonResponse(operatorUser));
    renderApp("/relatorios");
    expect(await screen.findByText("Acesso recusado")).toBeInTheDocument();
    expect(screen.queryByTestId("audit-section")).not.toBeInTheDocument();
  });
});
