# Contrato de interface (UI)

SPA React autenticada. Textos em pt-BR. O ciclo de um veículo no pátio chama-se **estadia** na UI. Tokens: `frontend/DESIGN.md` (`tokens.css`). A vitrine da fundação **não** é mais a tela inicial.

## Shell

- `{component.global-nav}` preta, 44px, links `{typography.nav-link}`. Em ≤ 833px: hamburger.
- Itens: Operação, Vagas, Tarifas, Caixa, Histórico, Relatórios, Usuários.
- Operador **não vê** Vagas, Tarifas, Relatórios, Usuários. Digitar o endereço dessas rotas mostra recusa (sem executar a ação).
- Auditoria não é item extra do topo: vive em Relatórios, só Administrador.
- Action Blue só em CTA. Sem segundo acento, sem sombra em chrome, sem gradiente.

## Rotas

| Rota | Perfis | Tela |
|------|--------|------|
| `/login` | público | login e senha |
| `/operacao` | Admin, Operador | painel + entrada + lista de estacionados |
| `/vagas` | Admin | setores, vagas, lote, bloqueio/manutenção |
| `/tarifas` | Admin | tabelas tarifárias |
| `/caixa` | Admin, Operador | abrir/fechar, totais; sangria/suprimento/ajuste só Admin |
| `/historico` | Admin, Operador | filtros de estadias concluídas |
| `/relatorios` | Admin | caixa, entradas/saídas, exportar, auditoria |
| `/usuarios` | Admin | CRUD de status/perfil |

Não autenticado em rota interna → `/login`.

## Estados visuais de vaga

Usar tokens existentes, não verde/vermelho de marca:

| Status | Tratamento |
|--------|------------|
| Livre | superfície canvas + hairline + rótulo “Livre” |
| Ocupada | tile escuro + `{colors.on-dark}` + rótulo “Ocupada” |
| Bloqueada | tinta muted + rótulo “Bloqueada” |
| Manutenção | parchment + rótulo “Manutenção” |

## Confirmações obrigatórias

Saída e pagamento; fechamento de caixa; alteração de tarifa; reabertura de caixa; cancelamento de estadia ativa.

## Telas

### Login

Campos login e senha. Erro genérico se inválido ou inativo (sem revelar se o login existe). Sucesso: `/operacao` e cookie de sessão.

### Operação (painel)

Destaque: total cadastrado; ocupadas, livres, bloqueadas, manutenção; taxa geral e por setor; caixa aberto/fechado; total recebido no dia.

Lista de estacionados: placa, tipo, setor, vaga, entrada, permanência (relógio local a partir de `entryAt`), ação “Registrar saída”. Busca por placa; filtros setor, vaga, tipo.

Formulário de entrada: placa, tipo, setor (só compatíveis), vaga (manual ou “sugerir”). Sem vaga ou sem tarifa: mensagem que impede gravar.

Vazio: “Nenhum veículo estacionado.”

### Saída

Resumo completo da spec; forma de pagamento obrigatória; comprovante imprimível após confirmar. Sem caixa aberto ou sem estadia ativa: recusa clara.

### Vagas / Tarifas / Usuários / Caixa / Histórico / Relatórios

Cadastros e listas da spec; estados vazios informativos; lote de vagas com prefixo e intervalo; tarifas não se excluem depois de usadas.

Relatórios: visualizar na tela, imprimir, baixar PDF e CSV (mesmos totais). Auditoria: data, hora, usuário, ação, dados alterados.

## Acessibilidade mínima

Alvos de toque nas ações essenciais em celular. Rótulos nos campos. Nav hamburger no recorte tablet/telefone do DESIGN.md.
