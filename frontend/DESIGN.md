# Sistema de design — QuestUI

Fonte da verdade visual da interface Parking App. Adaptado do QuestUI para **pt-BR**. Tokens não devem divergir dos valores abaixo.

Estilo ornamental inspirado em RPG: ouro, vermelho profundo e roxo real sobre marrons escuros. Peso e artesanato em cada superfície — nunca visual “flat” contemporâneo.

## Cores

- **Ouro primário** (`#CA8A04`): ações principais, destaques
- **Vermelho profundo** (`#991B1B`): acento secundário, alerta
- **Roxo real** (`#581C87`): ênfase rara, elementos especiais
- **Fundo** (`#1A0F0A`): fundo da página (nunca preto puro)
- **Superfície** (`#2C1A10`): fundo de cards
- **Superfície elevada** (`#3D2517`): painéis elevados
- **Borda** (`#5C3D2E`)
- **Ouro claro da borda** (`#DAA520`)
- **Pergaminho** (`#F5E6D3`): texto (nunca branco `#FFFFFF`)
- **Texto secundário** (`#BFA98A`)
- **Sucesso** (`#22C55E`)
- **Erro** (`#991B1B`)

## Tipografia

- **Títulos e rótulos de UI**: Cinzel
- **Corpo**: Spectral
- **Mono**: Fira Code

| Estilo | Fonte | Tamanho | Peso | Entrelinha |
|--------|--------|---------|------|------------|
| Display | Cinzel | 40px | bold | 1.15 |
| H1 | Cinzel | 32px | bold | 1.2 |
| H2 | Cinzel | 24px | semibold | 1.25 |
| H3 | Cinzel | 20px | semibold | 1.3 |
| H4 | Cinzel | 16px | regular | 1.35 |
| Corpo LG | Spectral | 18px | regular | 1.7 |
| Corpo | Spectral | 16px | regular | 1.7 |
| Corpo SM | Spectral | 14px | regular | 1.6 |
| Legenda | Spectral | 12px | medium | 1.4 |
| Código | Fira Code | 14px | regular | 1.6 |

## Espaçamento

Unidade base: **8px**.

- **xs** 4px — ícones inline
- **sm** 8px — padding apertado
- **md** 16px — padding padrão
- **lg** 24px — padding de card
- **xl** 32px — gaps de painel
- **2xl** 48px — seções
- **3xl** 64px — página

## Raio

Sensação medieval, pouco arredondado.

- **sm** 2px — badges
- **padrão** 4px — botões, cards, campos
- **md** 6px — modais
- **lg** 8px — painéis
- **full** 9999px — orbes

## Elevação (glow dourado)

- **sm**: offset 1px, blur 3px, `#CA8A04` 15% — botões
- **padrão**: offset 2px, blur 8px, `#CA8A04` 20% — cards
- **md**: offset 4px, blur 16px, `#CA8A04` 25%
- **lg**: offset 8px, blur 32px, `#CA8A04` 30%
- **glow**: blur 20px, `#CA8A04` 40% — ativo/selecionado

## Componentes desta fundação

### Botão primário

Preenchimento `#CA8A04`, texto `#1A0F0A`, borda 1px `#DAA520`. Hover `#B8780A`. Ativo/foco: glow dourado. Desabilitado: opacidade 0.35, sem glow, borda `#5C3D2E`. Tamanho md: padding 8px 22px, fonte 14px, altura 40px.

### Card

Fundo `#2C1A10`, borda 1px `#5C3D2E`, sombra sm, raio 4px, padding 24px. Acento superior 2px `#CA8A04`. Hover: glow em 300ms.

### Campo

Altura 40px, padding 8px 12px, raio 4px. Padrão: borda `#5C3D2E`, fundo `#2C1A10`. Hover: borda `#CA8A04`. Foco: borda 2px `#CA8A04` + anel 3px `#CA8A04` 25%. Rótulo: Cinzel 14px / 500, pergaminho, margem inferior 6px.

## Faça e não faça

1. **Faça** usar Cinzel em títulos e rótulos da UI.
2. **Faça** aplicar glow dourado em elementos interativos no estado ativo/foco.
3. **Faça** usar a paleta marrom em camadas (`#1A0F0A`, `#2C1A10`, `#3D2517`) — nunca preto puro.
4. **Não** use visual flat contemporâneo; o QuestUI vive de borda, textura e camadas.
5. **Não** misture sans-serif; o par Cinzel + Spectral é obrigatório.
6. **Faça** usar roxo real (`#581C87`) com parcimônia.
7. **Não** use branco `#FFFFFF` no texto; use pergaminho `#F5E6D3`.
8. **Faça** acento dourado no topo de cards importantes.
9. **Não** exagere animações; fades de 300ms.
10. **Faça** caixa alta e tracking em chips e labels pequenas (chips estão fora da vitrine desta fundação).
