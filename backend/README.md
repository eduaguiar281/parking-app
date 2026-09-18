# Backend — Parking App

Serviço HTTP da gestão de estacionamento (setores, vagas, tarifas, caixa, estadias, relatórios).

## Stack

- SDK **.NET 10**
- ASP.NET Core Minimal APIs
- Persistência: **EF Core + SQLite** (`backend/parking.db`, ou `ConnectionStrings:Parking`)
- Sessão: cookie HTTP-only `ParkingApp.Session`
- PDF: QuestPDF (Community)

Identificadores de código (tipos, funções, variáveis, arquivos) são em **inglês**. Este README, comentários de negócio e nomes de teste são em **pt-BR**.

Administrador inicial (base vazia): login `admin`, senha de `PARKING_BOOTSTRAP_PASSWORD` (local: `admin123` no `launchSettings`).

A UI chama a API em `/api` (proxy do Vite em desenvolvimento). `GET /alive` permanece anônimo.

## Arquitetura

A fatia entra no **mesmo** `csproj` (`ParkingApp.Api`). Não há três projetos Domain/Application/Infrastructure. Pastas (ou tipos com dono claro) bastam.

Separação de responsabilidades:

| Camada | Responsabilidade | Fora |
|--------|------------------|------|
| **Domain** | Regras e invariantes (placa, vaga, estadia, tarifa), sem I/O | Arquivo, banco, HTTP, UI |
| **Application** | Caso de uso: orquestra domínio + persistência | Detalhe de armazenamento ou transporte |
| **Infrastructure** | Armazenamento e I/O — **EF Core + SQLite** | Decisão de placa, vaga, estadia ou tarifa |
| **API / UI** | Transporte HTTP (Minimal APIs) e tela | Regra de pátio |

O tipo que decide a regra não lê nem grava SQLite. Teste de domínio passa sem disco, rede ou host HTTP. Trocar o arquivo SQLite por outro meio não exige editar os tipos de Domain.

## Comandos

Na pasta `backend/`:

```bash
dotnet test
dotnet run --project src
```

O SDK 10 gera a solution `ParkingApp.slnx`. `dotnet test` na pasta `backend/` usa esse arquivo.

Com o serviço no ar, abra **http://localhost:5080/alive**. A resposta deve ser `200` com o texto `estou vivo`. Sem o processo, esse endereço não devolve sucesso.

`dotnet test` é o comando único da suíte. Ele aplica cobertura de **80% de linhas** (Coverlet).

## Testes

Convenção de nomes:

`<método>_<cenário>_<resultado esperado>`

Exemplos: `GetAlive_servicoEmExecucao_retornaEstouVivo`, `CreateSession_senhaInvalida_retorna401`, `Calculate_carro30min_soPrimeiraHora`.

Os testes ficam em `tests/`. Domain não usa SQLite. API usa `WebApplicationFactory` com arquivo SQLite temporário.
