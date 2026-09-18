export class ApiError extends Error {
  status: number;
  code: string;

  constructor(status: number, code: string, message: string) {
    super(message);
    this.status = status;
    this.code = code;
  }
}

export async function api<T>(path: string, init?: RequestInit): Promise<T> {
  const headers = new Headers(init?.headers);
  if (init?.body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  const response = await fetch(path, {
    credentials: "include",
    ...init,
    headers,
  });

  if (response.status === 204) {
    return undefined as T;
  }

  const text = await response.text();
  const data = text ? (JSON.parse(text) as { code?: string; message?: string }) : null;
  if (!response.ok) {
    throw new ApiError(
      response.status,
      data?.code ?? "error",
      data?.message ?? "Falha na requisição.",
    );
  }

  return data as T;
}

export async function download(path: string, filename: string) {
  const response = await fetch(path, { credentials: "include" });
  if (!response.ok) {
    throw new ApiError(response.status, "download", "Não foi possível baixar o arquivo.");
  }
  const blob = await response.blob();
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = filename;
  link.click();
  URL.revokeObjectURL(url);
}
