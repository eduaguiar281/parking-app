# Design system — Parking App

Guia de interface do **Parking App**. O padrão visual é o console dark-only
do print de referência: menu lateral fixo, área de trabalho, cards de painel
e acento violeta.

**Stack de UI**: React 19 · Vite · TypeScript · CSS nativo (`tokens.css` +
classes em `app.css`). Sem Tailwind, sem biblioteca de componentes, sem
roteamento nesta fundação.

**Regra que governa o resto**: a interface é **dark-only**. Não existe tema
claro, não existe toggle. Toda cor assume fundo preto/zinc.

Os nomes de token abaixo são a fonte humana. `frontend/src/styles/tokens.css`
copia os valores em custom properties. Não invente hex fora desta lista.

---

## 1. Fundamentos

### 1.1 Paleta

| Papel | Token | Valor | Uso |
|---|---|---|---|
| Fundo da área de trabalho | `{colors.page}` | `#09090b` | main |
| Fundo do menu | `{colors.sidebar}` | `#000000` | aside |
| Superfície de painel | `{colors.surface}` | `rgb(24 24 27 / 0.55)` | cards |
| Superfície sólida | `{colors.surface-solid}` | `#18181b` | fundo de input |
| Superfície elevada | `{colors.surface-raised}` | `rgb(39 39 42 / 0.7)` | chips selecionados |
| Borda padrão | `{colors.border}` | `rgb(39 39 42 / 0.8)` | menu, inputs |
| Borda sutil | `{colors.border-subtle}` | `rgb(63 63 70 / 0.55)` | cards |
| Texto primário | `{colors.ink}` | `#f4f4f5` | títulos e valores |
| Texto de campo | `{colors.ink-field}` | `#e4e4e7` | valor digitado |
| Texto secundário | `{colors.ink-muted}` | `#a1a1aa` | rótulos, item de menu inativo |
| Texto terciário | `{colors.ink-subtle}` | `#71717a` | descrições |
| Texto de ajuda | `{colors.ink-hint}` | `#52525b` | seções do menu, placeholders |
| Sobre acento | `{colors.on-primary}` | `#ffffff` | marca e botão primário |
| Menu ativo (fundo) | `{colors.nav-active-bg}` | `rgb(139 92 246 / 0.15)` | item corrente |
| Menu ativo (borda) | `{colors.nav-active-border}` | `rgb(139 92 246 / 0.3)` | item corrente |
| Menu ativo (texto) | `{colors.nav-active-ink}` | `#ede9fe` | item corrente |

**Acento do produto**: **violeta**. `{colors.primary}` `#7c3aed` na marca e
nas ações primárias, `{colors.primary-hover}` `#8b5cf6` no hover,
`{colors.focus-ring}` `rgb(139 92 246 / 0.7)` em todo foco.

Não invente outra cor de marca. Cores de §1.2 são estado, não identidade.

### 1.2 Cores semânticas

Nunca escolha cor por gosto — cada uma carrega significado fixo. A vitrine
desta fundação **não as usa** em métricas inventadas; entram quando houver
status real.

| Significado | Token | Valor típico |
|---|---|---|
| Sucesso / concluído | `{colors.success}` | `#34d399` |
| Aviso / atenção / temporário | `{colors.warning}` | `#fcd34d` |
| Erro / bloqueio / rejeitado | `{colors.danger}` | `#fca5a5` |
| Em andamento / ativo | `{colors.primary}` | `#7c3aed` |
| Informativo | `{colors.info}` | `#38bdf8` |

**aviso = tente de novo** (falha transitória);
**erro = corrija** (o dado está errado).

### 1.3 Tipografia e escala

Família: `{typography.family}` = `Inter, system-ui, -apple-system, BlinkMacSystemFont, sans-serif`.

Densidade de console. Tamanhos abaixo de 14px são intencionais.

| Uso | Token | Tamanho | Peso | Cor |
|---|---|---|---|---|
| Título de página (topbar) | `{typography.title}` | 18px | 600 | `{colors.ink}` |
| Título de card | `{typography.card-title}` | 16px | 600 | `{colors.ink}` |
| Item de menu | `{typography.nav}` | 13px | 400 / 500 ativo | muted / nav-active |
| Corpo / campo | `{typography.body}` | 14px | 400 | `{colors.ink-field}` |
| Rótulo de campo | `{typography.label}` | 14px | 400 | `{colors.ink-muted}` |
| Descrição / lead | `{typography.lead}` | 14px | 400 | `{colors.ink-subtle}` |
| Ajuda / tag da marca | `{typography.hint}` | 12px | 400 | `{colors.ink-subtle}` |
| Seção do menu | `{typography.micro}` | 10px | 600 | `{colors.ink-hint}` |

Seção do menu: `uppercase`, `letter-spacing: 0.08em`. Título de página:
line-height 1.25. Corpo: 1.5. Item de menu: 1.35.

### 1.4 Forma e elevação

| Token | Valor | Uso |
|---|---|---|
| `{radius.card}` | 12px | cards |
| `{radius.control}` | 8px | inputs, botões, item de menu, marca |
| `{radius.badge}` | 6px | badges |
| `{radius.pill}` | 9999px | chips — **fora desta vitrine** |
| `{shadow.card}` | `inset 0 1px 0 rgb(255 255 255 / 0.04)` | highlight interno do painel |
| `{shadow.button}` | `0 10px 15px -3px rgb(76 29 149 / 0.3), 0 4px 6px -4px rgb(76 29 149 / 0.3)` | botão primário |
| `{shadow.focus}` | `0 0 0 2px {colors.page}, 0 0 0 4px {colors.focus-ring}` | foco visível |
| `{layout.sidebar}` | 256px | largura do menu no desktop |

**Espaçamento** (grade de 4px): `{space.xxs}` 4px · `{space.xs}` 8px ·
`{space.sm}` 12px · `{space.md}` 16px · `{space.lg}` 20px · `{space.xl}` 24px ·
`{space.xxl}` 32px · `{space.section}` 48px.

Menu: padding `{space.md}` / `{space.sm}`. Topbar e conteúdo: `{space.lg}`
vertical, `{space.xl}` horizontal.

### 1.5 Ícones

Tamanho 16px, `aria-hidden` se decorativos. Marca: quadrado 32px violeta com
ícone 16px. Sem biblioteca de ícones — SVG inline.

---

## 2. Shell e navegação

O console é **uma tela** (sem rota por feature). O chrome é fixo: menu à
esquerda e área de trabalho à direita. Uma visão nova **adiciona** item ao
menu; não cria página.

```
{ id, label, pageTitle }
label       → texto curto no menu (estreito)
pageTitle   → h1 no topbar (pode usar separador ·)
```

### 2.1 Marca

À esquerda, no topo do menu: marca violeta + nome do produto + tag
(`Console`). Nome `{typography.body}` / 600; tag `{typography.hint}`.

### 2.2 Seção

```
10px / 600 / uppercase / tracking 0.08em / {colors.ink-hint}
```

### 2.3 Item de menu

Largura total, `type="button"`, ícone 16px + rótulo 13px, `rounded-lg`,
`py 10px` / `px 12px`.

- **Ativo** (`aria-current="page"`): fundo `{colors.nav-active-bg}`, borda
  `{colors.nav-active-border}`, texto `{colors.nav-active-ink}`, peso 500.
- **Inativo**: texto `{colors.ink-muted}`; hover fundo zinc-800/50 e texto
  `{colors.ink}`.

Nesta fundação há **um** item (`Vitrine`). Não invente módulos de pátio,
ticket ou pagamento no menu.

### 2.4 Área de trabalho

Topbar só com o `h1`. Conteúdo abaixo, sem largura máxima de marketing —
o painel acompanha a coluna.

---

## 3. Componentes da vitrine

### 3.1 Card / painel

Como os blocos do print (cabeçalho + corpo), não como card de vitrine
centralizado:

```
fundo {colors.surface}
borda 1px {colors.border-subtle}
raio {radius.card}
sombra {shadow.card}
cabeçalho: padding {space.lg} {space.lg} 0
corpo: padding {space.lg}
```

Título do card: `{typography.card-title}`. Lead: `{typography.lead}`.

### 3.2 Botão primário

Uma ação primária por tela.

```
altura 40px · padding 8px 16px · raio {radius.control}
fundo {colors.primary} · texto {colors.on-primary}
hover {colors.primary-hover} · foco {shadow.focus}
desabilitado: opacidade 50%
```

Não use pílula. Não use `scale` no ativo.

### 3.3 Campo

O rótulo **envolve** o controle (`<label>` como contêiner).

```
rótulo: {typography.label}
input: raio {radius.control}
       borda 1px {colors.border}
       fundo {colors.surface-solid}
       padding 6px 12px
       texto {typography.body} / {colors.ink-field}
       placeholder {colors.ink-hint}
foco: {shadow.focus}
```

Quando o metadado de rótulo não existir, use o **nome técnico**. Nunca
exiba `null` ou campo sem rótulo. Não invente texto de ajuda.

---

## 4. Estados, texto e acessibilidade

### 4.1 Carregando, vazio e erro

Texto específico do domínio. Reticências de processo: `…`. Cada caixa de
estado oferece **no máximo uma ação**.

### 4.2 Idioma

Interface, mensagens e este documento em **pt-BR**. Mensagem de erro:
o que aconteceu e o que fazer. Sem detalhe interno do servidor.

### 4.3 Acessibilidade

- Foco sempre visível: `{shadow.focus}`.
- Estado nunca só por cor (`aria-current` no item ativo).
- `type="button"` em botão que não submete.
- `aria-hidden` em ícone decorativo; `aria-label` no `aside` do menu.

### 4.4 Regras não negociáveis

1. **Nunca `<button>` dentro de `<button>`.**
2. **Refetch nunca regride para "carregando" da primeira carga.**
3. **Nada posicionado com `absolute` dentro de tabela.**
4. **Descarte de resposta obsoleta** (id em `ref`).
5. **Nunca exibir zero no lugar de "não disponível".**
6. **Valor monetário é string decimal**, nunca `number`.

---

## 5. Do’s e don’ts (vitrine)

Faça:

- Shell de console: menu 256px à esquerda, área de trabalho `{colors.page}`.
- Item ativo com lavagem violeta, não com botão primário sólido no menu.
- Título de página 18px no topbar; marca “Parking App” só no menu.
- Card como painel de conteúdo (borda sutil, highlight interno).
- Botão e campo com `{radius.control}` (8px).
- Dark-only; textos em pt-BR; sem consulta ao backend.

Não faça:

- Card claro centralizado em página de marketing (hero, parchment, pílula).
- Tema claro, Action Blue, segundo acento de marca, gradiente.
- Largura máxima tipo landing (`max-width: 720px`) no conteúdo.
- Rota extra, catálogo de componentes, ou itens de menu de negócio
  ainda não especificados.
- Tailwind, Radix, Next.js ou biblioteca de ícones nesta fundação.
- Copiar textos, métricas ou nomes do console de referência (EngPlatform).

---

## 6. Checklist para uma tela nova

- [ ] Item no catálogo do menu (`id`, `label`, `pageTitle`); sem rota nova
- [ ] Superfícies com tokens de §1 e painel de §3
- [ ] Acento de marca só violeta; item ativo usa lavagem, não fill sólido
- [ ] Campos com `<label>` envolvente
- [ ] Rótulo ausente cai no nome técnico
- [ ] Estados de carregando/vazio/erro com texto do domínio, quando houver dados
- [ ] Cada caixa de estado oferece no máximo uma ação
- [ ] Nenhum `<button>` aninhado
- [ ] Textos em pt-BR; nenhum detalhe do servidor na tela
