# Feature Specification: Gestão de estacionamento

**Feature Branch**: `002-gestao-estacionamento`

**Created**: 2026-09-18

**Status**: Draft

**Input**: User description: "Sistema profissional de gestão de um estacionamento: setores, vagas, tarifas, entrada e saída, pagamentos, caixa diário, usuários, histórico, auditoria e relatórios, em pt-BR com valores em R$."

## Clarifications

### Session 2026-09-18

- Q: Quando existem ao mesmo tempo uma tarifa ativa sem setor (vale para qualquer pátio) e outra ativa só para um setor, as duas para o mesmo tipo de veículo, qual deve ser aplicada na entrada daquele setor? → A: A tarifa específica do setor é a aplicada; a geral só entra se não houver uma do setor para aquele tipo.
- Q: Se o caixa de ontem ainda estiver aberto, o sistema pode abrir o caixa de hoje ao mesmo tempo? → A: Só pode haver um caixa aberto por vez; é preciso fechar o anterior antes de abrir o da nova data.
- Q: No fechamento, como o sistema deve calcular o valor esperado do caixa? → A: Esperado = valor de abertura + receitas de saídas pagas + suprimentos − sangrias, com ajustes somados ou subtraídos conforme o sinal.
- Q: Depois que um estacionamento já foi pago e encerrado, o administrador pode corrigir esse atendimento? → A: Atendimento já pago é imutável (valor e forma de pagamento). Só administrador cancela estacionamento ativo ainda não pago, com motivo.
- Q: Se dois operadores tentarem ocupar a mesma vaga livre ao mesmo tempo, o que deve acontecer? → A: A primeira entrada confirmada ocupa a vaga; a segunda é recusada com mensagem de que a vaga já não está livre.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Entrar no sistema com o perfil correto (Priority: P1)

Um administrador ou um operador informa login e senha e acessa apenas as funções do seu perfil. Usuário inativo não entra. Cada ação posterior fica associada a quem atendeu.

**Why this priority**: Sem autenticação e sem perfil, não há responsável pelo atendimento, nem restrição entre operação e administração.

**Independent Test**: Criar um administrador e um operador, entrar com cada um e confirmar menus e bloqueios; tentar entrar com usuário inativo e senha errada.

**Acceptance Scenarios**:

1. **Given** um usuário ativo com senha válida, **When** informa login e senha corretos, **Then** entra no sistema e vê somente os módulos permitidos ao perfil.
2. **Given** um operador autenticado, **When** tenta abrir gestão de setores, vagas, tarifas, usuários, relatórios completos ou auditoria (inclusive pelo endereço direto), **Then** o acesso é recusado e a ação administrativa não é executada.
3. **Given** um usuário inativo, **When** tenta entrar, **Then** o acesso é recusado com mensagem clara, sem revelar se o login existe.
4. **Given** login ou senha inválidos, **When** tenta entrar, **Then** vê mensagem de erro clara e não entra.

---

### User Story 2 - Configurar setores e vagas (Priority: P1)

O administrador cadastra setores (nome, código, descrição opcional, categorias permitidas e status) e vagas enumeradas vinculadas a um setor. Pode gerar vagas em lote (ex.: P-A-001 até P-A-100). Cada vaga tem código único, categoria permitida e status livre, ocupada, bloqueada ou em manutenção.

**Why this priority**: Sem mapa de setores e vagas não há onde estacionar nem o que o painel contar.

**Independent Test**: Cadastrar setores de exemplo (pátio de motos, pátio de carros, coberto, visitantes), criar vagas avulsas e em lote, inativar um setor e bloquear uma vaga, e conferir que só vagas compatíveis e livres entram na disponibilidade.

**Acceptance Scenarios**:

1. **Given** um administrador autenticado, **When** cadastra um setor com nome, código identificador, categorias (Carro, Motocicleta ou ambas) e status ativo, **Then** o setor fica disponível para novas entradas compatíveis.
2. **Given** um setor ativo, **When** cadastra uma vaga com código único e categoria permitida compatível com o setor, **Then** a vaga nasce livre e entra na quantidade disponível.
3. **Given** um setor com prefixo de código, **When** gera vagas em lote de um número inicial até um final (ex.: P-A-001 a P-A-100), **Then** todas as vagas do intervalo são criadas com código único, sem colidir com códigos já existentes.
4. **Given** uma vaga livre, **When** o administrador marca bloqueada ou em manutenção, **Then** ela deixa de ser atribuível e deixa de contar como disponível.
5. **Given** um setor inativo, **When** um operador tenta usá-lo em uma nova entrada, **Then** o setor não aparece como opção válida.

---

### User Story 3 - Configurar tabelas tarifárias (Priority: P1)

O administrador cadastra, edita, ativa, inativa e consulta tabelas tarifárias por tipo de veículo e, quando necessário, por setor. Cada tabela tem valor da primeira hora, valor da hora adicional iniciada, vigência e status. Tarifas já usadas não são excluídas.

**Why this priority**: A entrada exige tarifa ativa compatível; a cobrança não pode estar fixa e invisível.

**Independent Test**: Criar tarifas de carro e motocicleta com os valores de exemplo, inativar uma, tentar excluir uma já utilizada e conferir a consulta da vigência.

**Acceptance Scenarios**:

1. **Given** um administrador autenticado, **When** cadastra uma tabela com nome, tipo de veículo, setor opcional, primeira hora, hora adicional, início de vigência e status ativa, **Then** a tabela passa a valer para novas entradas compatíveis na vigência.
2. **Given** uma tabela ativa, **When** o administrador a inativa ou encerra a vigência, **Then** novas entradas deixam de usá-la e o histórico antigo permanece intacto.
3. **Given** uma tabela já aplicada em algum atendimento, **When** o administrador tenta excluí-la, **Then** a exclusão é recusada; apenas inativação é permitida.
4. **Given** duas tabelas ativas com a mesma abrangência (mesmo tipo e mesmo setor, inclusive “qualquer setor”) e vigência sobreposta, **When** tenta ativar a segunda, **Then** a ativação é recusada com mensagem de conflito.
5. **Given** uma tarifa ativa geral (sem setor) e outra ativa do mesmo tipo só para um setor, ambas vigentes, **When** registra entrada nesse setor, **Then** aplica-se a tarifa específica do setor, não a geral.

---

### User Story 4 - Abrir e controlar o caixa do dia (Priority: P1)

O operador ou o administrador abre o caixa do dia operacional com valor de abertura. Enquanto o caixa está aberto, pagamentos e movimentações (sangria, suprimento, ajuste — só administrador) entram nele. No fechamento, o sistema mostra o valor esperado, o informado e a diferença. Só administrador reabre caixa fechado, com motivo.

**Why this priority**: Saída paga exige caixa aberto; sem caixa diário não há controle financeiro do expediente.

**Independent Test**: Abrir o caixa do dia, registrar uma sangria e um suprimento (como administrador), fechar conferindo totais e diferença, e tentar abrir um segundo caixa enquanto o primeiro ainda está aberto (mesma data ou data seguinte).

**Acceptance Scenarios**:

1. **Given** não há caixa aberto no sistema, **When** um usuário autorizado abre o caixa com valor de abertura, **Then** o caixa fica aberto, com data e hora de abertura e responsável registrados.
2. **Given** já existe um caixa aberto (da mesma data ou de data anterior), **When** alguém tenta abrir outro, **Then** a abertura é recusada até o caixa atual ser fechado.
3. **Given** um caixa aberto, **When** um administrador registra sangria, suprimento ou ajuste com valor e motivo, **Then** a movimentação entra no caixa com data, hora e responsável, e os totais são atualizados.
4. **Given** um caixa aberto com movimentações, **When** o responsável fecha informando o valor de fechamento e observações, **Then** o sistema grava valor esperado (abertura + receitas de saídas + suprimentos − sangrias ± ajustes), valor informado, diferença, data, hora e responsável, e impede novos pagamentos nesse caixa.
5. **Given** um caixa fechado, **When** um operador tenta reabrir ou alterar movimentações, **Then** a ação é recusada.
6. **Given** um caixa fechado, **When** um administrador tenta editar uma receita de saída, **Then** a alteração é recusada; ele só reabre com motivo e lança sangria, suprimento ou ajuste.

---

### User Story 5 - Registrar entrada de veículo (Priority: P1)

O operador informa placa, tipo (Carro ou Motocicleta), setor e vaga. A data e hora de entrada são preenchidas automaticamente. A placa é normalizada e validada. Só entram setores e vagas compatíveis. A vaga pode ser escolhida ou sugerida. Sem vaga compatível, sem tarifa ativa compatível ou com placa já estacionada, a entrada é bloqueada.

**Why this priority**: É o fluxo principal do pátio: ocupar exatamente uma vaga compatível e iniciar o atendimento.

**Independent Test**: Com setor, vaga livre e tarifa ativa, registrar uma entrada válida; repetir a mesma placa; tentar moto em setor só de carro; tentar entrada sem tarifa.

**Acceptance Scenarios**:

1. **Given** setor ativo, vaga livre compatível e tarifa ativa compatível, **When** o operador registra placa válida, tipo, setor e vaga (manual ou sugerida), **Then** a vaga fica ocupada, o veículo aparece como estacionado, o responsável é gravado e o painel atualiza na hora.
2. **Given** uma placa com estacionamento ativo, **When** tenta nova entrada, **Then** a entrada é recusada com mensagem clara.
3. **Given** um tipo de veículo sem vaga compatível livre, **When** tenta registrar a entrada, **Then** a entrada é recusada e nenhuma vaga é ocupada.
4. **Given** um tipo e setor sem tarifa ativa vigente, **When** tenta concluir a entrada, **Then** a entrada é recusada com mensagem explicando a ausência de tarifa.
5. **Given** placa em minúsculas ou com formato antigo/Mercosul válido, **When** registra a entrada, **Then** a placa é gravada em maiúsculas e aceita nos formatos `ABC-1234` e `ABC1D23`.
6. **Given** dois operadores confirmando entrada na mesma vaga livre ao mesmo tempo, **When** a primeira confirmação conclui, **Then** a vaga fica com esse veículo e a segunda entrada é recusada com mensagem de que a vaga já não está livre.

---

### User Story 6 - Registrar saída e pagamento (Priority: P1)

O operador escolhe um veículo estacionado, vê o resumo (placa, tipo, setor, vaga, entrada, saída, permanência, tarifa, valor) e confirma a forma de pagamento. A vaga é liberada, o atendimento encerrado, o caixa do dia recebe a receita e um comprovante simples é gerado.

**Why this priority**: Encerra o ciclo operacional e gera a receita do dia.

**Independent Test**: Estacionar um veículo, abrir o caixa, registrar a saída com cada regra de cobrança de exemplo e conferir vaga livre, caixa e comprovante.

**Acceptance Scenarios**:

1. **Given** um veículo com estacionamento ativo e caixa aberto, **When** o operador solicita a saída, **Then** vê o resumo completo e só conclui após confirmar a forma de pagamento (Dinheiro, Pix, Cartão de débito, Cartão de crédito ou Outro).
2. **Given** o resumo conferido, **When** confirma a saída, **Then** grava saída, permanência, valor, forma de pagamento e tarifa aplicada, libera a vaga, registra a receita no caixa aberto, registra o responsável e oferece o comprovante.
3. **Given** um veículo sem estacionamento ativo, **When** tenta registrar saída, **Then** a ação é recusada com mensagem clara.
4. **Given** estacionamento ativo sem caixa aberto, **When** tenta registrar saída, **Then** a saída é recusada até existir caixa aberto.
5. **Given** as regras de hora cheia, **When** cobra os exemplos (carro 30 min = primeira hora; carro 1h01 = primeira + uma adicional; carro 2h30 = primeira + duas adicionais; moto 45 min = primeira hora; moto 2h10 = primeira + duas adicionais), **Then** os valores batem com a tarifa gravada na entrada.

---

### User Story 7 - Acompanhar o painel operacional (Priority: P1)

O operador vê totais de vagas, ocupadas, livres, bloqueadas e em manutenção; taxa de ocupação geral e por setor; veículos estacionados; situação do caixa do dia; e total recebido no dia. A lista mostra placa, tipo, setor, vaga, entrada, permanência atualizada e ação de saída, com busca por placa e filtros.

**Why this priority**: É a tela do expediente: sem ela o operador não enxerga o pátio nem o caixa.

**Independent Test**: Com algumas vagas em cada status e veículos estacionados, abrir o painel, filtrar por placa/setor/vaga/tipo e disparar a saída a partir da lista.

**Acceptance Scenarios**:

1. **Given** vagas e estacionamentos cadastrados, **When** o operador abre o painel, **Then** vê totais, ocupação, lista de estacionados, caixa aberto ou fechado e total recebido no dia, todos coerentes com o estado atual.
2. **Given** veículos estacionados, **When** busca por placa ou filtra por setor, vaga ou tipo, **Then** a lista mostra só os registros que atendem o critério.
3. **Given** um veículo na lista, **When** aciona registrar saída, **Then** inicia o fluxo de saída da User Story 6.
4. **Given** o tempo passando, **When** o operador permanece no painel, **Then** o tempo de permanência de cada veículo estacionado permanece atualizado.

---

### User Story 8 - Gerenciar usuários (Priority: P2)

O administrador cria, edita, inativa e reativa usuários com nome completo, login (usuário ou e-mail), senha, perfil (Administrador ou Operador) e status. O operador não gerencia usuários.

**Why this priority**: Necessário para operar com mais de um atendente, mas o pátio já funciona com o administrador inicial.

**Independent Test**: Criar um operador, inativá-lo, confirmar que não entra, reativá-lo e confirmar que um operador não acessa essa tela.

**Acceptance Scenarios**:

1. **Given** um administrador autenticado, **When** cria um usuário com nome, login, senha, perfil e status ativo, **Then** o novo usuário consegue entrar com essas credenciais.
2. **Given** um usuário existente, **When** o administrador o inativa, **Then** esse usuário deixa de entrar; ao reativar, volta a entrar.
3. **Given** um operador autenticado, **When** tenta criar, editar, inativar ou reativar usuários, **Then** a ação é recusada.

---

### User Story 9 - Consultar histórico de estacionamentos (Priority: P2)

Qualquer perfil autorizado consulta estacionamentos finalizados com placa, tipo, setor, vaga, entrada, saída, permanência, tarifa e valores aplicados, forma de pagamento, caixa e responsável. Filtros: período, placa, setor, vaga, tipo, forma de pagamento e operador.

**Why this priority**: Fecha o ciclo de conferência operacional depois que entradas e saídas já existem.

**Independent Test**: Finalizar alguns atendimentos distintos e filtrar o histórico até isolar cada combinação.

**Acceptance Scenarios**:

1. **Given** estacionamentos finalizados, **When** o usuário abre o histórico sem filtro, **Then** vê todos os atendimentos concluídos com os campos acima, inclusive a tarifa efetivamente aplicada.
2. **Given** o histórico, **When** aplica filtros de período, placa, setor, vaga, tipo, forma de pagamento ou operador, **Then** só aparecem registros que satisfazem todos os filtros informados.
3. **Given** uma tarifa ou vaga alterada depois do atendimento, **When** consulta o histórico daquele atendimento, **Then** os dados gravados na época permanecem, sem herdar a alteração posterior.

---

### User Story 10 - Emitir relatórios operacionais e financeiros (Priority: P2)

O administrador visualiza, imprime e exporta (PDF e CSV) o relatório de caixa e o relatório de entradas e saídas, com totais e detalhamento. O operador não acessa relatórios completos.

**Why this priority**: Gestão e conferência; a operação do pátio já funciona sem exportar.

**Independent Test**: Gerar os dois relatórios para um período com movimentações, imprimir a visualização e exportar PDF e CSV, conferindo totais.

**Acceptance Scenarios**:

1. **Given** caixas com abertura, receitas, sangrias, suprimentos, ajustes e fechamento, **When** o administrador emite o relatório de caixa, **Then** vê status, responsáveis, abertura, receitas, totais por forma de pagamento, movimentações especiais, esperado, informado, diferença e o detalhamento de cada movimento.
2. **Given** entradas e saídas no período, **When** emite o relatório de entradas e saídas, **Then** vê a lista de atendimentos e os totais: quantidade de entradas, de saídas, veículos estacionados no momento, receita total, receita por tipo, receita por setor, média de permanência e taxa de ocupação por setor.
3. **Given** um relatório na tela, **When** solicita impressão ou exportação, **Then** obtém visualização imprimível e arquivos PDF e CSV com o mesmo conteúdo essencial.
4. **Given** um operador autenticado, **When** tenta abrir relatórios completos, **Then** o acesso é recusado.

---

### User Story 11 - Consultar auditoria (Priority: P3)

O administrador consulta o rastro de ações relevantes: mudanças cadastrais, caixa, movimentações especiais, entradas, saídas, pagamentos e cancelamentos de estacionamento ativo. Cada evento tem data, hora, responsável, ação e dados alterados. O operador não consulta auditoria.

**Why this priority**: Rastreabilidade administrativa; a operação do dia já funciona se os eventos estiverem sendo gravados, mesmo antes da tela de consulta.

**Independent Test**: Executar uma mudança cadastral, uma abertura de caixa e uma saída, abrir a auditoria e localizar os três eventos com responsável e dados.

**Acceptance Scenarios**:

1. **Given** ações relevantes já realizadas, **When** o administrador abre a auditoria, **Then** cada evento lista data, hora, usuário, ação e dados alterados.
2. **Given** um operador autenticado, **When** tenta consultar a auditoria, **Then** o acesso é recusado.
3. **Given** um administrador cancela um estacionamento ativo por erro de lançamento (com motivo), **When** consulta a auditoria, **Then** o cancelamento aparece com responsável, motivo e dados anteriores.
4. **Given** um estacionamento já pago, **When** alguém tenta alterar valor ou forma de pagamento, **Then** a alteração é recusada e o histórico e o caixa permanecem iguais.

---

### Edge Cases

- Pátio sem vaga compatível livre: a entrada é recusada; nenhuma vaga muda de status.
- Placa inválida (fora dos formatos brasileiro antigo e Mercosul): a entrada é recusada antes de ocupar vaga.
- Placa já com estacionamento ativo: nova entrada é recusada.
- Setor ou vaga incompatível com o tipo de veículo: não aparece como opção e não pode ser forçado.
- Vaga ocupada, bloqueada ou em manutenção: não pode ser atribuída.
- Duas entradas confirmadas na mesma vaga livre ao mesmo tempo: a primeira ocupa; a segunda é recusada com mensagem de que a vaga já não está livre; o segundo veículo não fica estacionado.
- Tentativa de bloquear ou colocar em manutenção uma vaga ocupada: a mudança de status é recusada até a vaga estar livre.
- Setor inativo: não recebe novas entradas; veículos já estacionados nele continuam até a saída.
- Sem tarifa ativa compatível na vigência: entrada bloqueada com mensagem explicativa.
- Tarifa geral (sem setor) e tarifa do mesmo tipo restrita a um setor, ambas ativas e vigentes: na entrada daquele setor aplica-se a específica; a geral só vale se não houver tabela daquele tipo para o setor.
- Conflito de tarifas ativas na mesma abrangência e vigência: a segunda não ativa.
- Saída sem estacionamento ativo: recusada.
- Saída sem caixa aberto: recusada.
- Qualquer tentativa de abrir um segundo caixa enquanto outro está aberto (mesma data ou data diferente): recusada até o atual ser fechado.
- Fechamento com diferença entre esperado e informado: permitido; a diferença é gravada e visível. O esperado é abertura + receitas de saídas pagas + suprimentos − sangrias ± ajustes.
- Movimentação em caixa já fechado: recusada para todos. Administrador reabre com motivo e lança sangria, suprimento ou ajuste; não edita receita de saída.
- Reabertura de caixa por operador: recusada.
- Usuário inativo ou senha inválida: não entra.
- Operador acessando endereço administrativo: recusado, sem executar a ação.
- Geração em lote com código já existente no intervalo: o lote inteiro é recusado, nada é criado pela metade.
- Permanência noturna (entra em um dia e sai no seguinte): permitida; a cobrança usa a tarifa gravada na entrada e o pagamento cai no caixa aberto no momento da saída.
- Alteração futura de tarifa, setor, vaga ou usuário: não reescreve atendimentos já concluídos.
- Exclusão de tarifa já utilizada: recusada; só inativação.
- Lista vazia (sem veículos, sem histórico, sem movimentação): mostra estado vazio informativo, não uma tela em branco.
- Dados de demonstração, se existirem: identificados de forma visível como demonstração.
- Cancelamento de estacionamento ativo (lançamento errado): apenas administrador, com motivo obrigatório; a vaga é liberada e o evento vai para a auditoria. Atendimento já pago não é apagado nem alterado (valor e forma de pagamento permanecem).

## Requirements *(mandatory)*

### Functional Requirements

#### Idioma, formato e navegação

- **FR-001**: Toda a interface visível ao usuário DEVE estar em português do Brasil.
- **FR-002**: Valores monetários DEVEM ser exibidos em reais (R$) no formato brasileiro.
- **FR-003**: Datas DEVEM ser exibidas no formato brasileiro e horários no horário local do estabelecimento.
- **FR-004**: A navegação principal DEVE oferecer: Operação, Vagas, Tarifas, Caixa, Histórico, Relatórios e Usuários, exibindo a cada perfil somente o que ele pode usar.
- **FR-005**: A interface DEVE ser utilizável em desktop, tablet e celular, sem omitir as ações essenciais de cada perfil.
- **FR-006**: Ações sensíveis (saída e pagamento, fechamento de caixa, alteração de tarifa, reabertura de caixa, cancelamento de atendimento) DEVEM exigir confirmação explícita antes de concluir.
- **FR-007**: O sistema DEVE exibir mensagens de erro e de sucesso na própria interface (não só no transporte). Estados vazios DEVEM ser informativos. Esta feature NÃO semeia pátio de demonstração; se no futuro houver dados de treino, DEVEM trazer o prefixo visível “Demonstração”.
- **FR-008**: Indicadores visuais DEVEM distinguir vagas livres, ocupadas, bloqueadas e em manutenção.

#### Autenticação e permissões

- **FR-009**: O sistema DEVE autenticar por login (usuário ou e-mail) e senha. A senha NÃO DEVE ser recuperável em texto claro.
- **FR-010**: Cada usuário DEVE ter nome completo, login, senha, perfil (Administrador ou Operador), status ativo ou inativo, data de criação e último acesso.
- **FR-011**: Usuário inativo NÃO DEVE autenticar.
- **FR-012**: O perfil Administrador DEVE ter acesso a todos os módulos e ações desta spec.
- **FR-013**: O perfil Operador DEVE poder usar painel, entrada, saída e pagamento, abrir e fechar caixa, consultar movimentações do caixa e consultar histórico operacional. NÃO DEVE gerir setores, vagas, tarifas ou usuários; NÃO DEVE emitir relatórios completos; NÃO DEVE registrar sangria, suprimento ou ajuste; NÃO DEVE reabrir caixa; NÃO DEVE consultar auditoria.
- **FR-014**: Apenas administradores DEVEM criar, editar, inativar ou reativar usuários.
- **FR-015**: Toda ação operacional e administrativa DEVE registrar o usuário responsável.
- **FR-016**: Se o operador abrir o endereço de uma função administrativa (Vagas, Tarifas, Usuários, Relatórios completos, Auditoria), o acesso DEVE ser recusado sem executar a ação. A matriz do que cada perfil pode fazer está em FR-013.

#### Setores e vagas

- **FR-017**: O administrador DEVE cadastrar, editar, ativar e inativar setores com nome, código identificador, descrição opcional, categorias permitidas (Carro, Motocicleta ou ambas) e status ativo ou inativo.
- **FR-018**: O administrador DEVE cadastrar vagas individuais vinculadas a um setor, com código único, categoria de veículo permitida e status livre, ocupada, bloqueada ou em manutenção.
- **FR-019**: O administrador DEVE poder criar vagas em lote informando prefixo e intervalo numérico (ex.: de P-A-001 até P-A-100). Se qualquer código do intervalo já existir, o lote NÃO DEVE ser criado.
- **FR-020**: A categoria da vaga DEVE ser compatível com as categorias do setor.
- **FR-021**: Cada veículo estacionado DEVE ocupar exatamente uma vaga. Uma vaga ocupada NÃO DEVE ser atribuída a outro veículo. Se duas entradas na mesma vaga livre forem confirmadas ao mesmo tempo, a primeira a concluir DEVE ocupar a vaga e a segunda DEVE ser recusada com mensagem de que a vaga já não está livre.
- **FR-022**: Uma vaga só DEVE receber veículo da categoria permitida. Vagas bloqueadas ou em manutenção NÃO DEVEM ser ocupadas.
- **FR-023**: A quantidade disponível DEVE considerar somente vagas em setor ativo, com status livre e liberadas para uso (não bloqueadas nem em manutenção).
- **FR-024**: NÃO DEVE ser permitido alterar uma vaga ocupada para bloqueada ou em manutenção.

#### Tarifação

- **FR-025**: Tarifas NÃO DEVEM ser fixas e invisíveis: o administrador DEVE cadastrar, editar, ativar, inativar e consultar tabelas tarifárias.
- **FR-026**: Cada tabela DEVE ter nome, tipo de veículo, setor de aplicação (opcional), valor da primeira hora, valor por hora adicional iniciada, data de início de vigência, data de término opcional e status ativa ou inativa.
- **FR-027**: A primeira hora DEVE ser cobrada integralmente, inclusive para permanência inferior a uma hora. Cada hora adicional iniciada DEVE ser cobrada como hora completa.
- **FR-028**: A tarifa aplicada a um atendimento DEVE ser a vigente e compatível com tipo de veículo e setor no momento da entrada. Se existirem ao mesmo tempo uma tabela ativa sem setor e outra ativa do mesmo tipo restrita ao setor da entrada, DEVE-se aplicar a específica do setor; a geral só DEVE ser usada quando não houver tabela daquele tipo para aquele setor. Nome da tabela e valores aplicados DEVEM ser gravados no estacionamento.
- **FR-029**: Mudança futura de tarifa NÃO DEVE alterar cobrança já gravada. A preservação do histórico (tarifa, vaga, setor, usuário) segue FR-057.
- **FR-030**: Tarifa já utilizada NÃO DEVE ser excluída; apenas inativada.
- **FR-031**: Sem tarifa ativa compatível, a entrada DEVE ser bloqueada com mensagem explicativa.
- **FR-032**: NÃO DEVE haver duas tabelas ativas com a mesma abrangência (mesmo tipo de veículo e mesmo setor, tratando “sem setor” como abrangência própria) e vigência sobreposta.

#### Entrada, saída e pagamento

- **FR-033**: Na entrada, o operador DEVE informar placa, tipo de veículo (Carro ou Motocicleta), setor e vaga. Data e hora de entrada DEVEM ser preenchidas automaticamente.
- **FR-034**: Placas DEVEM ser normalizadas para maiúsculas e validadas nos formatos brasileiro antigo (`ABC-1234`) e Mercosul (`ABC1D23`).
- **FR-035**: A interface de entrada DEVE listar somente setores e vagas compatíveis com o tipo informado, permitir escolha manual ou sugestão automática de vaga livre compatível, e recusar a entrada se não houver vaga compatível.
- **FR-036**: NÃO DEVE haver nova entrada para placa com estacionamento ativo.
- **FR-037**: Após a entrada, status da vaga, indicadores do setor e painel DEVEM atualizar imediatamente, e o responsável DEVE ser gravado.
- **FR-038**: Antes de confirmar a saída, o sistema DEVE apresentar resumo com placa, tipo, setor e vaga, entrada, saída, permanência total, tarifa aplicada, valor total e forma de pagamento (Dinheiro, Pix, Cartão de débito, Cartão de crédito ou Outro).
- **FR-039**: Na confirmação da saída, o sistema DEVE gravar saída, permanência, valor, forma de pagamento e tarifa aplicada; registrar a receita no caixa aberto; liberar a vaga; encerrar o atendimento; gravar o responsável; e gerar comprovante simples de saída e pagamento.
- **FR-040**: NÃO DEVE haver saída para veículo sem estacionamento ativo nem para saída paga sem caixa aberto.
- **FR-041**: Formas de pagamento DEVEM ser apenas registradas no atendimento e no caixa. Esta feature NÃO exige liquidação bancária, maquininha ou QR Pix real.
- **FR-042**: Somente administrador DEVE cancelar estacionamento ativo ainda não pago por erro de lançamento, com motivo obrigatório, liberando a vaga e registrando auditoria. Atendimento já pago NÃO DEVE ser apagado nem ter valor ou forma de pagamento alterados. A movimentação de receita gerada por essa saída NÃO DEVE ser editada nem excluída.

#### Caixa diário

- **FR-043**: Deve existir um caixa por data operacional, com status aberto ou fechado, valor de abertura, data e hora de abertura, responsável pela abertura, movimentações, totais por forma de pagamento, valor de fechamento informado, valor esperado, diferença, data e hora de fechamento, responsável pelo fechamento e observações. O valor esperado DEVE ser: valor de abertura + receitas de saídas pagas + suprimentos − sangrias, com ajustes somados ou subtraídos conforme o sinal informado no lançamento. A diferença DEVE ser valor informado de fechamento menos valor esperado.
- **FR-044**: Pagamentos SÓ DEVEM ser registrados se houver caixa aberto. Cada saída paga DEVE gerar uma entrada financeira nesse caixa.
- **FR-045**: NÃO DEVE haver mais de um caixa aberto no sistema por vez. Abrir o caixa de uma nova data operacional exige fechar o caixa atualmente aberto. Também NÃO DEVE haver dois caixas para a mesma data operacional.
- **FR-046**: Administrador DEVE poder registrar sangrias, suprimentos e ajustes com valor, motivo, data, hora e responsável.
- **FR-047**: Com o caixa fechado, ninguém edita nem exclui movimentações já lançadas (incluindo receita de saída). O operador NÃO DEVE reabrir o caixa. O administrador SÓ DEVE corrigir numerário reabrindo o caixa (FR-048) e lançando sangria, suprimento ou ajuste — nunca reescrevendo a saída paga.
- **FR-048**: Reabertura de caixa fechado DEVE ser exclusiva de administrador, com motivo obrigatório e auditoria.
- **FR-049**: Totais do caixa DEVEM ser exibidos separados por forma de pagamento.

#### Histórico, relatórios e auditoria

- **FR-050**: O histórico de estacionamentos finalizados DEVE conter placa, tipo, setor e vaga, entrada, saída, permanência, tarifa e valores efetivamente aplicados, forma de pagamento, caixa relacionado e responsável, com filtros por período, placa, setor, vaga, tipo, forma de pagamento e operador.
- **FR-051**: O relatório de caixa DEVE apresentar caixa do dia, status, responsáveis, abertura, receitas totais, totais por forma de pagamento, sangrias, suprimentos e ajustes, esperado, informado, diferença e detalhamento das movimentações.
- **FR-052**: O relatório de entradas e saídas DEVE apresentar a lista de atendimentos (placa, tipo, setor e vaga, entrada, saída, permanência, valor pago, forma de pagamento, responsável) e os indicadores: quantidade de entradas, quantidade de saídas, veículos estacionados no momento, receita total, receita por tipo, receita por setor, média de permanência e taxa de ocupação por setor.
- **FR-053**: Relatórios DEVEM poder ser visualizados, impressos e exportados em PDF e CSV. Apenas o administrador acessa relatórios completos.
- **FR-054**: O sistema DEVE registrar auditoria de: criação, alteração e inativação de setores, vagas, tarifas e usuários; abertura, fechamento e reabertura de caixa; sangrias, suprimentos e ajustes; entradas, saídas e pagamentos; cancelamentos de estacionamento ativo.
- **FR-055**: Cada evento de auditoria DEVE conter data, hora, usuário responsável, ação realizada e dados alterados. A consulta da auditoria é exclusiva do administrador.

#### Persistência e imutabilidade do histórico

- **FR-056**: Usuários, setores, vagas, tarifas, entradas, saídas, pagamentos, caixas, movimentações financeiras, histórico e auditoria DEVEM ser persistidos e sobreviver ao encerramento da sessão ou do aplicativo.
- **FR-057**: Alterações posteriores em tarifas, vagas, setores ou usuários NÃO DEVEM modificar os dados já gravados em atendimentos concluídos. O atendimento DEVE conservar a tarifa e os identificadores efetivamente aplicados.
- **FR-058**: A taxa de ocupação geral e por setor DEVE ser calculada como vagas ocupadas divididas pelas vagas utilizáveis (ocupadas + livres) do recorte, excluindo bloqueadas, em manutenção e vagas de setor inativo.

### Key Entities

Termo canônico na spec e na UI: **estadia** (ciclo de um veículo no pátio; às vezes chamado atendimento). No código e no contrato HTTP: `Stay`.

- **Usuário**: Pessoa que acessa o sistema. Tem nome, login, senha, perfil (Administrador ou Operador), status, criação e último acesso.
- **Setor**: Área do pátio (ex.: Pátio A, Coberto, Visitantes). Tem nome, código, descrição opcional, categorias permitidas e status.
- **Vaga**: Unidade enumerada dentro de um setor (ex.: P-A-001). Tem código único, setor, categoria permitida e status livre, ocupada, bloqueada ou em manutenção.
- **Tabela tarifária**: Preço vigente por tipo de veículo e, se houver, por setor. Tem valores de primeira hora e hora adicional, vigência e status. Depois de usada, só pode ser inativada.
- **Estadia**: Ciclo de um veículo da entrada à saída. Ocupa uma vaga, guarda a tarifa aplicada no momento da entrada e, ao encerrar, guarda permanência, valor, forma de pagamento, caixa e responsáveis.
- **Caixa diário**: Controle financeiro de uma data operacional, com abertura, movimentações, totais por forma de pagamento e fechamento (esperado, informado e diferença).
- **Movimentação financeira**: Receita de saída paga, sangria, suprimento ou ajuste, sempre ligada a um caixa e a um responsável.
- **Evento de auditoria**: Rastro de ação relevante, com quando, quem, o quê e os dados alterados.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Um operador autenticado, com setor, vaga livre e tarifa já configurados, conclui o registro de uma entrada válida em menos de 45 segundos.
- **SC-002**: Após cada entrada ou saída, 100% dos totais do painel (ocupadas, livres, bloqueadas, manutenção, ocupação e lista de estacionados) conferem com a contagem real das vagas e dos veículos.
- **SC-003**: 100% dos exemplos de cobrança desta spec produzem o valor esperado: carro 30 min = só a primeira hora; carro 1h01 = primeira + 1 adicional; carro 2h30 = primeira + 2 adicionais; moto 45 min = só a primeira hora; moto 2h10 = primeira + 2 adicionais.
- **SC-004**: Depois de alterar ou inativar uma tabela tarifária, 100% dos atendimentos já concluídos conservam a tarifa e o valor originalmente gravados.
- **SC-005**: 100% das saídas pagas aparecem no caixa aberto do momento da saída, na forma de pagamento escolhida, e a vaga correspondente volta a livre.
- **SC-006**: Em 100% das tentativas, um operador é impedido de gerir setores, vagas, tarifas e usuários, de ver relatórios completos, de reabrir caixa e de consultar auditoria.
- **SC-007**: Um responsável fecha o caixa do dia, com esperado (abertura + receitas + suprimentos − sangrias ± ajustes), informado e diferença visíveis e conferíveis, em menos de 2 minutos quando as movimentações já estão lançadas.
- **SC-008**: 100% dos estacionamentos finalizados são localizáveis no histórico pela placa e pelo período em que ocorreram.
- **SC-009**: Um administrador gera, imprime e exporta (PDF e CSV) os relatórios de caixa e de entradas/saídas de um período em menos de 2 minutos por relatório.
- **SC-010**: As ações essenciais de cada perfil (entrar, sair/pagar, abrir/fechar caixa no operador; cadastros e relatórios no administrador) permanecem utilizáveis em desktop, tablet e celular.
- **SC-011**: Pelo menos 90% dos operadores já treinados concluem uma entrada e uma saída pagas na primeira tentativa, sem ajuda.
- **SC-012**: 100% das ações relevantes listadas em FR-054 deixam evento de auditoria com data, hora, responsável, ação e dados alterados.

## Assumptions

- Há um único estabelecimento. Multiunidade, franquia ou vários pátios como empresas distintas ficam fora desta feature.
- Os tipos de veículo são somente Carro e Motocicleta.
- A primeira hora não tem tolerância nem carência: qualquer período até 60 minutos cobra a primeira hora cheia.
- Permanência que atravessa a meia-noite é permitida. A tarifa é a gravada na entrada; o pagamento entra no caixa que estiver aberto na saída.
- A data operacional do caixa é a data de calendário informada na abertura. Só um caixa permanece aberto por vez em todo o sistema; o dia anterior precisa ser fechado antes de abrir o seguinte.
- Fechar caixa com diferença é permitido; a diferença é registrada, não bloqueia o fechamento. O esperado usa a fórmula: abertura + receitas de saídas pagas + suprimentos − sangrias ± ajustes.
- Entrada de veículo não exige caixa aberto; saída paga exige.
- Pix, cartão e “Outro” são formas de registro, sem integração com banco, maquininha ou QR real.
- Não há reserva antecipada, mensalista, convênio, cortesia, desconto, câmera de placa, cancela automática nem aplicativo do motorista.
- Comprovante e impressão de relatório usam visualização imprimível do próprio sistema; não há exigência de impressora térmica específica.
- O administrador inicial existe para o primeiro acesso (criado na implantação). Não há auto cadastro público.
- Cancelamento de estacionamento ativo é exceção administrativa (erro de lançamento), não um fluxo do operador. Atendimento já pago é imutável; diferença de numerário se resolve com sangria, suprimento ou ajuste, não editando a saída.
- Na sugestão automática, o sistema escolhe uma vaga livre compatível qualquer; o operador pode trocar antes de confirmar. Reserva temporária enquanto o formulário está aberto está fora desta feature: a ocupação só vale na confirmação.
- Tarifa geral (sem setor) e tarifa de setor do mesmo tipo podem coexistir ativas: na entrada, a do setor tem precedência.
- Formato antigo de placa aceita o padrão `ABC-1234` (hífen). O sistema normaliza letras para maiúsculas.
- A identidade visual já definida do produto permanece a referência de aparência; esta feature não inventa um segundo sistema visual.
- Não há seed de setores, vagas, tarifas ou estadias. Se no futuro existirem dados de treino, o prefixo visível é “Demonstração”. Mensagens de sucesso aparecem na tela (toast ou texto junto ao formulário), não só na resposta técnica.

### Fora de escopo

- Autoatendimento do motorista, app do cliente, totem ou pagamento pelo celular do condutor.
- Reconhecimento automático de placa, integração com cancela, sensor de vaga ou CFTV.
- Contratos mensais, credenciados, isenções e tabelas progressivas além de primeira hora + hora adicional.
- Múltiplos estabelecimentos, filiais ou moedas diferentes de R$.
- Integração bancária de Pix, adquirente de cartão ou conciliação automática.
- Recuperação de senha por e-mail, cadastro público e mais de dois perfis além de Administrador e Operador.
- Correção de valor ou forma de pagamento de atendimento já pago.
