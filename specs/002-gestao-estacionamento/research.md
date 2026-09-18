# Pesquisa: Gestão de estacionamento

## 1. Autenticação

**Decision:** Cookie HTTP-only (`ParkingApp.Session`) com `AddAuthentication().AddCookie()`. Senha com `PasswordHasher<User>` (`Microsoft.Extensions.Identity.Core`). Sem ASP.NET Identity completo. Sessão obrigatória em `/api/*`, exceto `POST /api/session`. `GET /alive` permanece anônimo. Cookie **não persistente** (`IsPersistent = false`): fecha com o navegador/aplicativo. Sem timeout por ociosidade (`ExpireTimeSpan` longo, ex. 12 h, só como teto de segurança do ticket; sliding ligado para o expediente). `DELETE /api/session` encerra a sessão.

**Rationale:** Clarify: a sessão vale até o logout ou o fechamento do aplicativo. Cookie de sessão atende o “fechar o app”; logout apaga o cookie. Timeout curto de 15 min contradiz a spec. O hasher do framework não adiciona loja Identity (YAGNI / KISS). Cookie + proxy Vite no mesmo origin evita JWT e CORS.

**Alternatives considered:**
- JWT no `localStorage` — XSS e sessão duplicada na UI.
- Identity UI + EF Identity — telas e tabelas fora da spec.
- Cookie persistente de 14 dias — permanece depois de fechar o app.
- Expirar aos 15 min ociosos — clarify rejeitou.

## 2. Persistência

**Decision:** EF Core + SQLite (`parking.db` em `backend/`, ignorado no git). Um `ParkingDbContext` concreto em Infrastructure. Application usa o contexto direto, sem `IRepository<T>`. Instante gravado em UTC (`DateTimeOffset`); exibição em `America/Sao_Paulo`.

**Rationale:** README do backend já fixa EF Core + SQLite. Constitution: um único meio; interface de repositório com uma implementação é arquitetura extra.

**Alternatives considered:**
- JSON em arquivo — não aguenta concorrência de duas entradas na mesma vaga.
- PostgreSQL / SQL Server — segundo motor sem pedido.

## 3. Recorte Domain / Application / Infrastructure

**Decision:** Pastas no mesmo `ParkingApp.Api.csproj`. Domain: placa (inclui `ABC1234` → `ABC-1234`), preço da estadia, escolha de tarifa, ocupação, esperado do caixa, “último admin” — zero I/O. Application: casos de uso. Infrastructure: EF, SQLite, hash, PDF. API: Minimal APIs. UI: React.

**Rationale:** Spec tem regra persistida; constitution exige o recorte e proíbe pastas vazias e três assemblies.

**Alternatives considered:**
- Tudo em `Program.cs` — mistura regra com SQLite.
- Três csproj — YAGNI.
- MediatR / UoW genérico — indireção sem variação.

## 4. Acoplamento UI ↔ API

**Decision:** Vite faz proxy de `/api` para `http://localhost:5080`. Sem SignalR: após mutação a tela recarrega o painel; permanência na lista é calculada no cliente a partir de `entryAt`.

**Rationale:** Cookie funciona sem CORS. “Atualizar imediatamente” é o operador que acabou de lançar.

**Alternatives considered:**
- CORS + cookie em `:5080`.
- Polling / SignalR.

## 5. Concorrência da vaga e troca

**Decision:** Transação com `UPDATE spots SET status = Occupied WHERE id = @id AND status = Free`; índice único filtrado em estadia ativa por `spotId` e por placa normalizada. Primeira confirmação vence (409 na segunda). **Não existe** endpoint de mover estadia ativa para outra vaga.

**Rationale:** Clarify: sem reserva no formulário; sem troca de vaga depois da entrada.

**Alternatives considered:**
- PATCH de vaga na estadia ativa — rejeitado na clarify.
- Lock pessimista na tela.

## 6. Relatórios PDF e CSV

**Decision:** CSV no servidor (sem CsvHelper). PDF com QuestPDF. Impressão via `window.print`.

**Rationale:** FR-053 pede arquivo PDF e CSV.

**Alternatives considered:**
- Só “salvar como PDF” do browser.
- jsPDF no cliente.

## 7. Usuário inicial e último administrador

**Decision:** Se a tabela de usuários estiver vazia no arranque, criar Administrador `admin` (`PARKING_BOOTSTRAP_PASSWORD`). Sem seed de pátio. Inativar ou mudar para Operador o último Administrador ativo devolve 409.

**Rationale:** Spec: administrador da implantação; clarify: sempre resta um admin ativo.

**Alternatives considered:**
- Permitir zero admins e recuperação fora do sistema.
- Seed de setores/vagas.

## 8. Frontend: rotas e vitrine

**Decision:** `react-router`. Vitrine deixa de ser a home. Nav com os sete itens; Auditoria em Relatórios (admin). Logout no shell. Tokens do DESIGN.md.

**Rationale:** FR-016 exige URL para recusar operador. A fundação já cumpriu a vitrine.

**Alternatives considered:**
- Views sem URL.
- Manter vitrine como home.

## 9. Fechamento de caixa

**Decision:** `POST /api/cash/{id}/close` permitido a qualquer operador ou administrador autenticado. `closedByUserId` é quem chamou o close, independente de `openedByUserId`.

**Rationale:** Clarify: troca de turno. Reabertura continua só admin.

**Alternatives considered:**
- Só quem abriu fecha.
- Operador só fecha o próprio; admin fecha qualquer um.

## 10. Placa

**Decision:** Domain normaliza: maiúsculas; se 7 caracteres `ABC1234` (3 letras + 4 dígitos) → `ABC-1234`; Mercosul `ABC1D23` intacto. Busca compara a forma normalizada.

**Rationale:** Clarify: operador digita sem hífen no pátio.

**Alternatives considered:**
- Recusar sem hífen.
- Gravar só alfanumérico sem hífen.
