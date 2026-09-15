# Quickstart: Fundações do monorepo

Validação local das três histórias. Sem CI. Backend e frontend sobem **separados**.

## Pré-requisitos

- SDK .NET 10 (`dotnet --version` começa com `10.`)
- Node.js 20.19+ ou 22 LTS (`node -v`)
- Navegador

## História 1 — Navegar o monorepo

1. Abrir a raiz do repositório.
2. Confirmar `backend/` e `frontend/` irmãos.
3. Abrir `backend/README.md` e `frontend/README.md`: pt-BR, stack, comandos, código em inglês, cobertura 80%, convenção de nomes de teste.

**Esperado:** SC-001 e SC-002.

## História 2 — Serviço vivo

Contrato: [contracts/alive.yaml](./contracts/alive.yaml)

```bash
cd backend
dotnet test
dotnet run --project src
```

Com o processo no ar, abrir `http://localhost:5080/alive`.

**Esperado:**
- Testes passam; cobertura de linhas ≥ 80%.
- Resposta 200, corpo exatamente `estou vivo`, em menos de 3 segundos.
- Sem o processo, o endereço não devolve esse texto.

## História 3 — Vitrine visual

Contrato: [contracts/showcase-ui.md](./contracts/showcase-ui.md). DESIGN.md: `frontend/DESIGN.md`.

```bash
cd frontend
npm install
npm test
npm run dev
```

Abrir `http://localhost:5173` **com o backend desligado**.

**Esperado:**
- Testes passam; cobertura de linhas ≥ 80%.
- Título, card, botão e campo em pt-BR no visual QuestUI.
- Nenhuma chamada de rede ao serviço.
- Revisor marca ≥ 8 regras do/don’t aplicáveis (SC-004).

## Independência (SC-005)

Rodar a história 2 sem o frontend e a história 3 sem o backend. As duas devem passar sozinhas.
