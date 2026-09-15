# Backend — Parking App

Serviço HTTP da fundação do Parking App. Sem regra de estacionamento nesta etapa.

## Stack

- SDK **.NET 10**
- ASP.NET Core Minimal APIs

Identificadores de código (tipos, funções, variáveis, arquivos) são em **inglês**. Este README, comentários de negócio e nomes de teste são em **pt-BR**.

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
