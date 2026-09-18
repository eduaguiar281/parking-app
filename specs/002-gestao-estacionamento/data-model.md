# Modelo de dados: Gestão de estacionamento

Persistência: SQLite via EF Core. Instante em UTC; UI em `America/Sao_Paulo`. Estadias concluídas guardam *snapshot* de setor, vaga e tarifa — alteração cadastral posterior não reescreve o histórico.

## Enumerações

| Nome | Valores |
|------|---------|
| UserRole | Administrator, Operator |
| UserStatus | Active, Inactive |
| SectorStatus | Active, Inactive |
| VehicleCategory | Car, Motorcycle, Both |
| VehicleType | Car, Motorcycle |
| SpotStatus | Free, Occupied, Blocked, Maintenance |
| TariffStatus | Active, Inactive |
| StayStatus | Active, Completed, Cancelled |
| PaymentMethod | Cash, Pix, DebitCard, CreditCard, Other |
| CashStatus | Open, Closed |
| CashMovementType | ExitPayment, Bleed, Supply, Adjustment |

## User

| Campo | Tipo | Regras |
|-------|------|--------|
| id | uuid | PK |
| fullName | string | obrigatório |
| login | string | único, case-insensitive; usuário ou e-mail |
| passwordHash | string | hasher do framework; nunca texto claro |
| role | UserRole | Administrator ou Operator |
| status | UserStatus | inativo não autentica |
| createdAt | DateTimeOffset | UTC |
| lastAccessAt | DateTimeOffset? | atualizado no login |

- **Ciclo:** criado ativo ou inativo → inativar/reativar. Sem exclusão física nesta feature.
- **Relacionamentos:** abre/fecha caixa; lança entradas, saídas e auditoria.

## Sector

| Campo | Tipo | Regras |
|-------|------|--------|
| id | uuid | PK |
| name | string | obrigatório |
| code | string | único |
| description | string? | opcional |
| allowedCategories | VehicleCategory | Car, Motorcycle ou Both |
| status | SectorStatus | inativo não recebe nova entrada |

- **Ciclo:** ativo ↔ inativo. Veículos já estacionados no setor inativo permanecem até a saída.

## Spot

| Campo | Tipo | Regras |
|-------|------|--------|
| id | uuid | PK |
| code | string | único no estabelecimento |
| sectorId | uuid | FK Sector |
| vehicleCategory | VehicleType | deve ser compatível com o setor (Car/Motorcycle ⊆ Both) |
| status | SpotStatus | Occupied só via estadia ativa |

- **Ciclo:** Free → Occupied (entrada) → Free (saída ou cancelamento). Free → Blocked ou Maintenance (admin); Occupied não muda para Blocked/Maintenance.
- **Lote:** prefixo + intervalo numérico com padding do fim (ex. 001–100). Se qualquer código já existir, o lote inteiro é recusado.
- **Disponível:** setor Active, status Free.

## TariffTable

| Campo | Tipo | Regras |
|-------|------|--------|
| id | uuid | PK |
| name | string | obrigatório |
| vehicleType | VehicleType | |
| sectorId | uuid? | null = tabela geral |
| firstHourAmount | decimal | ≥ 0, R$ |
| additionalHourAmount | decimal | ≥ 0, R$ |
| effectiveFrom | date | início inclusive |
| effectiveTo | date? | fim inclusive; null = aberta |
| status | TariffStatus | |

- **Conflito:** não coexistir duas ativas com o mesmo `(vehicleType, sectorId)` e vigência sobreposta (`sectorId` null é abrangência própria).
- **Escolha na entrada:** vigentes, ativas, tipo igual; **específica do setor vence a geral**.
- **Exclusão:** proibida se já referenciada por estadia; só inativar.
- **Snapshot:** nome e valores copiados para a estadia na entrada.

## ParkingStay (estadia / `Stay`)

| Campo | Tipo | Regras |
|-------|------|--------|
| id | uuid | PK |
| plate | string | maiúsculas; `ABC-1234` ou `ABC1D23` |
| vehicleType | VehicleType | |
| sectorId | uuid | FK (navegação); código/nome também em snapshot |
| sectorCodeSnapshot | string | |
| sectorNameSnapshot | string | |
| spotId | uuid | |
| spotCodeSnapshot | string | |
| tariffTableId | uuid | |
| tariffNameSnapshot | string | |
| firstHourAmountSnapshot | decimal | |
| additionalHourAmountSnapshot | decimal | |
| entryAt | DateTimeOffset | preenchido pelo sistema |
| exitAt | DateTimeOffset? | na conclusão |
| duration | TimeSpan? | na conclusão |
| amountCharged | decimal? | na conclusão; usa snapshots |
| paymentMethod | PaymentMethod? | na conclusão |
| cashRegisterId | uuid? | caixa aberto da saída |
| entryUserId | uuid | |
| exitUserId | uuid? | |
| status | StayStatus | |
| cancelReason | string? | só Cancelled |

- **Identidade ativa:** no máximo uma estadia Active por placa; no máximo uma Active por vaga.
- **Ciclo:** Active → Completed (saída paga, caixa aberto) ou Active → Cancelled (só admin, ainda não pago). Completed é imutável (valor e forma de pagamento).
- **Preço (Domain):** permanência ≤ 1 h → só primeira hora; senão primeira + adicional × `ceil(horas − 1)`. Exemplos da spec são o aceite.
- **Permanência noturna:** permitida; cobrança pelos snapshots; pagamento no único caixa aberto no momento da saída.

## CashRegister

| Campo | Tipo | Regras |
|-------|------|--------|
| id | uuid | PK |
| operationalDate | date | um registro por data |
| status | CashStatus | **no máximo um Open no sistema** |
| openingAmount | decimal | |
| openedAt | DateTimeOffset | |
| openedByUserId | uuid | |
| informedClosingAmount | decimal? | |
| expectedAmount | decimal? | ver fórmula |
| difference | decimal? | informado − esperado |
| closedAt | DateTimeOffset? | |
| closedByUserId | uuid? | |
| closingNotes | string? | |
| reopenReason | string? | última reabertura admin |

- **Esperado:** `abertura + Σ ExitPayment + Σ Supply − Σ Bleed + Σ Adjustment` (ajuste com sinal).
- **Ciclo:** Open → Closed. Reabrir (admin + motivo) → Open. Abrir nova data exige o Open atual Closed.
- **Entrada de veículo** não exige caixa; **saída paga** exige.

## CashMovement

| Campo | Tipo | Regras |
|-------|------|--------|
| id | uuid | PK |
| cashRegisterId | uuid | caixa Open no lançamento |
| type | CashMovementType | |
| amount | decimal | Bleed/Supply/ExitPayment ≥ 0; Adjustment com sinal |
| paymentMethod | PaymentMethod? | obrigatório em ExitPayment |
| reason | string? | obrigatório em Bleed, Supply, Adjustment |
| occurredAt | DateTimeOffset | |
| userId | uuid | |
| parkingStayId | uuid? | ExitPayment |

- ExitPayment nasce só da saída paga e **não se edita nem exclui**.
- Bleed, Supply, Adjustment: só Administrator, caixa Open.
- Caixa Closed: operador não mexe; admin não reescreve ExitPayment (usa sangria/suprimento/ajuste após reabrir).

## AuditEvent

| Campo | Tipo | Regras |
|-------|------|--------|
| id | uuid | PK |
| occurredAt | DateTimeOffset | |
| userId | uuid | |
| action | string | criação, alteração, inativação, abertura, fechamento, reabertura, sangria, suprimento, ajuste, entrada, saída, pagamento, cancelamento |
| entityType | string | |
| entityId | string | |
| changedData | string (JSON) | dados anteriores/novos relevantes |

Consulta só Administrator.

## Relacionamentos

```text
Sector 1 ── * Spot
Sector 0..1 ── * TariffTable
Spot 1 ── * ParkingStay
TariffTable 1 ── * ParkingStay
User 1 ── * ParkingStay (entrada / saída)
User 1 ── * CashRegister (abertura / fechamento)
CashRegister 1 ── * CashMovement
CashRegister 1 ── * ParkingStay (saídas pagas)
ParkingStay 0..1 ── 1 CashMovement (ExitPayment)
User 1 ── * AuditEvent
```

## Validação (Domain, sem I/O)

- Placa: normalizar maiúsculas; aceitar só `ABC-1234` e `ABC1D23`.
- Compatibilidade tipo × setor × vaga.
- Tarifa vigente: específica do setor senão geral; zero compatível → recusar entrada.
- Ocupação: ocupadas / (ocupadas + livres) no recorte; exclui Blocked, Maintenance e setor inativo; denominador 0 → 0%.
- Caixa esperado: fórmula acima.

## Fora de escopo

Mensalista, reserva, segundo estabelecimento, integração Pix/adquirente, correção de estadia paga, segundo motor de banco.
