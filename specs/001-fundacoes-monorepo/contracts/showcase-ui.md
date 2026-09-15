# Contrato da vitrine (UI)

Tela única, sem roteamento. Não chama o backend. Textos em pt-BR.

## Elementos obrigatórios

| Elemento | Papel acessível | Tokens QuestUI (DESIGN.md) |
|----------|-----------------|----------------------------|
| Título ornamental | `heading` nível 1 | Cinzel, cor pergaminho `#F5E6D3`, fundo da página `#1A0F0A` |
| Card | agrupamento visível (ex. `article`) | superfície `#2C1A10`, borda `#5C3D2E` ou acento superior `#CA8A04`, raio 4px, padding 24px |
| Botão | `button` | primário: fundo `#CA8A04`, texto `#1A0F0A`, borda `#DAA520`; glow dourado no ativo/foco |
| Campo | `textbox` com rótulo | fundo `#2C1A10`, borda `#5C3D2E`, foco borda `#CA8A04`, texto `#F5E6D3` |

## Comportamento

- Hover, foco e desabilitado só mudam o visual (CSS). Sem persistência.
- Transições ≤ 300ms.
- Sem sans-serif. Sem branco `#FFFFFF`. Sem preto puro no fundo.
- Sem chips nesta vitrine.

## Aceite visual (SC-004)

Revisor confere as regras do/don’t aplicáveis a título, card, botão e campo contra `frontend/DESIGN.md`.
