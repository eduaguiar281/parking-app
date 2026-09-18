import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AppRoutes } from "../src/App";
import { AuthProvider } from "../src/auth/AuthContext";

export function jsonResponse(data: unknown, status = 200) {
  return {
    ok: status >= 200 && status < 400,
    status,
    text: async () => (data === undefined ? "" : JSON.stringify(data)),
    blob: async () => new Blob([JSON.stringify(data)]),
    headers: new Headers({ "Content-Type": "application/json" }),
  } as Response;
}

export function renderApp(path: string) {
  return render(
    <MemoryRouter initialEntries={[path]}>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </MemoryRouter>,
  );
}

export const adminUser = {
  id: "11111111-1111-1111-1111-111111111111",
  fullName: "Admin",
  login: "admin",
  role: "Administrator",
};

export const operatorUser = {
  id: "22222222-2222-2222-2222-222222222222",
  fullName: "Operador",
  login: "op",
  role: "Operator",
};
