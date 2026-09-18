# Contrato da vitrine (UI)

Tela única, sem roteamento. Não chama o backend. Textos em pt-BR.

## Elementos obrigatórios

| Elemento | Papel acessível | Tokens do DESIGN.md |
|----------|-----------------|---------------------|
| Título | `heading` nível 1 | `{typography.hero-display}` (Inter/SF Pro Display 56px / 600, tracking negativo); tinta `{colors.on-dark}` `#ffffff`; fundo da página `{component.product-tile-dark}` / `{colors.surface-tile-1}` `#272729` |
| Card | agrupamento visível (ex. `article`) | `{component.store-utility-card}` com fill `{colors.canvas-parchment}` `#f5f5f7`, borda `{colors.hairline}` `#e0e0e0`, raio `{rounded.lg}` 18px, padding `{spacing.lg}` 24px, sem sombra |
| Botão | `button` | `{component.button-primary}`: fundo `{colors.primary}` `#0066cc`, texto `{colors.on-primary}` `#ffffff`, raio `{rounded.pill}`; ativo `scale(0.95)`; foco outline 2px `{colors.primary-focus}` `#0071e3` (Action Blue também no tile escuro) |
| Campo | `textbox` com rótulo | `{component.search-input}`: fundo `{colors.canvas}`, texto `{colors.ink}`, raio `{rounded.pill}`, altura 44px, borda `rgba(0, 0, 0, 0.08)`; rótulo `{colors.ink}` no card claro; foco outline 2px `{colors.primary-focus}` |

## Comportamento

- Foco e pressionado só mudam o visual (CSS). Sem persistência.
- Sem segundo acento de cor. Sem sombra no card, no botão ou no texto.
- Sem gradiente decorativo. Sem arredondar o tile de página (`{rounded.none}`); o card usa `{rounded.lg}`; o campo e o botão usam `{rounded.pill}`.
- Sem chips nesta vitrine.

## Aceite visual (SC-004)

Revisor confere as regras do/don’t aplicáveis a título, card, botão e campo contra `frontend/DESIGN.md`.
