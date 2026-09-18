# Implementation Plan: Fundações do monorepo

**Branch**: `001-fundacoes-monorepo` | **Date**: 2026-09-15 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-fundacoes-monorepo/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Criar a fundação do Parking App como monorepo de dois projetos irmãos: serviço .NET 10 com um `GET /alive` (`estou vivo`) e interface React 19.3 (Vite) com vitrine do DESIGN.md (título, card, botão, campo), sem os dois se falarem. Cada lado tem README em pt-BR, testes em `tests/` com 80% de linhas e nomes `<método>_<cenário>_<resultado esperado>`. Código em inglês.

Detalhes em [research.md](./research.md).

## Technical Context

**Language/Version**: C# / SDK .NET 10; TypeScript + React 19.3; Node.js 20.19+ ou 22 LTS

**Primary Dependencies**: ASP.NET Core Minimal APIs; xUnit; Microsoft.AspNetCore.Mvc.Testing; Coverlet; Vite; Vitest; Testing Library; CSS nativo (tokens do DESIGN.md)

**Storage**: N/A (nada persistido)

**Testing**: `dotnet test` em `backend/`; `npm test` em `frontend/` (Vitest). Cobertura ≥ 80% de linhas em cada README

**Target Platform**: Desenvolvimento local (HTTP). Sem Docker/CI nesta feature

**Project Type**: Monorepo web (serviço + SPA), dois projetos na raiz

**Performance Goals**: `GET /alive` responde em < 3 s em localhost (SC-006)

**Constraints**: YAGNI > KISS > DRY > SOLID. Sem camadas DDD vazias, sem lib compartilhada, sem Nx/Turborepo, sem auth/persistência/estacionamento. Frontend não chama o backend. Identificadores em inglês; docs/UI/nomes de teste em pt-BR

**Scale/Scope**: 2 projetos, 1 endpoint, 1 tela (4 elementos visuais), 0 entidades persistidas

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Status | Como o plano atende |
|-----------|--------|---------------------|
| I. YAGNI | Pass | Um endpoint, uma tela, sem DDD/Swagger/Docker/workspace |
| II. KISS | Pass | Minimal API + `App.tsx` com HTML semântico; tokens CSS |
| III. DRY | Pass | DESIGN.md é a fonte humana; CSS só materializa tokens. Sem pacote compartilhado (proibido pela spec) |
| IV. Clean Code | Pass | Nomes em inglês no código; arquivos pequenos; sem mortos |
| V. Ortogonalidade | Pass | Backend e frontend independentes; UI não conhece `/alive` |
| VI. SOLID | Pass | Sem interface/factory para uma implementação |
| VII. Testes | Pass | 80% no README; convenção de nomes; suíte em `tests/` |
| Idioma | Pass | READMEs declaram inglês no código; resto pt-BR |
| Fluxo Git | Pass | Branch curto a partir de `main`; fatia desta spec (docs + os dois lados) no mesmo PR; DESIGN.md tem spec própria nesta feature |
| Precedência | Pass | Não há abstração “para o futuro” |

**Pós-Fase 1:** contratos `alive.yaml` e `showcase-ui.md` descrevem só o que a spec já exige. Sem violação nova. Tabela de complexidade vazia.

## Project Structure

### Documentation (this feature)

```text
specs/001-fundacoes-monorepo/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── alive.yaml
│   └── showcase-ui.md
└── tasks.md              # /speckit-tasks — ainda não criado
```

### Source Code (repository root)

```text
.gitignore
backend/
├── README.md
├── ParkingApp.sln
├── src/
│   ├── ParkingApp.Api.csproj
│   ├── Program.cs
│   └── Properties/
│       └── launchSettings.json   # http://localhost:5080
└── tests/
    ├── ParkingApp.Api.Tests.csproj
    └── AliveEndpointTests.cs     # GetAlive_servicoEmExecucao_retornaEstouVivo
frontend/
├── README.md
├── DESIGN.md
├── package.json
├── vite.config.ts
├── index.html
├── src/
│   ├── main.tsx
│   ├── App.tsx
│   └── styles/
│       ├── tokens.css
│       └── app.css
└── tests/
    └── App.test.tsx              # render_vitrineInicial_exibeTituloCardBotaoECampo
```

**Structure Decision:** Dois projetos irmãos na raiz, como FR-001–003. Código do API plano em `backend/src` (sem pastas Domain/Application/Infrastructure). Vitrine no `App.tsx`, sem biblioteca de componentes. `tests/` irmão de `src/` em cada projeto. Atualizar `.gitignore` da raiz com `bin/`, `obj/`, `node_modules/`, `dist/` e pastas de cobertura.

## Complexity Tracking

> Nenhuma violação da constitution. Sem linhas nesta tabela.
