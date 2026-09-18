# Pesquisa: Fundações do monorepo

## 1. Forma do serviço (sinal de vida)

**Decision:** ASP.NET Core Minimal APIs no SDK .NET 10, um único `GET /alive` sem autenticação, corpo `text/plain` `estou vivo`, status 200.

**Rationale:** A spec exige um endereço que responde que o serviço está vivo, sem regra de estacionamento. Minimal APIs é o caminho mais curto no .NET 10 ([teste de Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-10.0)). `text/plain` evita JSON, OpenAPI e DTOs que a spec não pediu (YAGNI / KISS).

**Alternatives considered:**
- Console + mensagem no terminal — rejeitado na clarify.
- Controllers + Swagger — camada e UI de contrato sem consumidor.
- JSON `{ "status": "alive" }` — mais estrutura sem ganho nesta feature.
- Health Checks (`AddHealthChecks`) — pacote extra para um texto fixo.

## 2. Testes do backend

**Decision:** xUnit + `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`) + Coverlet. Comando único: `dotnet test` na pasta `backend/`. Limiar 80% de linhas, declarado no README.

**Rationale:** É o padrão documentado para ASP.NET Core 10 ([testes de integração](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0)). Um teste de contrato contra `GET /alive` cobre o único comportamento. Nome: `GetAlive_servicoEmExecucao_retornaEstouVivo`.

**Alternatives considered:**
- NUnit / MSTest — equivalentes; xUnit é o default dos templates.
- Só teste de unidade no delegate — não prova o endereço HTTP que o colega abre.

## 3. Frontend (React 19.3)

**Decision:** Vite (`react-ts`) + React 19.3 + TypeScript. Node.js 20.19+ ou 22 LTS. Testes com Vitest + Testing Library + jsdom, arquivos em `frontend/tests/`. Cobertura 80% de linhas via `@vitest/coverage-v8`.

**Rationale:** Vite é o scaffold oficial ([guia Vite](https://vite.dev/guide/)). `react-ts` é o template atual; TypeScript entra como padrão da ferramenta, não como camada extra. Vitest reutiliza o mesmo `vite.config`. React 19.3 é o Latest da spec ([anúncio](https://react.dev/blog/2026/09/09/react-19-3)).

**Alternatives considered:**
- CRA — descontinuado.
- Next.js — roteamento e SSR que a vitrine não precisa.
- Jest — segunda toolchain ao lado do Vite.
- JSX sem TypeScript — foge do template oficial e gera retrabalho.

## 4. Aparência (DESIGN.md)

**Decision:** `frontend/DESIGN.md` é a fonte humana. `frontend/src/styles/tokens.css` copia os tokens (cores, tipo, espaço, raio, sombra). A vitrine vive em `App.tsx` com HTML semântico (`h1`, `article`, `button`, `input`) e classes CSS. Sem biblioteca de componentes, sem Tailwind. Fonte: Inter (Google Fonts) com stack `system-ui` / `-apple-system`.

**Rationale:** A spec pede quatro elementos, não um design system implementado. O visual vigente é dark-only com acento violeta; Tailwind/Radix/Next trariam stack que a vitrine não precisa (YAGNI / KISS). Gerar CSS a partir do markdown seria automação sem pedido. Duplicar hex no DESIGN.md e no CSS é similaridade acidental aceitável (constitution DRY).

**Alternatives considered:**
- Tailwind / shadcn / Radix — dependências e classes utilitárias para quatro elementos.
- Next.js — roteamento e SSR que a vitrine não precisa.
- Quatro arquivos de componente + Storybook — catálogo que a spec proibiu.
- CSS-in-JS — dependência extra.

Fontes: Inter via Google Fonts no `index.html`, com stack `system-ui, -apple-system, BlinkMacSystemFont`.

## 5. Monorepo e pastas

**Decision:** Só `backend/` e `frontend/` na raiz. Sem Nx, Turborepo, npm workspaces ou `packages/`. `backend/ParkingApp.sln` referencia `src` e `tests`. Sem camadas Domain/Application/Infrastructure.

**Rationale:** Spec e YAGNI. A solution só agrupa `dotnet test` / `dotnet run`.

**Alternatives considered:**
- `apps/` + `packages/` — pasta extra proibida.
- Um repo por app — contradiz o monorepo pedido.

## 6. Acoplamento frontend ↔ backend

**Decision:** Nenhum fetch, proxy Vite ou CORS nesta feature. Portas locais fixas só para o README: API `http://localhost:5080/alive`, UI `http://localhost:5173`.

**Rationale:** Clarify B. Independência das histórias P1.

## 7. Cobertura e idioma

**Decision:** 80% de linhas em cada README. Identificadores de código em inglês (regra local nos READMEs). Docs, UI e nomes de teste em pt-BR.

**Rationale:** Assunção da spec + constitution.

## 8. Fora desta feature

CI, Docker, persistência, autenticação, OpenAPI UI, biblioteca compartilhada, app nativo.
