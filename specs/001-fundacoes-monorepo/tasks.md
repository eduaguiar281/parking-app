---
description: "Lista de tarefas da fundação do monorepo"
---

# Tasks: Fundações do monorepo

**Input**: Design documents from `/specs/001-fundacoes-monorepo/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Incluídos — FR-010 e constitution exigem suíte, 80% de linhas e nomes `<método>_<cenário>_<resultado esperado>`.

**Organization**: Tarefas por história de usuário para implementação e teste independentes.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Pode rodar em paralelo (arquivos diferentes, sem depender de tarefa incompleta)
- **[Story]**: US1, US2, US3 — só nas fases de história
- Caminhos de arquivo obrigatórios na descrição

## Path Conventions

- Backend: `backend/src/`, `backend/tests/`
- Frontend: `frontend/src/`, `frontend/tests/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Pastas do monorepo e ignore de artefatos gerados

- [x] T001 Criar a árvore `backend/src/`, `backend/tests/`, `frontend/src/styles/` e `frontend/tests/` conforme `specs/001-fundacoes-monorepo/plan.md`
- [x] T002 Atualizar `.gitignore` com `backend/**/bin/`, `backend/**/obj/`, `frontend/node_modules/`, `frontend/dist/`, `coverage/` e `TestResults/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Esqueleto que compila nos dois projetos, sem `/alive` e sem vitrine

**⚠️ CRITICAL**: Nenhuma história começa antes desta fase

- [x] T003 Criar `backend/src/ParkingApp.Api.csproj` (SDK .NET 10, Web) e `backend/src/Program.cs` que sobe o host e compila, sem endpoint de vida
- [x] T004 [P] Criar o esqueleto Vite + React 19.3 + TypeScript em `frontend/package.json`, `frontend/vite.config.ts`, `frontend/tsconfig.json`, `frontend/index.html`, `frontend/src/main.tsx` e `frontend/src/App.tsx` (stub, sem vitrine)
- [x] T005 Criar `backend/tests/ParkingApp.Api.Tests.csproj` referenciando a API, xUnit, `Microsoft.AspNetCore.Mvc.Testing` e Coverlet com limiar de 80% de linhas
- [x] T006 Criar `backend/ParkingApp.sln` incluindo `backend/src/ParkingApp.Api.csproj` e `backend/tests/ParkingApp.Api.Tests.csproj`
- [x] T007 Configurar Vitest + Testing Library + jsdom + cobertura 80% de linhas em `frontend/vite.config.ts` com testes em `frontend/tests/`

**Checkpoint**: `dotnet build` em `backend/` e `npm install && npm run build` em `frontend/` passam. Sem regra de estacionamento.

---

## Phase 3: User Story 1 - Navegar o monorepo (Priority: P1) 🎯 MVP

**Goal**: Um colega abre a raiz, vê `backend/` e `frontend/` e entende padrões, stack e testes pelos READMEs em pt-BR.

**Independent Test**: Clone mental da raiz → pastas irmãs com `src/`, `tests/` e `README.md` preenchidos (SC-001, SC-002). Não precisa subir processo.

### Implementation for User Story 1

- [x] T008 [P] [US1] Escrever `backend/README.md` em pt-BR com propósito, SDK .NET 10, identificadores em inglês, cobertura 80%, convenção de nomes de teste, e comandos `dotnet test` / `dotnet run --project src`
- [x] T009 [P] [US1] Escrever `frontend/README.md` em pt-BR com propósito, React 19.3, Node 20.19+ ou 22 LTS, identificadores em inglês, cobertura 80%, convenção de nomes de teste, e comandos `npm install` / `npm test` / `npm run dev`

**Checkpoint**: US1 demonstrável só lendo a raiz e os dois READMEs.

---

## Phase 4: User Story 2 - Serviço pronto para evoluir (Priority: P1)

**Goal**: Serviço .NET 10 sobe e `GET /alive` devolve `estou vivo` (contrato `contracts/alive.yaml`).

**Independent Test**: `cd backend && dotnet test && dotnet run --project src` e abrir `http://localhost:5080/alive` → 200 e corpo `estou vivo` em < 3 s. Sem o frontend.

### Tests for User Story 2 ⚠️

> Escrever o teste PRIMEIRO e garantir que FALHA antes de T011

- [x] T010 [US2] Escrever o teste `GetAlive_servicoEmExecucao_retornaEstouVivo` em `backend/tests/AliveEndpointTests.cs` (WebApplicationFactory, GET `/alive`, 200, `text/plain`, corpo `estou vivo`)

### Implementation for User Story 2

- [x] T011 [US2] Mapear `GET /alive` em `backend/src/Program.cs` para 200 `text/plain` com corpo exato `estou vivo`, sem autenticação
- [x] T012 [P] [US2] Fixar `http://localhost:5080` em `backend/src/Properties/launchSettings.json`
- [x] T013 [US2] Rodar `dotnet test` em `backend/` e confirmar cobertura de linhas ≥ 80% e o teste T010 verde

**Checkpoint**: US2 independente da vitrine. Sem o processo, `/alive` não parece sucesso.

---

## Phase 5: User Story 3 - Interface com identidade visual documentada (Priority: P1)

**Goal**: SPA React 19.3 com DESIGN.md e vitrine (título, card, botão, campo), sem chamar o backend.

**Independent Test**: Backend desligado → `cd frontend && npm test && npm run dev` → `http://localhost:5173` mostra os quatro elementos (SC-004, SC-005).

### Tests for User Story 3 ⚠️

> Escrever o teste PRIMEIRO e garantir que FALHA antes de T017

- [x] T014 [US3] Escrever o teste `render_vitrineInicial_exibeTituloCardBotaoECampo` em `frontend/tests/App.test.tsx` (heading 1, card, button, textbox; nenhuma chamada de rede)

### Implementation for User Story 3

- [x] T015 [P] [US3] Escrever `frontend/DESIGN.md` em pt-BR com tokens e do’s/don’ts (dark-only, acento violeta, Inter, grade 4px, raios de card/controle)
- [x] T016 [P] [US3] Materializar tokens em `frontend/src/styles/tokens.css`
- [x] T017 [P] [US3] Estilizar vitrine em `frontend/src/styles/app.css` (card, botão primário, campo, hover/foco)
- [x] T018 [US3] Implementar a vitrine em `frontend/src/App.tsx` (h1, article, button, input+label em pt-BR; importar os CSS; sem fetch)
- [x] T019 [P] [US3] Carregar Inter em `frontend/index.html`
- [x] T020 [US3] Rodar `npm test` em `frontend/` e confirmar cobertura de linhas ≥ 80% e o teste T014 verde

**Checkpoint**: US3 funciona com o backend desligado. Sem catálogo extra de componentes.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Alinhar docs ao que realmente roda e validar o quickstart

- [x] T021 [P] Conferir que `backend/README.md` documenta `http://localhost:5080/alive` e o comando único de teste
- [x] T022 [P] Conferir que `frontend/README.md` documenta `http://localhost:5173` e que a UI não depende do backend
- [x] T023 Remover stub/counter/assets padrão do Vite que sobrarem em `frontend/src/` e `frontend/public/`
- [x] T024 Executar os passos de `specs/001-fundacoes-monorepo/quickstart.md` (histórias 1–3, inclusive US3 com backend desligado)
- [x] T025 Confirmar que `frontend/src/` não contém `fetch`, `axios` nem URL do backend

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: sem dependências
- **Foundational (Phase 2)**: depende da Phase 1 — BLOQUEIA as histórias
- **User Stories (Phase 3–5)**: dependem da Phase 2; US1, US2 e US3 são independentes entre si
- **Polish (Phase 6)**: depois das histórias que forem entrar no incremento

### User Story Dependencies

- **User Story 1 (P1)**: depois da Phase 2 — só READMEs
- **User Story 2 (P1)**: depois da Phase 2 — não depende da US1 nem da US3 (README da US1 já cita os comandos)
- **User Story 3 (P1)**: depois da Phase 2 — não chama o backend; T018 depende de T014, T016 e T017

### Within Each User Story

- Testes (US2/US3) primeiro, falhando, depois implementação
- Sem modelos de persistência
- US2: teste de contrato → `Program.cs` → `launchSettings.json` → `dotnet test`
- US3: DESIGN.md e CSS em paralelo → `App.tsx` → `npm test`

### Parallel Opportunities

- T003 e T004 em paralelo (depois T001)
- T008 e T009 em paralelo
- T012 em paralelo com T010 (arquivos diferentes)
- T015, T016, T017, T019 em paralelo após T004/T007; T018 depois do teste T014 e dos CSS
- US2 e US3 em paralelo após a Phase 2 (pessoas diferentes)
- T021 e T022 em paralelo

---

## Parallel Example: User Story 1

```bash
Task: "Escrever backend/README.md"
Task: "Escrever frontend/README.md"
```

## Parallel Example: User Story 3

```bash
Task: "Escrever frontend/DESIGN.md"
Task: "Materializar frontend/src/styles/tokens.css"
Task: "Estilizar frontend/src/styles/app.css"
Task: "Carregar fontes em frontend/index.html"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Phase 1 + Phase 2
2. Phase 3 (READMEs)
3. **STOP**: navegar a raiz e os READMEs
4. Demo de orientação do repo

### Incremental Delivery

1. Setup + Foundational → projetos compilam
2. US1 → navegação documentada (MVP)
3. US2 → `GET /alive` (demo do serviço)
4. US3 → vitrine do DESIGN.md (demo da interface)
5. Phase 6 → quickstart.md

### Parallel Team Strategy

1. Time faz Phase 1–2 junto
2. Depois:
   - Dev A: US1
   - Dev B: US2
   - Dev C: US3
3. Phase 6 no final

---

## Notes

- [P] = arquivos diferentes, sem dependência pendente
- Commits Conventional Commits, pequenos, branch curto até `main`
- Sem `apps/`, `packages/`, camadas DDD, auth ou persistência
- Identificadores de código em inglês; README, DESIGN.md, UI e nomes de teste em pt-BR
