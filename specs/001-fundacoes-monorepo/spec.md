# Feature Specification: Fundações do monorepo

**Feature Branch**: `001-fundacoes-monorepo`

**Created**: 2026-09-15

**Status**: Draft

**Input**: User description: "Crie as fundações desse projeto. Ele vai ser uma padrão monorepo. Estrutura backend/ (src, tests, README) e frontend (src, tests, README). Backend com SDK .NET 10. Frontend com React na versão mais estável e recente. Aparência via DESIGN.md no frontend, consultando QuestUI."

## Clarifications

### Session 2026-09-18

- Q: A identidade visual da fundação continua sendo QuestUI ornamental? → A: Não. A fonte da verdade é o `frontend/DESIGN.md` vigente: console dark-only com menu lateral, acento violeta, Inter. A vitrine continua sendo título, card, botão e campo **dentro** desse shell.

### Session 2026-09-15

- Q: Como um colega deve confirmar que o serviço do backend está no ar? → A: Abrir um endereço e receber uma resposta simples de que o serviço está vivo
- Q: A tela inicial do frontend deve consultar esse endereço de vida do backend, ou só mostrar a identidade visual? → A: A tela inicial é só a identidade visual; não fala com o backend nesta feature
- Q: O que a tela inicial precisa mostrar para provar a identidade visual? → A: Vitrine curta: título, card, botão e campo, todos no visual do DESIGN.md
- Q: Em que idioma devem ficar os nomes no código (tipos, funções, variáveis)? → A: Inglês nos identificadores de código; pt-BR em docs, UI e nomes de teste

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Navegar o monorepo (Priority: P1)

Um membro da equipe abre o repositório e encontra dois projetos irmãos, cada um com código, testes e um README que explica padrões, stack e cobertura de testes. Não precisa perguntar onde começa o serviço nem onde começa a interface.

**Why this priority**: Sem essa estrutura, nenhuma funcionalidade de estacionamento pode ser desenvolvida de forma previsível. É a base de todo o resto.

**Independent Test**: Abrir a raiz do repositório e confirmar as pastas `backend/` e `frontend/`, cada uma com `src/`, `tests/` e `README.md` preenchido em pt-BR.

**Acceptance Scenarios**:

1. **Given** um clone novo do repositório, **When** a pessoa abre a raiz, **Then** vê `backend/` e `frontend/` como projetos irmãos, sem misturar código dos dois lados.
2. **Given** a pasta `backend/`, **When** a pessoa abre o README, **Then** encontra padrões do projeto, stack, identificadores de código em inglês, cobertura mínima de testes e a convenção `<método>_<cenário>_<resultado esperado>` em pt-BR.
3. **Given** a pasta `frontend/`, **When** a pessoa abre o README, **Then** encontra os mesmos tipos de informação para a interface.

---

### User Story 2 - Serviço pronto para evoluir (Priority: P1)

Um membro da equipe consegue construir, executar e testar o serviço a partir de `backend/`, usando o SDK .NET 10. O projeto sobe sem regra de estacionamento ainda — só a fundação.

**Why this priority**: O backend é metade do produto. Se não compilá-lo e testá-lo agora, o README de cobertura fica mentira e as próximas specs não têm onde pousar.

**Independent Test**: Na pasta `backend/`, seguir o README, construir o serviço, iniciá-lo, abrir o endereço documentado e ver a resposta de “estou vivo”; em seguida a suíte de testes passa.

**Acceptance Scenarios**:

1. **Given** o SDK .NET 10 instalado, **When** a pessoa segue o README de `backend/`, **Then** o serviço constrói sem erro.
2. **Given** o serviço em execução, **When** a pessoa abre o endereço de vida documentado no README, **Then** recebe uma resposta simples de que o serviço está vivo, sem dados de estacionamento.
3. **Given** o README de `backend/` com cobertura mínima declarada, **When** a suíte em `backend/tests/` roda, **Then** os testes passam, usam a convenção de nomes e a cobertura atende o README.

---

### User Story 3 - Interface com identidade visual documentada (Priority: P1)

Um membro da equipe consegue construir, executar e testar a interface a partir de `frontend/`, com React na versão estável mais recente. A aparência segue o sistema de design descrito em `frontend/DESIGN.md`. A tela inicial é um console (menu lateral + área de trabalho) com vitrine curta no main (título, card, botão e campo) — dark-only, acento violeta `{colors.primary}`, tipografia Inter 13px/14px/18px. Nesta feature a tela NÃO consulta o backend: a vitrine funciona mesmo com o serviço desligado.

**Why this priority**: Sem identidade visual e sem app que rode, o frontend é só pasta vazia. O DESIGN.md é a especificação própria da aparência, exigida nesta fundação.

**Independent Test**: Abrir `frontend/DESIGN.md`, subir só a interface pelo README (backend desligado) e conferir título, card, botão e campo no visual do DESIGN.md.

**Acceptance Scenarios**:

1. **Given** o ambiente descrito no README de `frontend/` e o backend desligado, **When** a pessoa constrói e inicia a interface, **Then** vê em pt-BR um título, um card, um botão e um campo, todos no visual do DESIGN.md, e a tela NÃO tenta consultar o serviço.
2. **Given** `frontend/DESIGN.md`, **When** um revisor compara cores, tipos, espaçamento e regras do/don’t, **Then** o arquivo é a fonte da verdade visual (shell com menu `{colors.sidebar}` `#000000`, área `{colors.page}` `#09090b`, item ativo em lavagem violeta, acento `{colors.primary}` `#7c3aed`, Inter, grade de 4px, dark-only, card-painel com `{shadow.card}`).
3. **Given** o README de `frontend/` com cobertura mínima declarada, **When** a suíte em `frontend/tests/` roda, **Then** os testes passam, usam a convenção de nomes e a cobertura atende o README.

---

### Edge Cases

- Se alguém clonar o repo sem o SDK .NET 10, o README de `backend/` DEVE dizer qual SDK usar e o que acontece (não constrói) — não improvisar outro SDK.
- Se a versão do React no lockfile não for a estável mais recente no momento da implementação, a implementação DEVE atualizar para essa estável (hoje: 19.3) e registrar a versão no README.
- Pastas extras na raiz (`apps/`, `packages/`, ferramentas de monorepo) NÃO DEVEM ser criadas nesta feature.
- Camadas vazias de Domain/Application/Infrastructure NÃO DEVEM ser criadas agora: não há regra de negócio para elas (YAGNI).
- Funcionalidade de estacionamento (vagas, tickets, pagamento) NÃO DEVE entrar nesta feature.
- Se o serviço não estiver em execução, abrir o endereço de vida NÃO DEVE parecer sucesso: a pessoa não recebe a resposta de “estou vivo”.
- O endereço de vida NÃO DEVE exigir login nem devolver regra de negócio.
- Com o backend desligado, a tela inicial do frontend AINDA DEVE carregar; falha de rede para o serviço NÃO DEVE aparecer nesta feature, porque não há consulta.
- A vitrine NÃO DEVE virar catálogo completo de componentes nem fluxo de estacionar: só título, card, botão e campo.
- Botão e campo da vitrine NÃO DEVEM persistir dados nem chamar o backend; podem ter estado visual (foco, pressionado, desabilitado) só para demonstrar o DESIGN.md.
- Identificadores de código em pt-BR (tipos, funções, variáveis) NÃO atendem o README desta fundação; docs, textos da tela e nomes de teste continuam em pt-BR.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O repositório DEVE ser um monorepo com exatamente dois projetos de aplicação na raiz: `backend/` e `frontend/`.
- **FR-002**: `backend/` DEVE conter `src/`, `tests/` e `README.md`.
- **FR-003**: `frontend/` DEVE conter `src/`, `tests/`, `README.md` e `DESIGN.md`.
- **FR-004**: Cada README DEVE estar em pt-BR e documentar: propósito do projeto, stack, como construir, executar e testar, que identificadores de código são em inglês, cobertura mínima de testes e a convenção de nomes de teste `<método>_<cenário>_<resultado esperado>` em pt-BR.
- **FR-005**: O backend DEVE ser desenvolvido com SDK .NET 10. Nenhuma outra versão de SDK DEVE ser a base do projeto.
- **FR-006**: O frontend DEVE ser desenvolvido com React na versão estável mais recente do canal Latest no momento da implementação. Na data desta spec, essa versão é React 19.3.
- **FR-007**: `frontend/DESIGN.md` DEVE ser a fonte da verdade visual da interface (cores, tipografia, espaçamento, raio, elevação, componentes base, do’s e don’ts).
- **FR-008**: A tela inicial DEVE ser o shell de console do DESIGN.md (menu lateral + área de trabalho) com exatamente estes elementos no main: título, card, botão e campo (fundo `{colors.page}`, menu `{colors.sidebar}`, título `{colors.ink}`, acento único `{colors.primary}`, item de menu ativo em lavagem violeta, card-painel `{colors.surface}` com `{radius.card}` e `{shadow.card}`, botão e campo com `{radius.control}`). NÃO DEVE omitir nenhum desses quatro elementos nem introduzir segundo acento de marca, tema claro, gradiente decorativo, rota extra ou módulos de negócio no menu.
- **FR-009**: Com o serviço em execução, um colega DEVE abrir um endereço documentado no README de `backend/` e receber uma resposta simples de que o serviço está vivo. Esse endereço NÃO DEVE exigir autenticação nem expor regra de estacionamento. Confirmação só no terminal NÃO atende este requisito.
- **FR-010**: Backend e frontend DEVEM ter suíte de testes em `tests/`. Cobertura DEVE cumprir o mínimo declarado no README de cada projeto. Nomes de teste DEVEM estar em pt-BR e seguir `<método>_<cenário>_<resultado esperado>`.
- **FR-011**: Identificadores de código (tipos, funções, variáveis, arquivos de código) DEVEM estar em inglês. Os READMEs de `backend/` e `frontend/` DEVEM declarar essa regra local, como a constitution permite. README, comentários de negócio e textos da interface DEVEM permanecer em pt-BR. `DESIGN.md` é a fonte visual vigente (tokens e do’s/don’ts).
- **FR-012**: Textos visíveis na interface DEVEM estar em pt-BR.
- **FR-013**: Esta feature NÃO DEVE adicionar autenticação, persistência, pagamento, mapa de vagas nem biblioteca compartilhada entre backend e frontend.
- **FR-014**: A tela inicial do frontend NÃO DEVE consultar o backend (nem o endereço de vida). A confirmação de que o serviço está vivo permanece só pelo endereço documentado no README de `backend/`.

### Key Entities

- **Projeto backend**: Unidade do monorepo que concentra o serviço. Artefatos visíveis: código em `src/`, testes em `tests/`, padrões em `README.md`.
- **Projeto frontend**: Unidade do monorepo que concentra a interface. Artefatos visíveis: código em `src/`, testes em `tests/`, padrões em `README.md`, identidade visual em `DESIGN.md`.
- **Sistema de design**: Documento de aparência da interface (`frontend/DESIGN.md`). Define paleta, tipo, espaço, elevação e o que é permitido ou proibido na UI.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Uma pessoa nova no time localiza `backend/` e `frontend/` e abre o README correspondente em menos de 1 minuto, sem ajuda.
- **SC-002**: 100% dos projetos de aplicação (`backend` e `frontend`) têm README em pt-BR com stack, comandos, regra “código em inglês” e cobertura de testes explícita.
- **SC-003**: 100% das suítes de teste dos dois projetos passam em um único comando documentado no README de cada um.
- **SC-004**: Um revisor, olhando o console (menu + título, card, botão, campo) contra o DESIGN.md, marca como atendidas pelo menos 8 regras do/don’t aplicáveis (menu 256px `{colors.sidebar}`, área `{colors.page}`, item ativo em lavagem violeta, acento `{colors.primary}`, dark-only, título de página 18px/600, item de menu 13px, corpo 14px, card-painel com `{radius.card}` e `{shadow.card}`, botão e campo `{radius.control}` com foco `{shadow.focus}`, sem pílula, sem landing centralizada, sem tema claro). Chips e métricas inventadas ficam fora desta vitrine.
- **SC-005**: As duas demonstrações desta spec são independentes: serviço vivo pelo endereço do README, e tela inicial só com identidade visual, sem o outro projeto ligado.
- **SC-006**: Com o serviço no ar, 100% das tentativas de abrir o endereço de vida documentado no README devolvem a confirmação de que está vivo em menos de 3 segundos em ambiente local.

## Assumptions

- A pasta FRONTEND pedida pelo usuário fica em `frontend/` na raiz, irmã de `backend/` (o texto original não prefixou `frontend/`, mas o monorepo com dois projetos exige essa raiz).
- “Fundações” inclui projetos que constroem, sobem e testam — não pastas vazias só com README.
- Não há ferramenta de monorepo (workspaces, Nx, Turborepo) nesta feature. Dois projetos lado a lado são suficientes (KISS / YAGNI).
- Não há solução compartilhada nem pacote `packages/` nesta feature.
- Cobertura mínima padrão, até o README definir outro número: 80% de linhas em cada projeto.
- Não há persistência, autenticação nem API de negócio nesta feature. O único endereço público desta fundação é o de “estou vivo”.
- A tela inicial do frontend é uma vitrine curta (título, card, botão, campo) da identidade visual, não um fluxo de estacionar, e NÃO consulta o backend nesta feature.
- React 19.3 é a versão estável Latest na data da spec ([anúncio React 19.3](https://react.dev/blog/2026/09/09/react-19-3)). Se o Latest mudar antes da implementação, usa-se o Latest novo e o README registra a versão.
- O DESIGN.md é a fonte da verdade visual; a vitrine NÃO DEVE divergir de tokens (cores, tipos, espaços, raios, sombras) nem das regras do/don’t.
- Identificadores de código (tipos, funções, variáveis) são em inglês, declarado nos READMEs; documentação, UI e nomes de teste são em pt-BR.
- Apps nativos (iOS/Android) estão fora de escopo.
- CI/CD automatizado nesta feature está fora de escopo; os comandos precisam existir no README para um humano rodar localmente.
