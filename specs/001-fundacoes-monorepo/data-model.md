# Modelo de dados: Fundações do monorepo

Esta feature **não persiste** entidades de negócio. Não há banco, arquivo de estado nem sessão.

## Entidades de runtime (transitórias)

### AliveStatus

Resposta do endereço de vida. Não é gravada.

| Campo | Tipo | Regras |
|-------|------|--------|
| texto | string | Valor exato: `estou vivo`. Sem campos extras. |

- **Identidade:** não há. Toda resposta bem-sucedida é o mesmo texto.
- **Ciclo de vida:** existe só na resposta HTTP 200. Se o processo não está no ar, não há representação.
- **Relacionamentos:** nenhum.

### ShowcaseView

Estado visual da tela inicial. Não é gravado.

| Campo | Tipo | Regras |
|-------|------|--------|
| titulo | texto visível | pt-BR, `{typography.title}` no topbar (“Vitrine”) |
| card | conteúdo do card | pt-BR, painel `{colors.surface}` com `{radius.card}` no main |
| botao | rótulo | pt-BR; estados visuais CSS (foco/pressionado/desabilitado) permitidos |
| campo | valor local do input | opcional; some ao recarregar; não envia a lugar nenhum |

- **Identidade:** uma única tela, sem rotas extras nesta feature.
- **Ciclo de vida:** monta com o app; desmonta ao fechar. Sem transição de “salvo”.
- **Relacionamentos:** nenhum com AliveStatus (a UI não consulta o serviço).

## Entidades de repositório (não são dados de aplicação)

Documentadas na spec; não viram tabelas.

- **Projeto backend:** `backend/` com `src/`, `tests/`, `README.md`.
- **Projeto frontend:** `frontend/` com `src/`, `tests/`, `README.md`, `DESIGN.md`.
- **Sistema de design:** `frontend/DESIGN.md` (tokens visuais da vitrine).

## Validação

- AliveStatus: status 200 e corpo exatamente `estou vivo`.
- ShowcaseView: os quatro elementos visíveis; zero chamadas de rede ao backend.
- Nenhuma validação de e-mail, CPF, vaga ou pagamento.

## Fora de escopo

Ticket, vaga, usuário, pagamento, preferências.
