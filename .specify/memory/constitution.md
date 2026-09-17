<!--
Relatório de impacto de sincronização
- Mudança de versão: 1.2.0 → 1.3.0
- Princípios modificados: nenhum redefinido
- Seções adicionadas/expandidas:
  - V. Ortogonalidade: verificação de persistência vs. domínio
  - Precedência: recorte D/A/I não é arquitetura extra; YAGNI
    não autoriza I/O no tipo da regra
  - Fluxo: andaime vazio ≠ fatia sem Domain/Application/Infrastructure
  - Gate do Constitution Check em `/speckit-plan`
- Seções removidas: nenhuma
- TODOs de acompanhamento: o plano e o código de
  `002-controle-estacionamento` misturam JSON em `ParkingLot` —
  não conformes até recortar persistência para Infrastructure
-->

# Constituição do Parking App

## Princípios fundamentais

### I. YAGNI (You Aren't Gonna Need It)

DEVE implementar apenas o comportamento exigido pela spec aceita, pela tarefa
atual ou por um pedido explícito de produto. NÃO DEVE adicionar funcionalidade
especulativa, configuração sem uso, camadas extras ou abstrações justificadas
somente por necessidades futuras hipotéticas.

Justificativa: código sem uso aumenta o custo de mudança, a superfície de
revisão e o risco de defeito, sem entregar valor agora.

Verificação: todo tipo, módulo, flag ou ramificação nova DEVE corresponder a um
requisito atual. Se não corresponder, NÃO DEVE ser entregue.

### II. KISS (Keep It Simple, Stupid)

DEVE escolher o desenho mais simples que atenda o requisito atual e passe nos
testes exigidos. DEVE preferir fluxo de controle linear e legível a esperteza,
metaprogramação ou generalização prematura.

Justificativa: código simples é mais fácil de revisar, depurar e substituir.
Complexidade que o problema não exige é defeito de desenho.

Verificação: se dois desenhos atendem a spec, DEVE vencer o que tiver menos
peças, menos conceitos e menos indireção.

### III. DRY (Don't Repeat Yourself)

NÃO DEVE duplicar o mesmo conhecimento (regra de negócio, invariante,
mapeamento ou política) em vários lugares quando essas cópias teriam de mudar
juntas. DEVE extrair conhecimento compartilhado só quando a duplicação for real
e estável — não apenas código parecido.

NÃO DEVE criar uma abstração compartilhada só para unificar similaridade
acidental. Copiar algumas linhas que mudam por motivos diferentes é permitido.

Justificativa: conhecimento duplicado diverge; unificar à força código sem
relação cria acoplamento que depois viola KISS e YAGNI.

Verificação: mudar uma regra de negócio NÃO DEVE exigir caça a cópias
desconexas. Um helper compartilhado DEVE ter um único motivo nomeado para
existir.

### IV. Clean Code

DEVE escrever código que um colega entenda sem uma reunião de alinhamento:

- Nomes DEVEM revelar intenção (o quê e por quê, não detalhe de implementação).
- Funções DEVEM fazer uma coisa, em um nível de abstração, e permanecer
  pequenas.
- Código morto, trechos comentados e comentários enganosos NÃO DEVEM
  permanecer.
- Comentários DEVEM explicar intenção ou restrição não óbvia, nunca repetir o
  código.
- Módulos DEVEM manter estrutura clara e consistente e evitar efeitos colaterais
  ocultos.

Justificativa: código é lido muito mais do que é escrito. Código opaco não
pode ser alterado nem testado com segurança.

Verificação: um revisor que não fez a mudança DEVE conseguir afirmar o
propósito da unidade só pelos nomes e pela estrutura.

### V. Ortogonalidade

DEVE manter responsabilidades independentes: mudança em uma preocupação NÃO
DEVE forçar mudanças sem relação em outras. Módulos DEVEM se comunicar por
contratos explícitos (interfaces, eventos, dados), não por estado mutável
compartilhado e oculto.

NÃO DEVE vazar detalhes de armazenamento, transporte, UI ou framework para as
regras de domínio. NÃO DEVE acoplar funcionalidades independentes por estado
global, singletons escondidos ou imports transversais só por conveniência.

Justificativa: peças ortogonais podem ser entendidas, testadas e trocadas
isoladamente. Acoplamento oculto transforma cada mudança em edição de sistema
inteiro.

Verificação: alterar a funcionalidade A NÃO DEVE exigir edição na
funcionalidade B, a menos que o contrato de B dependa explicitamente de A.

Além disso, quando a spec tiver regra de negócio persistida (ex.: pátio que
sobrevive a restart):

- O tipo que decide placa, vaga, estadia ou tarifa NÃO DEVE ler nem gravar
  arquivo, banco, HTTP ou UI.
- Persistência DEVE viver em Infrastructure da mesma fatia.
- Teste da regra de domínio DEVE passar sem disco, rede ou host HTTP.
- Trocar a forma de guardar (arquivo ↔ outro meio) NÃO DEVE exigir edição
  nesses tipos de regra.

### VI. SOLID

DEVE aplicar SOLID onde isso reduz acoplamento e esclarece dono da mudança,
sem inventar tipos para variação que ainda não existe:

- Responsabilidade única: uma unidade DEVE ter um único motivo para mudar.
- Aberto/fechado: extensão DEVE ser possível sem alterar chamadores estáveis
  quando já existe variação real na spec.
- Substituição de Liskov: subtipos DEVEM honrar o contrato das abstrações.
- Segregação de interface: clientes NÃO DEVEM depender de métodos que não
  usam.
- Inversão de dependência: política de alto nível NÃO DEVE depender de detalhes
  de baixo nível; ambos DEVEM depender de abstrações que já existem no desenho.

NÃO DEVE introduzir interfaces, factories ou inversão de controle só porque
"o SOLID manda", quando há uma única implementação e nenhuma variação aceita.

Justificativa: SOLID protege costuras que já existem. Aplicado cedo, vira
arquitetura especulativa e perde para YAGNI e KISS.

Verificação: cada constructo SOLID DEVE corresponder a uma divisão de
responsabilidade ou variação atual na spec, não a uma variação prevista.

### VII. Testes (NÃO NEGOCIÁVEL)

DEVE cobrir todo comportamento desenvolvido com testes no escopo e na cobertura
mínima definidos no README daquele projeto. Se o README do projeto não definir
cobertura, a mudança NÃO DEVE ser considerada completa até o README ser
atualizado com uma regra explícita ou a lacuna ser registrada como exceção
aceita na spec.

Os testes DEVEM ser nomeados e descritos com a convenção:

`<método>_<cenário>_<resultado esperado>`

Exemplo: `calcularTarifa_periodoNoturno_retornaValorComAdicional`.

Os nomes DEVEM identificar a unidade sob teste, a condição específica e o
resultado observável. Nomes vagos (`test1`, `shouldWork`, `happyPath`) NÃO
DEVEM ser usados.

Justificativa: a cobertura definida no README torna o critério local e
revisável. A convenção de nomes torna falhas autoexplicativas no CI.

Verificação: comportamento de produção novo ou alterado, sem testes que
atendam a cobertura do README e a convenção de nomes, NÃO DEVE entrar.

## Precedência dos princípios

Quando os princípios colidirem, aplicar esta ordem e parar na primeira regra
decisiva:

**YAGNI > KISS > DRY > SOLID**

Regras de resolução:

1. YAGNI vence qualquer outro princípio de desenho. Não manter abstração,
   helper ou costura SOLID sem uso na spec atual.
2. KISS vence DRY e SOLID. Não extrair nem inverter dependências se o resultado
   for mais difícil de ler do que a duplicação ou a chamada concreta.
3. DRY vence SOLID. Compartilhar conhecimento que já existe antes de criar
   tipos novos para "preparar o futuro" de uma única implementação.
4. SOLID só se aplica depois que YAGNI, KISS e DRY estiverem satisfeitos.

Clean Code e Ortogonalidade são restrições permanentes: DEVEM ser aplicadas em
toda mudança.

Recortar Domain (regra), Application (caso de uso), Infrastructure (I/O) e API
(transporte) NÃO É arquitetura extra. É o mínimo da Ortogonalidade quando a
spec tem regra de negócio.

Arquitetura extra (YAGNI/KISS rejeitam): segundo meio de persistência sem
pedido, interface/repositório genérico com uma só implementação, três
projetos Domain/Application/Infrastructure, EF/banco que a spec não pediu.

YAGNI vence interface sem variação real. YAGNI NÃO vence colar
armazenamento, HTTP ou UI no tipo que contém a regra.

Se um desenho mais ortogonal exigir comportamento fora da spec (tela, campo,
endpoint a mais), YAGNI ainda vence — recortar dentro do escopo atual.

Qualquer exceção a esta ordem DEVE ser documentada na spec ou no plano, com o
princípio sobrescrito e o motivo.

## Idioma

Toda governança, documentação e artefato do Spec Kit neste repositório DEVE
ser escrito em português brasileiro (pt-BR). Isso inclui constitution, specs,
planos, tasks, checklists, READMEs, comentários que expliquem regra de negócio
e nomes de teste.

Identificadores de código (tipos, funções, variáveis) DEVEM seguir o idioma
definido no README de cada projeto; na ausência de regra local, DEVE-se usar
pt-BR também nesses nomes.

Textos de interface visíveis ao usuário DEVEM estar em pt-BR, salvo exceção
explícita na spec.

## Fluxo de desenvolvimento

Planejamento, implementação e revisão DEVEM tratar esta constitution como
filtro padrão de desenho:

- Specs e planos NÃO DEVEM propor componentes especulativos. YAGNI é o
  primeiro corte.
- A implementação DEVE preferir a solução óbvia. Complexidade exige
  justificativa escrita no plano.
- Duplicação SÓ DEVE ser removida quando for o mesmo conhecimento mudando pelo
  mesmo motivo.
- PRs DEVEM incluir testes que atendam a cobertura do README do projeto alvo e
  a convenção `<método>_<cenário>_<resultado esperado>`.
- Revisores DEVEM recusar mudanças que adicionem abstração sem uso, violem a
  precedência ou sejam entregues sem os testes exigidos.

O Constitution Check de `/speckit-plan` SÓ PODE marcar Ortogonalidade como
Pass se:

1. A regra de domínio for testável sem I/O.
2. Persistência exigida pela spec tiver dono em Infrastructure (não no tipo
   de regra).
3. Não houver pasta/projeto Domain/Application/Infrastructure sem
   comportamento nesta spec.

Se o plano misturar persistência na regra, o gate FALHA. Simplificar o
desenho ou registrar exceção na tabela de complexidade, com o princípio
sobrescrito e o motivo.

O Git DEVE seguir Trunk-Based Development. `main` é o trunk. Os branches DEVEM
ser de vida curta e voltar ao trunk rápido. Branches longos de funcionalidade
NÃO SÃO permitidos.

Os commits DEVEM ser pequenos e focados em uma mudança. DEVE-se preferir vários
commits pequenos a um commit misturado.

Os commits DEVEM seguir Conventional Commits (`feat:`, `fix:`, `docs:`,
`refactor:`, `test:`, `chore:`, …). O tipo e a descrição DEVEM corresponder à
mudança.

Os pull requests DEVEM ser semânticos: o título DEVE seguir Conventional
Commits. O corpo DEVE descrever o que mudou e por quê. Um PR sem essa
descrição NÃO DEVE ser mesclado.

Quando a spec da feature NÃO tiver regra de negócio, NÃO DEVEM ser criadas
pastas ou projetos vazios de Domain, Application ou Infrastructure (YAGNI).

Quando a spec TIVER regra de negócio, a implementação DEVE entregar a fatia
já recortada no mesmo branch curto e no mesmo projeto de aplicação:

- Domain: regras e invariantes, sem I/O
- Application: casos de uso (orquestra Domain + persistência)
- Infrastructure: armazenamento e detalhes de I/O
- API/UI: transporte e tela

Isso NÃO exige três assemblies. Pastas (ou tipos claramente donos de uma
preocupação) no csproj/app existentes bastam.

Andaime de design NÃO DEVE ser um PR separado só de pastas vazias, a menos
que tenha especificação própria.

Pull requests e verificações desta constitution DEVEM validar estes princípios.
Complexidade sem justificativa DEVE ser registrada na tabela de rastreamento de
complexidade do plano, ou a mudança DEVE ser simplificada.

A orientação de desenvolvimento em tempo de execução vive neste arquivo. Specs,
planos e tasks DEVEM permanecer consistentes com ele; em conflito, esta
constitution prevalece até ser emendada.

## Governança

Esta constitution prevalece sobre prática informal, preferência pessoal e
padrões ad hoc neste repositório.

Emendas:

- DEVEM ser escritas neste arquivo antes de serem tratadas como vinculantes.
- DEVEM atualizar `Última alteração` para a data da mudança e incrementar
  `Versão` com versionamento semântico:
  - MAJOR: remoção ou redefinição incompatível de um princípio ou da ordem de
    precedência.
  - MINOR: princípio novo ou regra materialmente expandida.
  - PATCH: esclarecimento, redação ou refinamento não semântico.
- DEVEM incluir justificativa curta na mudança (commit, PR ou spec) e nota de
  migração quando houver código conhecido em violação da nova regra.

Conformidade:

- PRs, revisões e planejamento Spec Kit (`/speckit.plan`, `/speckit.analyze`,
  `/speckit.implement`) DEVEM verificar alinhamento com estes princípios.
- Violações detectadas DEVEM ser corrigidas na mesma mudança ou registradas
  como exceção explícita e com prazo na spec.
- A precedência (YAGNI > KISS > DRY > SOLID) é o desempate; argumentos que a
  invertam sem emenda são não conformes.

**Versão**: 1.3.0 | **Ratificada**: 2026-09-15 | **Última alteração**: 2026-09-17
