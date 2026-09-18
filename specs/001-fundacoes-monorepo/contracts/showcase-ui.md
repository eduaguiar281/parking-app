# Contrato da vitrine (UI)

Tela única, sem roteamento. Não chama o backend. Textos em pt-BR.
Chrome de console: menu à esquerda, área de trabalho à direita.

## Elementos obrigatórios

| Elemento | Papel acessível | Tokens do DESIGN.md |
|----------|-----------------|---------------------|
| Menu | `complementary` / `aside` com nome “Menu” | largura `{layout.sidebar}` 256px; fundo `{colors.sidebar}` `#000000`; borda direita `{colors.border}` |
| Marca | texto no menu | “Parking App” + tag “Console”; marca 32px `{colors.primary}` |
| Item de menu | `button` com `aria-current="page"` | “Vitrine”; 13px; ativo `{colors.nav-active-bg}` + `{colors.nav-active-border}` + `{colors.nav-active-ink}` |
| Título | `heading` nível 1 | `{typography.title}` (Inter 18px / 600); “Vitrine”; tinta `{colors.ink}`; no topbar da área `{colors.page}` `#09090b` |
| Card | agrupamento visível (ex. `article`) | painel `{colors.surface}`, borda `{colors.border-subtle}`, raio `{radius.card}` 12px, `{shadow.card}` inset |
| Botão | `button` “Saiba mais” | fundo `{colors.primary}` `#7c3aed`, texto `{colors.on-primary}`, raio `{radius.control}` 8px, altura 40px; hover `{colors.primary-hover}`; foco `{shadow.focus}` |
| Campo | `textbox` com rótulo | fundo `{colors.surface-solid}`, texto `{colors.ink-field}`, raio `{radius.control}` 8px; rótulo `{colors.ink-muted}` envolvendo o controle; foco `{shadow.focus}` |

## Comportamento

- Foco e hover só mudam o visual (CSS). Sem persistência. Sem navegação real.
- Sem segundo acento de marca. Cores semânticas não aparecem como métricas inventadas.
- Sem gradiente decorativo. Sem pílula no botão ou no campo. Sem `scale` no ativo.
- Dark-only. Sem card parchment/branco. Sem layout de landing (conteúdo centralizado em 720px).
- Sem chips, sem módulos extras no menu, sem copiar textos do console de referência.

## Aceite visual (SC-004)

Revisor confere as regras do/don’t aplicáveis ao shell e a título, card, botão e campo contra `frontend/DESIGN.md`.
