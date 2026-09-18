---
description: "Lista de tarefas da gestão de estacionamento"
---

# Tasks: Gestão de estacionamento

**Input**: Design documents from `/specs/002-gestao-estacionamento/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Incluídos — constitution VII, plan.md e Independent Test de cada história. Nomes `<método>_<cenário>_<resultado esperado>` em pt-BR. Cobertura ≥ 80% de linhas. Testes de Domain sem I/O.

**Organization**: Tarefas por história de usuário para implementação e teste independentes.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Pode rodar em paralelo (arquivos diferentes, sem depender de tarefa incompleta)
- **[Story]**: US1–US11 — só nas fases de história
- Caminhos de arquivo obrigatórios na descrição

## Path Conventions

- Backend: `backend/src/`, `backend/tests/`
- Frontend: `frontend/src/`, `frontend/tests/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Pacotes, ignore e proxy sobre o monorepo já existente

- [X] T001 Adicionar EF Core SQLite, `Microsoft.Extensions.Identity.Core` e QuestPDF em `backend/src/ParkingApp.Api.csproj` e referências de teste em `backend/tests/ParkingApp.Api.Tests.csproj`
- [X] T002 [P] Adicionar `react-router-dom` em `frontend/package.json`
- [X] T003 [P] Ignorar `backend/parking.db` e `backend/*.db-shm` / `backend/*.db-wal` em `.gitignore`
- [X] T004 Configurar proxy `/api` → `http://localhost:5080` em `frontend/vite.config.ts`
- [X] T005 Atualizar persistência e sessão em `backend/README.md` e o fato de a UI chamar `/api` em `frontend/README.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Recorte D/A/I, SQLite, cookie, erros, cliente HTTP e shell de rotas. Sem login ainda.

**⚠️ CRITICAL**: Nenhuma história começa antes desta fase

- [X] T006 Criar enumerações de domínio em `backend/src/Domain/ParkingEnums.cs`
- [X] T007 [P] Criar resposta `{ code, message }` em pt-BR em `backend/src/Infrastructure/HttpError.cs`
- [X] T008 [P] Criar conversão UTC ↔ `America/Sao_Paulo` em `backend/src/Infrastructure/BrazilianTime.cs`
- [X] T009 Criar `ParkingDbContext` em `backend/src/Infrastructure/ParkingDbContext.cs` e registrar SQLite + `EnsureCreated` em `backend/src/Program.cs`
- [X] T010 Registrar cookie `ParkingApp.Session` em `backend/src/Program.cs` (sem `PasswordHasher` e sem endpoints de sessão)
- [X] T011 Mapear `MapGroup("/api")` com autorização padrão e manter `GET /alive` anônimo em `backend/src/Program.cs`
- [X] T012 Criar `AuditEvent` e `AuditWriter` em `backend/src/Infrastructure/AuditWriter.cs`
- [X] T013 [P] Criar `fetch` com `credentials: "include"` em `frontend/src/api/client.ts`
- [X] T014 Trocar a vitrine por `BrowserRouter` e rotas vazias autenticadas em `frontend/src/App.tsx` (tokens em `frontend/src/styles/tokens.css` permanecem)
- [X] T015 Criar `WebApplicationFactory` com SQLite isolado em `backend/tests/Api/ApiFactory.cs`

**Checkpoint**: `dotnet build` e `npm run build` passam. `GET /alive` inalterado. `/api` responde 401 sem cookie.

---

## Phase 3: User Story 1 - Entrar no sistema com o perfil correto (Priority: P1) 🎯 MVP

**Goal**: Login/senha, cookie, admin inicial, nav só com o que o perfil pode ver; inativo e senha errada recusados.

**Independent Test**: Seed admin; teste de API cria um operador; login de cada um; operador em `/usuarios` recusado; inativo não entra.

### Tests for User Story 1 ⚠️

> Escrever os testes PRIMEIRO e garantir que FALHAM antes da implementação

- [X] T016 [P] [US1] Testes de sessão `CreateSession_credenciaisValidas_defineCookie`, `CreateSession_usuarioInativo_retorna401` e `CreateSession_senhaInvalida_retorna401` em `backend/tests/Api/SessionEndpointTests.cs`
- [X] T017 [P] [US1] Teste `render_login_exibeCamposERecusaGenerica` em `frontend/tests/LoginPage.test.tsx`

### Implementation for User Story 1

- [X] T018 [US1] Criar entidade `User` em `backend/src/Infrastructure/Entities/User.cs` e mapeamento em `backend/src/Infrastructure/ParkingDbContext.cs`
- [X] T019 [US1] Registrar `PasswordHasher<User>` em `backend/src/Program.cs` e implementar `CreateSession` / `GetSession` / `DeleteSession` em `backend/src/Application/Sessions/SessionService.cs` e `POST|GET|DELETE /api/session` em `backend/src/Api/SessionEndpoints.cs`
- [X] T020 [US1] Semear Administrador `admin` quando a tabela estiver vazia em `backend/src/Infrastructure/BootstrapAdmin.cs` (senha `PARKING_BOOTSTRAP_PASSWORD`)
- [X] T021 [US1] Exigir papel nas rotas administrativas (403) em `backend/src/Api/RequireRole.cs`
- [X] T022 [US1] Implementar `frontend/src/pages/LoginPage.tsx` e `frontend/src/components/AppNav.tsx` (itens por perfil; hamburger ≤ 833px)
- [X] T023 [US1] Proteger rotas e recusar endereço administrativo do operador em `frontend/src/App.tsx` e `frontend/src/components/RequireRole.tsx`

**Checkpoint**: US1 demonstrável com login admin, logout e bloqueio de operador (usuário operador só no teste).

---

## Phase 4: User Story 2 - Configurar setores e vagas (Priority: P1)

**Goal**: Admin cadastra setores, vagas avulsas e em lote; bloqueio/manutenção; operador não acessa.

**Independent Test**: Cadastrar Pátio A (motos) e Pátio B (carros), lote P-A-001–003, bloquear uma vaga; lote com código existente recusa o intervalo inteiro.

### Tests for User Story 2 ⚠️

- [X] T024 [P] [US2] Testes `CreateSpotBatch_intervaloLivre_criaTodas` e `CreateSpotBatch_codigoExistente_naoCriaNada` em `backend/tests/Api/SpotEndpointTests.cs`
- [X] T025 [P] [US2] Teste `render_vagas_operadorNaoAcessa` em `frontend/tests/SpotsPage.test.tsx`

### Implementation for User Story 2

- [X] T026 [P] [US2] Criar entidades `Sector` e `Spot` em `backend/src/Infrastructure/Entities/Sector.cs` e `backend/src/Infrastructure/Entities/Spot.cs`
- [X] T027 [US2] Implementar cadastro, lote e mudança de status (ocupada não vai para bloqueada/manutenção) em `backend/src/Application/Spots/SpotService.cs` e `backend/src/Application/Sectors/SectorService.cs`
- [X] T028 [US2] Mapear `/api/sectors` e `/api/spots` (+ `/batch`) em `backend/src/Api/SectorEndpoints.cs` e `backend/src/Api/SpotEndpoints.cs` (admin). `GET /api/spots/suggest` fica na US5.
- [X] T029 [US2] Implementar `frontend/src/pages/SpotsPage.tsx` (setores, vagas, lote, indicadores com tokens do DESIGN.md)

**Checkpoint**: Admin gerencia o mapa do pátio; operador recebe 403.

---

## Phase 5: User Story 3 - Configurar tabelas tarifárias (Priority: P1)

**Goal**: Admin cria/edita/inativa tarifas; conflito de abrangência; usada não se exclui; específica do setor vence a geral.

**Independent Test**: Tarifas carro Pátio B R$ 10/5 e moto Pátio A R$ 5/3; segunda tabela igual recusada; exclusão de usada recusada.

### Tests for User Story 3 ⚠️

- [X] T030 [P] [US3] Teste de Domain `SelectTariff_geralESetor_escolheEspecifica` em `backend/tests/Domain/TariffSelectorTests.cs`
- [X] T031 [P] [US3] Testes `CreateTariff_abrangenciaSobreposta_retorna409` e `DeleteTariff_jaUtilizada_naoPermitido` em `backend/tests/Api/TariffEndpointTests.cs`

### Implementation for User Story 3

- [X] T032 [P] [US3] Implementar escolha de tarifa (setor > geral) em `backend/src/Domain/TariffSelector.cs`
- [X] T033 [US3] Criar entidade `TariffTable` em `backend/src/Infrastructure/Entities/TariffTable.cs` e serviço em `backend/src/Application/Tariffs/TariffService.cs` (inativar; não excluir se referenciada)
- [X] T034 [US3] Mapear `/api/tariffs` em `backend/src/Api/TariffEndpoints.cs` (admin) e gravar auditoria
- [X] T035 [US3] Implementar `frontend/src/pages/TariffsPage.tsx` com confirmação antes de alterar tarifa

**Checkpoint**: Entrada ainda não existe, mas tabelas e regra de precedência estão testáveis.

---

## Phase 6: User Story 4 - Abrir e controlar o caixa do dia (Priority: P1)

**Goal**: Um caixa aberto no sistema; abertura, totais, sangria/suprimento/ajuste (admin), fechamento com esperado = abertura + receitas + suprimentos − sangrias ± ajustes; reabertura só admin.

**Independent Test**: Abrir, lançar sangria e suprimento, fechar com diferença visível, recusar segundo caixa aberto (mesma data ou outra).

### Tests for User Story 4 ⚠️

- [X] T036 [P] [US4] Teste de Domain `CalculateExpected_aberturaReceitasSangriaAjuste_retornaFormula` em `backend/tests/Domain/CashExpectedTests.cs`
- [X] T037 [P] [US4] Testes `OpenCash_jaExisteAberto_retorna409` e `CloseCash_comMovimentos_gravaEsperadoEDiferenca` em `backend/tests/Api/CashEndpointTests.cs`

### Implementation for User Story 4

- [X] T038 [P] [US4] Implementar fórmula do esperado em `backend/src/Domain/CashExpected.cs`
- [X] T039 [US4] Criar `CashRegister` e `CashMovement` em `backend/src/Infrastructure/Entities/CashRegister.cs` e `backend/src/Infrastructure/Entities/CashMovement.cs`
- [X] T040 [US4] Implementar abrir/fechar/reabrir/movimentos especiais em `backend/src/Application/Cash/CashService.cs` (um Open; um por `operationalDate`; caixa fechado não edita lançamentos; ExitPayment imutável; correção só via reabertura + sangria/suprimento/ajuste)
- [X] T041 [US4] Mapear `/api/cash` em `backend/src/Api/CashEndpoints.cs`
- [X] T042 [US4] Implementar `frontend/src/pages/CashPage.tsx` (abrir/fechar; sangria só admin; confirmação de fechamento)

**Checkpoint**: Caixa do dia opera sem veículos.

---

## Phase 7: User Story 5 - Registrar entrada de veículo (Priority: P1)

**Goal**: Placa normalizada, vaga compatível, tarifa vigente com snapshot, uma vaga por veículo, primeira confirmação vence disputa.

**Independent Test**: Entrada válida ocupa vaga; mesma placa recusada; moto em setor de carro recusada; sem tarifa recusada; duas entradas na mesma vaga → 409 na segunda.

### Tests for User Story 5 ⚠️

- [X] T043 [P] [US5] Testes de Domain `Normalize_placaMinuscula_retornaMaiuscula`, `Validate_mercosulValida_aceita` e `Validate_formatoInvalido_rejeita` em `backend/tests/Domain/PlateTests.cs`
- [X] T044 [P] [US5] Testes `RegisterEntry_vagaLivre_ocupa`, `RegisterEntry_placaAtiva_retorna409` e `RegisterEntry_vagaDisputada_segundaRetorna409` em `backend/tests/Api/StayEntryEndpointTests.cs`

### Implementation for User Story 5

- [X] T045 [P] [US5] Implementar placa em `backend/src/Domain/Plate.cs`
- [X] T046 [US5] Criar entidade `ParkingStay` (status Active + snapshots) em `backend/src/Infrastructure/Entities/ParkingStay.cs` com índices únicos filtrados (placa ativa, vaga ativa)
- [X] T047 [US5] Implementar entrada (update condicional da vaga Free→Occupied, tarifa obrigatória, auditoria) em `backend/src/Application/Stays/RegisterEntry.cs`
- [X] T048 [US5] Mapear `POST /api/stays` e `GET /api/spots/suggest` em `backend/src/Api/StayEndpoints.cs` e `backend/src/Api/SpotEndpoints.cs`
- [X] T049 [US5] Implementar formulário de entrada em `frontend/src/pages/OperationPage.tsx` (placa, tipo, setor/vaga compatíveis, sugerir)

**Checkpoint**: Pátio ocupa vagas; painel completo ainda não é obrigatório.

---

## Phase 8: User Story 6 - Registrar saída e pagamento (Priority: P1)

**Goal**: Resumo, pagamento, hora cheia pelos snapshots, receita no caixa aberto, vaga livre, comprovante; estadia paga imutável; cancelar só Active (admin).

**Independent Test**: Cobranças da spec (carro 30 min / 1h01 / 2h30; moto 45 min / 2h10); saída sem caixa recusada; comprovante.

### Tests for User Story 6 ⚠️

- [X] T050 [P] [US6] Testes de Domain `Calculate_carro30min_soPrimeiraHora`, `Calculate_carro1h01_primeiraMaisUma` e `Calculate_moto2h10_primeiraMaisDuas` em `backend/tests/Domain/StayPricingTests.cs`
- [X] T051 [P] [US6] Testes `ConfirmExit_semCaixaAberto_retorna409`, `ConfirmExit_pago_imutaValor` e `CancelStay_ativa_liberaVaga` em `backend/tests/Api/StayExitEndpointTests.cs`

### Implementation for User Story 6

- [X] T052 [P] [US6] Implementar preço (primeira hora cheia + `ceil(horas−1)` adicionais) em `backend/src/Domain/StayPricing.cs`
- [X] T053 [US6] Implementar preview/saída/cancelamento em `backend/src/Application/Stays/ExitStay.cs` e `backend/src/Application/Stays/CancelStay.cs` (ExitPayment imutável)
- [X] T054 [US6] Mapear `/api/stays/{id}/exit-preview`, `/exit`, `/cancel`, `/receipt` em `backend/src/Api/StayEndpoints.cs`
- [X] T055 [US6] Implementar diálogo de saída, formas de pagamento e comprovante imprimível em `frontend/src/components/ExitDialog.tsx`

**Checkpoint**: Ciclo entrada→pagamento→vaga livre fecha com caixa.

---

## Phase 9: User Story 7 - Acompanhar o painel operacional (Priority: P1)

**Goal**: Totais, ocupação, lista de estacionados, busca/filtros, permanência no cliente, atalho para saída, status do caixa e receita do dia.

**Independent Test**: Vagas em cada status + veículos na lista; filtro por placa; saída a partir da lista; totais = contagem real.

### Tests for User Story 7 ⚠️

- [X] T056 [P] [US7] Teste de Domain `OccupancyRate_semVagasUtilizaveis_retornaZero` em `backend/tests/Domain/OccupancyTests.cs`
- [X] T057 [P] [US7] Teste `GetDashboard_aposEntradaESaida_totaisConferem` em `backend/tests/Api/DashboardEndpointTests.cs`
- [X] T058 [P] [US7] Teste `render_painel_listaEstacionadosEFiltroPlaca` em `frontend/tests/OperationPage.test.tsx`

### Implementation for User Story 7

- [X] T059 [P] [US7] Implementar taxa ocupadas/(ocupadas+livres) em `backend/src/Domain/Occupancy.cs`
- [X] T060 [US7] Implementar `GET /api/operations/dashboard` em `backend/src/Application/Operations/DashboardService.cs` e `backend/src/Api/DashboardEndpoints.cs`
- [X] T061 [US7] Completar painel (totais, ocupação por setor, caixa, receita do dia, permanência a partir de `entryAt`) em `frontend/src/pages/OperationPage.tsx`

**Checkpoint**: Expediente visível no `/operacao`. Histórias P1 de pátio fechadas.

---

## Phase 10: User Story 8 - Gerenciar usuários (Priority: P2)

**Goal**: Admin cria, edita, inativa e reativa usuários; operador não gerencia.

**Independent Test**: Criar operador, inativar (não entra), reativar; operador não abre `/usuarios`.

### Tests for User Story 8 ⚠️

- [X] T062 [P] [US8] Testes `CreateUser_admin_criaOperador` e `DeactivateUser_operador_naoAutentica` em `backend/tests/Api/UserEndpointTests.cs`
- [X] T063 [P] [US8] Teste `render_usuarios_operadorRecebeRecusa` em `frontend/tests/UsersPage.test.tsx`

### Implementation for User Story 8

- [X] T064 [US8] Implementar CRUD de status/perfil em `backend/src/Application/Users/UserService.cs` e `/api/users` em `backend/src/Api/UserEndpoints.cs` (admin)
- [X] T065 [US8] Implementar `frontend/src/pages/UsersPage.tsx`

**Checkpoint**: Segundo atendente existe sem seed extra.

---

## Phase 11: User Story 9 - Consultar histórico de estacionamentos (Priority: P2)

**Goal**: Lista de estadias concluídas com filtros; snapshots intactos após mudança de tarifa/vaga.

**Independent Test**: Finalizar estadias distintas e isolar cada uma pelos filtros.

### Tests for User Story 9 ⚠️

- [X] T066 [P] [US9] Testes `ListStayHistory_filtroPlacaEPeriodo_retornaSoCompativeis` e `ListStayHistory_tarifaAlteradaDepois_mantemSnapshot` em `backend/tests/Api/StayHistoryEndpointTests.cs`
- [X] T067 [P] [US9] Teste `render_historico_filtraPorPlaca` em `frontend/tests/HistoryPage.test.tsx`

### Implementation for User Story 9

- [X] T068 [US9] Completar `GET /api/stays` com filtros em `backend/src/Api/StayEndpoints.cs` e `backend/src/Application/Stays/StayHistory.cs`
- [X] T069 [US9] Implementar `frontend/src/pages/HistoryPage.tsx` (estado vazio informativo)

**Checkpoint**: Conferência operacional sem PDF.

---

## Phase 12: User Story 10 - Emitir relatórios operacionais e financeiros (Priority: P2)

**Goal**: Relatório de caixa e de entradas/saídas na tela, impressão, PDF e CSV; só admin.

**Independent Test**: Gerar os dois, imprimir, baixar PDF e CSV; totais iguais ao JSON.

### Tests for User Story 10 ⚠️

- [X] T070 [P] [US10] Testes `ReportCash_periodoComFechamento_incluiEsperado` e `ReportStays_periodo_totaisDeReceita` em `backend/tests/Api/ReportEndpointTests.cs`
- [X] T071 [P] [US10] Teste `GetReportPdf_admin_retornaApplicationPdf` em `backend/tests/Api/ReportExportTests.cs`

### Implementation for User Story 10

- [X] T072 [US10] Implementar JSON dos relatórios em `backend/src/Application/Reports/ReportService.cs` e `/api/reports/cash` + `/api/reports/stays` em `backend/src/Api/ReportEndpoints.cs`
- [X] T073 [US10] Gerar CSV manual e PDF QuestPDF em `backend/src/Infrastructure/ReportExport.cs` (`/cash.pdf`, `/cash.csv`, `/stays.pdf`, `/stays.csv`)
- [X] T074 [US10] Implementar `frontend/src/pages/ReportsPage.tsx` (visualizar, imprimir, baixar)

**Checkpoint**: Admin exporta conferência do dia.

---

## Phase 13: User Story 11 - Consultar auditoria (Priority: P3)

**Goal**: Admin lista eventos (quando, quem, ação, dados); operador 403. Gravação já ocorre nas mutações anteriores.

**Independent Test**: Mudança cadastral, abertura de caixa e saída aparecem na consulta; cancelamento de Active com motivo.

### Tests for User Story 11 ⚠️

- [X] T075 [P] [US11] Teste `ListAudit_aposEntrada_contemUsuarioEAcao` em `backend/tests/Api/AuditEndpointTests.cs`
- [X] T076 [P] [US11] Teste `render_auditoria_operadorNaoAcessa` em `frontend/tests/AuditSection.test.tsx`

### Implementation for User Story 11

- [X] T077 [US11] Mapear `GET /api/audit` em `backend/src/Api/AuditEndpoints.cs`
- [X] T078 [US11] Incluir seção de auditoria em `frontend/src/pages/ReportsPage.tsx`

**Checkpoint**: Rastro administrativo consultável.

---

## Phase 14: Polish & Cross-Cutting Concerns

**Purpose**: Idioma, cobertura, quickstart, estados vazios e DESIGN.md

- [X] T079 [P] Revisar textos de UI e mensagens de API em pt-BR; moeda R$ e datas `dd/MM/yyyy` em `frontend/src` e `backend/src/Infrastructure/BrazilianTime.cs`
- [X] T080 [P] Garantir estados vazios, toasts/mensagens de sucesso na tela e confirmações sensíveis nas páginas em `frontend/src/pages/`
- [X] T081 [P] Testar recorte ~419px (entrada, saída e caixa utilizáveis) em `frontend/tests/ResponsiveOperation.test.tsx`
- [X] T082 Atualizar exemplos de teste nos READMEs (`backend/README.md`, `frontend/README.md`) para a convenção pt-BR e cobertura 80%
- [X] T083 Correr `dotnet test` e `npm test` até os limiares de cobertura; `GET /alive` continua verde em `backend/tests/AliveEndpointTests.cs`
- [X] T084 Executar o roteiro de `specs/002-gestao-estacionamento/quickstart.md` (login, setores, tarifas, caixa, entrada, saída, operador)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: sem dependências
- **Foundational (Phase 2)**: depende do Setup — **bloqueia** todas as histórias
- **US1 (Phase 3)**: após Foundational — MVP de acesso
- **US2–US4**: após US1 (cookie + papel). US2, US3 e US4 podem seguir em paralelo entre si
- **US5**: após US2 + US3 (vaga + tarifa)
- **US6**: após US5 + US4 (estadia + caixa aberto)
- **US7**: após US5 (lista de estacionados); ganha receita do dia com US6
- **US8–US11**: após US1; US9/US10 após US6; US11 após existir mutação auditada
- **Polish**: após as histórias que se pretende entregar

### User Story Dependencies

- **US1 (P1)**: só Foundational
- **US2 (P1)**: US1 (admin autenticado)
- **US3 (P1)**: US1; setor opcional usa US2
- **US4 (P1)**: US1
- **US5 (P1)**: US2 + US3
- **US6 (P1)**: US5 + US4
- **US7 (P1)**: US5 (completo com US6)
- **US8 (P2)**: US1
- **US9 (P2)**: US6
- **US10 (P2)**: US4 + US6
- **US11 (P3)**: mutações das histórias anteriores

### Within Each User Story

- Testes primeiro e devem falhar
- Entidade → Application → Endpoints → UI
- Auditoria no serviço que muta, não só na tela da US11

### Parallel Opportunities

- T002, T003 em paralelo no Setup
- T007, T008, T013 em paralelo no Foundational
- Após US1: um dev em US2, outro em US4
- Testes `[P]` de uma história no mesmo instante (arquivos diferentes)
- Domain `Plate` / `StayPricing` / `Occupancy` / `CashExpected` / `TariffSelector` em arquivos distintos

---

## Parallel Example: User Story 5

```bash
Task: "Testes de Domain em backend/tests/Domain/PlateTests.cs"
Task: "Testes de API em backend/tests/Api/StayEntryEndpointTests.cs"
Task: "Plate em backend/src/Domain/Plate.cs"
```

## Parallel Example: User Story 1

```bash
Task: "SessionEndpointTests em backend/tests/Api/SessionEndpointTests.cs"
Task: "LoginPage.test em frontend/tests/LoginPage.test.tsx"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Phase 1 Setup
2. Phase 2 Foundational
3. Phase 3 US1
4. **STOP**: validar login admin, 401 genérico e 403 do operador
5. Demo de acesso

### Incremental Delivery (produto)

1. Setup + Foundational
2. US1 → acesso
3. US2 + US3 + US4 → cadastros e caixa (podem intercalados)
4. US5 + US6 + US7 → **pátio operando** (objetivo do quickstart P1)
5. US8 → equipe
6. US9 + US10 + US11 → conferência
7. Polish + quickstart completo

### Parallel Team Strategy

1. Time fecha Setup + Foundational + US1
2. Dev A: US2 → US3 → US5
3. Dev B: US4 → depois junta em US6
4. Dev C: US8 e shell de UI
5. US7/US9/US10/US11 depois do ciclo entrada/saída

---

## Notes

- [P] = arquivos diferentes, sem depender de tarefa incompleta no mesmo arquivo (`Program.cs` não é [P] se várias tarefas o editam — preferir `*Endpoints.cs`)
- Domain sem SQLite, HTTP ou UI
- Sem `IRepository<T>`, SignalR ou Identity UI
- Commit por tarefa ou grupo lógico (Conventional Commits)
- Parar no checkpoint e validar a história sozinha
