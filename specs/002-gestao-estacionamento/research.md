# Pesquisa: Gestão de estacionamento

## 1. Autenticação

**Decision:** Cookie HTTP-only (`ParkingApp.Session`) com `AddAuthentication().AddCookie()`. Senha com `PasswordHasher<User>` (`Microsoft.Extensions.Identity.Core`). Sem ASP.NET Identity completo (sem `IdentityDbContext`, e-mail de reset ou roles genéricas). Sessão obrigatória em `/api/*`, exceto `POST /api/session`. `GET /alive` permanece anônimo.

**Rationale:** A spec pede login/senha e senha ilegível em texto claro. Cookie + proxy Vite no mesmo origin evita JWT e CORS com credenciais. O hasher do framework não adiciona loja Identity que a spec não pediu (YAGNI / KISS).

**Alternatives considered:**
- JWT no `localStorage` — XSS e sessão duplicada na UI.
- Identity UI + EF Identity — telas e tabelas (`AspNetUsers`) fora da spec.
- Basic Auth — senha a cada request.

## 2. Persistência

**Decision:** EF Core + SQLite (`parking.db` em `backend/`, ignorado no git). Um `ParkingDbContext` concreto em Infrastructure. Application usa o contexto direto, sem `IRepository<T>`. Instante gravado em UTC (`DateTimeOffset`); exibição em `America/Sao_Paulo` (formato brasileiro).

**Rationale:** README do backend já fixa EF Core + SQLite quando a spec persiste. Constitution: um único meio; interface de repositório com uma implementação é arquitetura extra. UTC evita ambiguidade de DST; a spec pede horário local só na interface.

**Alternatives considered:**
- JSON em arquivo — não aguenta concorrência de duas entradas na mesma vaga.
- PostgreSQL / SQL Server — segundo motor sem pedido.
- DateTime “local” no banco — quebra se o host não estiver no fuso do pátio.

## 3. Recorte Domain / Application / Infrastructure

**Decision:** Pastas no mesmo `ParkingApp.Api.csproj` (não três projetos). Domain: placa, preço da estadia, escolha de tarifa, ocupação, esperado do caixa — zero I/O. Application: casos de uso (entrada, saída, caixa, cadastros) orquestram Domain + `ParkingDbContext`. Infrastructure: EF, SQLite, hash de senha, PDF. API: Minimal APIs em `/api`. UI: React.

**Rationale:** A spec tem regra persistida; a constitution exige o recorte e proíbe pastas vazias e assemblies extras. Teste de Domain passa sem disco, rede ou host HTTP.

**Alternatives considered:**
- Tudo em `Program.cs` — mistura regra com SQLite (gate de Ortogonalidade falha).
- Três csproj Domain/Application/Infrastructure — YAGNI.
- MediatR / UoW genérico — indireção sem variação.

## 4. Acoplamento UI ↔ API

**Decision:** Vite faz proxy de `/api` para `http://localhost:5080`. A UI chama só `/api/...` (mesmo origin na porta 5173). Sem SignalR: após mutação a tela recarrega o painel; permanência na lista é calculada no cliente a partir de `entryAt`.

**Rationale:** Cookie funciona sem CORS. “Atualizar imediatamente” na spec é o operador que acabou de lançar, não um mural em tempo real. WebSocket seria peça extra.

**Alternatives considered:**
- CORS + cookie em `localhost:5080` — SameSite e portas diferentes.
- Polling agressivo / SignalR — complexidade sem requisito de multi-aba ao vivo.

## 5. Concorrência da vaga

**Decision:** Transação com `UPDATE spots SET status = Occupied WHERE id = @id AND status = Free`; índice único filtrado em estadia ativa por `spotId` e por placa normalizada. A primeira confirmação vence; a segunda recebe 409.

**Rationale:** Clarify: sem reserva enquanto o formulário está aberto. Constraint no banco garante o invariante mesmo com duas requisições.

**Alternatives considered:**
- Lock pessimista na tela — reserva que a clarify rejeitou.
- Last-write-wins — dois veículos na mesma vaga.

## 6. Relatórios PDF e CSV

**Decision:** CSV montado no servidor (sem CsvHelper). PDF com QuestPDF. Impressão da tela via `window.print` (comprovante e relatório visível). O JSON do relatório alimenta a visualização; PDF/CSV saem dos mesmos totais.

**Rationale:** FR-053 pede arquivo PDF e CSV, não só “imprimir”. CSV manual evita pacote. QuestPDF é um pacote justificado pelo formato pedido, não um segundo banco.

**Alternatives considered:**
- Só `window.print` / “salvar como PDF” — não entrega arquivo CSV/PDF de um clique.
- jsPDF no cliente — duplica a regra de totais na UI.

## 7. Usuário inicial e dados de demonstração

**Decision:** Se a tabela de usuários estiver vazia no arranque, criar um Administrador (`admin`). Senha local em `launchSettings` / variável `PARKING_BOOTSTRAP_PASSWORD`. Sem seed de setores, vagas, tarifas ou estadias. Não há dados de demonstração nesta feature; se surgirem depois, o rótulo visível é “Demonstração”.

**Rationale:** Spec: administrador da implantação; dados de demonstração, se existirem, precisam de aviso. Seed de pátio inventaria expediente falso. O quickstart cadastra o mínimo na UI.

**Alternatives considered:**
- Pátio de exemplo marcado “demonstração” — útil, mas a spec não exige e mistura cadastro com vitrine.
- Sem usuário inicial — primeiro acesso impossível.

## 8. Frontend: rotas e vitrine da fundação

**Decision:** `react-router` nas rotas da spec. A vitrine (título/card/botão/campo) deixa de ser a tela inicial; `tokens.css` e DESIGN.md continuam a fonte visual. Nav `{component.global-nav}` com os sete itens; Auditoria entra em Relatórios (admin). Indicadores de vaga usam tokens existentes (superfície, tinta, hairline); Action Blue só em ação — sem segunda cor de marca.

**Rationale:** FR-016 exige endereço de tela para o bloqueio do operador. Segunda paleta verde/vermelha violaria o DESIGN.md. A fundação já cumpriu a vitrine; esta feature é o produto.

**Alternatives considered:**
- Views sem URL (só estado React) — não dá para recusar “endereço direto”.
- Manter a vitrine como home — contradiz o painel operacional.
