# Backend — Parking App

Serviço HTTP da fundação do Parking App. Sem regra de estacionamento nesta etapa.

## Stack

- SDK **.NET 10**
- ASP.NET Core Minimal APIs
- Persistência: **EF Core** com **SQLite** (quando a spec tiver regra persistida)

Identificadores de código (tipos, funções, variáveis, arquivos) são em **inglês**. Este README, comentários de negócio e nomes de teste são em **pt-BR**.

## Arquitetura

Quando a spec tiver regra de negócio, a fatia entra no **mesmo** `csproj` (`ParkingApp.Api`). Não há três projetos Domain/Application/Infrastructure. Pastas (ou tipos com dono claro) bastam. Pastas vazias sem comportamento da spec atual não devem ser criadas.

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

Exemplo: `GetAlive_servicoEmExecucao_retornaEstouVivo`.

Os testes ficam em `tests/`.
