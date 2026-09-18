import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { api } from "../api/client";

export type SessionUser = {
  id: string;
  fullName: string;
  login: string;
  role: "Administrator" | "Operator";
};

type AuthState = {
  user: SessionUser | null;
  loading: boolean;
  login: (login: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<SessionUser | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api<SessionUser>("/api/session")
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setLoading(false));
  }, []);

  const value = useMemo<AuthState>(
    () => ({
      user,
      loading,
      login: async (loginName, password) => {
        const next = await api<SessionUser>("/api/session", {
          method: "POST",
          body: JSON.stringify({ login: loginName, password }),
        });
        setUser(next);
      },
      logout: async () => {
        await api("/api/session", { method: "DELETE" });
        setUser(null);
      },
    }),
    [user, loading],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth precisa do AuthProvider");
  }
  return ctx;
}
