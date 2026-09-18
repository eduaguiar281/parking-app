# Quickstart: Gestão de estacionamento

Validação local das histórias P1. Contratos: [http-api.yaml](./contracts/http-api.yaml), [ui.md](./contracts/ui.md). Modelo: [data-model.md](./data-model.md).

## Pré-requisitos

- SDK .NET 10
- Node.js 20.19+ ou 22 LTS
- Navegador (desktop e um recorte estreito ~419px)

## Subir os dois lados

```bash
cd backend
dotnet test
dotnet run --project src
```

Em outro terminal:

```bash
cd frontend
npm install
npm test
npm run dev
```

- API: `http://localhost:5080` (`GET /alive` → `estou vivo`)
- UI: `http://localhost:5173` (proxy `/api` → 5080)
- Login inicial: `admin` ou `operador` / senha de `PARKING_BOOTSTRAP_PASSWORD` no `launchSettings` (local: `admin123`)

## Roteiro mínimo (P1)

1. **Login** — entrar como admin; tentar senha errada (mensagem genérica).
2. **Setores e vagas** — cadastrar Pátio A (motos) e Pátio B (carros); gerar lote P-A-001–P-A-003 e P-B-001–P-B-002; bloquear uma vaga.
3. **Tarifas** — carro Pátio B R$ 10 / R$ 5; moto Pátio A R$ 5 / R$ 3; vigência hoje.
4. **Caixa** — abrir o dia com valor de abertura; confirmar que um segundo aberto é recusado.
5. **Entrada** — placa `ABC-1234`, carro, Pátio B, vaga sugerida; painel mostra ocupada. Repetir a placa (recusa). Moto no Pátio B (recusa).
6. **Saída** — registrar saída com Pix; conferir resumo (primeira hora se < 1 h); vaga livre; receita no caixa; comprovante.
7. **Operador** — sair e entrar com `operador` (seed): sem Vagas/Tarifas/Usuários/Relatórios; URL `/usuarios` recusada.

## Esperado

- `dotnet test` e `npm test` passam; cobertura ≥ 80% de linhas em cada README.
- Totais do painel batem com as vagas e os estacionados (SC-002).
- Exemplos de cobrança da spec (SC-003) cobertos por teste de Domain sem SQLite.
- Persistência: reiniciar o `dotnet run` e os cadastros/estadias continuam.

## Fora deste guia

CI, Docker, impressora térmica, Pix real.
