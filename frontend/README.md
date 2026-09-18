# Frontend — Parking App

SPA autenticada da gestão de estacionamento. Textos em pt-BR. Tokens: [DESIGN.md](./DESIGN.md). Chamadas HTTP vão para `/api` (proxy Vite → `http://localhost:5080`) com `credentials: "include"`.

## Stack

- **React 19.3** + `react-router-dom`
- TypeScript
- Vite
- Node.js **20.19+** (22 LTS recomendado)

Identificadores de código (tipos, funções, variáveis, arquivos) são em **inglês**. Este README, `DESIGN.md`, textos da tela e nomes de teste são em **pt-BR**.

Rotas: `/login`, `/operacao`, `/vagas`, `/tarifas`, `/caixa`, `/historico`, `/relatorios`, `/usuarios`. Operador não vê cadastros administrativos.

## Comandos

Na pasta `frontend/`:

```bash
npm install
npm test
npm run dev
```

A interface sobe em **http://localhost:5173**. Suba o backend em `5080` para login e o pátio funcionarem.

`npm test` é o comando único da suíte (Vitest + cobertura). Cobertura mínima: **80% de linhas** do código em `src/` (exceto o bootstrap `main.tsx`).

## Testes

Convenção de nomes:

`<método>_<cenário>_<resultado esperado>`

Exemplos: `render_login_exibeCamposERecusaGenerica`, `render_vagas_operadorNaoAcessa`, `render_operacao_419px_acoesUtilizaveis`.

Os testes ficam em `tests/`.
