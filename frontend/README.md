# Frontend — Parking App

Interface da fundação do Parking App. Vitrine da identidade visual descrita em `DESIGN.md`. **Não consulta o backend.**

## Stack

- **React 19.3**
- TypeScript
- Vite
- Node.js **20.19+** (22 LTS recomendado)

Identificadores de código (tipos, funções, variáveis, arquivos) são em **inglês**. Este README, `DESIGN.md`, textos da tela e nomes de teste são em **pt-BR**.

Aparência: ver [DESIGN.md](./DESIGN.md).

## Comandos

Na pasta `frontend/`:

```bash
npm install
npm test
npm run dev
```

A interface sobe em **http://localhost:5173**. Rode com o backend desligado: a tela não depende do serviço.

`npm test` é o comando único da suíte (Vitest + cobertura). Cobertura mínima: **80% de linhas** do código em `src/` (exceto o bootstrap `main.tsx`).

## Testes

Convenção de nomes:

`<método>_<cenário>_<resultado esperado>`

Exemplo: `render_vitrineInicial_exibeTituloCardBotaoECampo`.

Os testes ficam em `tests/`.
