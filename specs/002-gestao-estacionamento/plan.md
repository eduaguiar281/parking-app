# Implementation Plan: Gestão de estacionamento

**Branch**: `002-gestao-estacionamento` | **Date**: 2026-09-18 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/002-gestao-estacionamento/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Entregar o produto de gestão de um estacionamento sobre a fundação já existente: autenticação por cookie de sessão (Administrador e Operador, até logout ou fechar o app), setores e vagas, tarifas parametrizáveis, entrada/saída com cobrança de hora cheia, um caixa aberto por vez (qualquer perfil operacional fecha), histórico, relatórios PDF/CSV, auditoria e UI pt-BR (R$, datas brasileiras) no visual do DESIGN.md.

Backend no mesmo `ParkingApp.Api`: Domain sem I/O, Application orquestra, Infrastructure EF Core + SQLite. Frontend React deixa a vitrine e passa a ser o app autenticado, com proxy Vite `/api` → `:5080`.

Detalhes em [research.md](./research.md).

## Technical Context

**Language/Version**: C# / SDK .NET 10; TypeScript + React 19.3; Node.js 20.19+ ou 22 LTS

**Primary Dependencies**: ASP.NET Core Minimal APIs; EF Core + SQLite; cookie auth + `PasswordHasher<User>`; QuestPDF; xUnit; WebApplicationFactory; Coverlet; Vite; React Router; Vitest; Testing Library

**Storage**: SQLite (`backend/parking.db`, gitignored). Instante em UTC; UI em `America/Sao_Paulo`

**Testing**: `dotnet test` em `backend/` (Domain sem I/O; API com factory + SQLite de teste). `npm test` em `frontend/`. Cobertura ≥ 80% de linhas em cada README. Nomes `<método>_<cenário>_<resultado esperado>` em pt-BR

**Target Platform**: Desenvolvimento local (HTTP). UI desktop, tablet e celular no browser. Sem Docker/CI nesta feature

**Project Type**: Monorepo web (serviço + SPA)

**Performance Goals**: Entrada válida < 45 s para operador treinado (SC-001); painel coerente após mutação (SC-002); fechamento de caixa < 2 min com lançamentos prontos (SC-007)

**Constraints**: YAGNI > KISS > DRY > SOLID. Um csproj. Sem `IRepository<T>`, SignalR, Identity UI, segundo banco, Pix/adquirente reais, timeout por ociosidade ou troca de vaga com estadia ativa. Vitrine da fundação deixa de ser a home; `GET /alive` permanece. Código em inglês; docs/UI/nomes de teste em pt-BR

**Scale/Scope**: Um estabelecimento; 7 telas autenticadas + login; 8 entidades persistidas; dois perfis; relatórios de caixa e de estadias

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Status | Como o plano atende |
|-----------|--------|---------------------|
| I. YAGNI | Pass | Só módulos da spec. Sem mensalista, WebSocket, Identity completo, segundo DB, mover vaga |
| II. KISS | Pass | Minimal APIs, DbContext concreto, cookie de sessão + proxy, CSV manual |
| III. DRY | Pass | Preço, placa, esperado do caixa e ocupação em Domain; snapshots na estadia; DESIGN.md como fonte visual |
| IV. Clean Code | Pass | Identificadores em inglês; casos de uso nomeados; vitrine deixa de ser a home |
| V. Ortogonalidade | Pass | Domain testável sem I/O; EF só em Infrastructure; UI não calcula tarifa; pastas D/A/I com comportamento nesta spec |
| VI. SOLID | Pass | Sem interface de repositório para uma implementação; permissão por perfil onde a spec já distingue dois atores |
| VII. Testes | Pass | 80% nos READMEs; Domain cobre SC-003 e normalização de placa; API cobre 409 de vaga/placa/caixa/último admin; UI cobre bloqueio de rota e logout |
| Idioma | Pass | READMEs já declaram inglês no código; UI e artefatos Spec Kit em pt-BR |
| Fluxo Git | Pass | Branch curto; fatia desta spec no mesmo PR (backend + frontend + specs) |
| Precedência | Pass | QuestPDF só porque FR-053 pede PDF; não há UoW/MediatR |

**Pós-Fase 1:** [data-model.md](./data-model.md) e contratos não introduzem entidade extra (comprovante e relatório são projeções). Gate de Ortogonalidade permanece Pass: regra de preço/placa/caixa esperado/último admin sem disco; persistência no contexto EF; nenhuma pasta D/A/I vazia. Tabela de complexidade vazia.

## Project Structure

### Documentation (this feature)

```text
specs/002-gestao-estacionamento/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── http-api.yaml
│   └── ui.md
└── tasks.md
```

### Source Code (repository root)

```text
backend/
├── README.md
├── ParkingApp.slnx
├── src/
│   ├── ParkingApp.Api.csproj
│   ├── Program.cs                    # cookie, EF, MapGroup /api, /alive
│   ├── Domain/                       # placa, preço, tarifa, ocupação, esperado, último admin
│   ├── Application/                  # casos de uso + ParkingDbContext
│   └── Infrastructure/               # DbContext, configurações EF, PDF, hasher
└── tests/
    ├── ParkingApp.Api.Tests.csproj
    ├── Domain/                       # sem I/O
    ├── Application/
    └── Api/                          # WebApplicationFactory
frontend/
├── README.md
├── DESIGN.md
├── package.json
├── vite.config.ts                    # proxy /api → :5080
├── src/
│   ├── main.tsx
│   ├── App.tsx                       # rotas e shell; não é mais a vitrine
│   ├── api/
│   ├── pages/
│   ├── components/
│   └── styles/
│       ├── tokens.css
│       └── app.css
└── tests/
```

**Structure Decision:** Continua o monorepo `backend/` + `frontend/`. A fatia de negócio entra no csproj existente, em pastas Domain/Application/Infrastructure com código desta spec (não PR de pastas vazias). A UI ganha rotas e deixa a vitrine; tokens do DESIGN.md permanecem. `GET /alive` não sai.

## Complexity Tracking

> Nenhuma violação da constitution. Sem linhas nesta tabela.
