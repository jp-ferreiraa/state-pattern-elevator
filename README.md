# Sistema de Controle de Elevador — Padrão State (C#)

![.NET](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp&logoColor=white)
![GoF](https://img.shields.io/badge/GoF-State%20Pattern-blue)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)

Aplicação de console em C# que demonstra a aplicação do padrão de projeto **State (GoF)** através da simulação de um elevador, cujo comportamento muda completamente de acordo com o estado em que se encontra.

## Descrição

Um elevador é um exemplo natural de máquina de estados: as mesmas ações (abrir porta, subir e entrar em manutenção) precisam se comportar de forma diferente dependendo da condição atual do equipamento. Abrir a porta é uma operação válida quando o elevador está parado, mas deve ser recusada enquanto ele está em movimento ou em manutenção.

A forma mais comum, e também mais frágil, de resolver esse problema é utilizar uma variável de estado (`enum`) juntamente com blocos `if` ou `switch` espalhados pelos métodos, verificando qual é o estado atual antes de decidir qual ação executar. Esse tipo de solução tende a crescer mal, pois cada novo estado exige alterações em vários pontos do código, aumentando o risco de erros e violando o princípio Aberto/Fechado (Open/Closed Principle).

Neste projeto, o problema foi resolvido utilizando o padrão **State**. Cada estado do elevador é representado por uma classe independente, responsável apenas pelo seu próprio comportamento. O `Elevador` (Context) apenas delega a operação solicitada ao estado atual, deixando que o polimorfismo determine qual implementação será executada.

O projeto foi desenvolvido inicialmente como parte da disciplina de Programação Orientada a Objetos e, posteriormente, revisado e refinado para servir como um estudo de caso da aplicação do padrão State.

---

## Interface da aplicação

Abaixo está a interface principal da aplicação em execução.

![Interface da aplicação](assets/console.png)

---

## Tecnologias

- C# 12
- .NET 8
- Console Application (sem dependências externas)

---

## Conceitos aplicados

- **State Pattern (GoF)** — comportamento do elevador encapsulado em classes de estado intercambiáveis.
- **Polimorfismo** — o `Context` chama sempre os mesmos métodos da classe base `ElevatorStateBase`; a implementação executada depende apenas do objeto de estado referenciado em tempo de execução.
- **Encapsulamento** — os membros que alteram o estado interno do elevador (`MudarEstado`, `IncrementarAndar`, `DecrementarAndar` e as instâncias de cada estado) são `internal`. Nada fora do próprio mecanismo de estados pode manipular o elevador ignorando suas regras.
- **Abstração e Princípio DRY** — `ElevatorStateBase` define a estrutura comum e mensagens padrão de bloqueio para operações não permitidas, permitindo que cada estado concreto sobrescreva apenas as operações que suporta.
- **Baixo acoplamento** — nenhum estado conhece os demais diretamente; toda transição passa pelo `Elevador`, que expõe as instâncias de estado necessárias.
- **Responsabilidade única** — cada classe de estado responde apenas pelo comportamento do seu próprio estado.

---

## Estrutura do projeto

```text
ElevadorState/
├── Program.cs
├── Elevator/
│   └── Elevador.cs
├── States/
│   ├── ElevatorStateBase.cs
│   ├── PortaAbertaState.cs
│   ├── PortaFechadaState.cs
│   ├── SubindoState.cs
│   ├── DescendoState.cs
│   ├── ManutencaoState.cs
│   ├── EmergenciaState.cs
│   └── ExcessoPesoState.cs
└── assets/
    ├── console.png
    ├── demo.gif
    └── uml.png
```

- **`Elevator/Elevador.cs`** — o `Context` do padrão. Mantém o andar atual e o estado corrente, delegando toda operação ao estado ativo. Não contém regras de negócio.
- **`States/ElevatorStateBase.cs`** — classe abstrata base que define métodos virtuais com comportamento padrão de bloqueio (DRY).
- **`States/*.cs`** — implementações concretas dos estados do elevador.
- **`Program.cs`** — camada de apresentação responsável apenas pela interação com o usuário.

---

## Funcionamento do padrão State

O `Elevador` mantém uma referência ao estado atual através da propriedade `EstadoAtual`, do tipo `ElevatorStateBase`. Todas as operações públicas do elevador (`AbrirPorta()`, `Subir()` etc.) seguem o mesmo formato:

```csharp
public void AbrirPorta() => Console.WriteLine(EstadoAtual.AbrirPorta(this));
```

Não existe, em nenhum lugar do `Elevador`, uma verificação do tipo "se o estado é tal, faça isso". Quem decide o comportamento é sempre o objeto de estado associado no momento — o `Elevador` apenas delega a chamada.

Uma decisão de design importante foi fazer com que os métodos de `ElevatorStateBase` recebam o `Elevador` como parâmetro (`AbrirPorta(Elevador elevador)`), em vez de armazenar uma referência ao contexto em cada estado. Isso torna os estados objetos *stateless*, permitindo que o `Elevador` reutilize uma única instância de cada estado durante toda sua vida útil.

As transições de estado são sempre iniciadas pelo próprio estado concreto através de `elevador.MudarEstado(...)`, mantendo toda a lógica de mudança encapsulada nas classes responsáveis.

---

## Estados implementados

| Estado | Responsabilidade |
|---|---|
| `PortaAbertaState` | Elevador parado com porta aberta. Permite fechar a porta, adicionar/remover peso ou entrar em manutenção. Se a carga exceder o limite, transiciona para `ExcessoPesoState`. |
| `PortaFechadaState` | Estado de repouso. Permite abrir a porta, subir, descer ou entrar em manutenção. |
| `SubindoState` | Controla o movimento de subida e retorna ao estado de repouso após concluir a operação. |
| `DescendoState` | Controla o movimento de descida e retorna ao estado de repouso após concluir a operação. |
| `ManutencaoState` | Bloqueia todas as operações regulares, permitindo apenas sair da manutenção ou responder ao alarme de emergência. |
| `EmergenciaState` | Acionado via alarme de incêndio. Força o elevador a descer imediatamente ao andar 0 (térreo), abre as portas e trava os comandos de movimento e manutenção até o desarme. |
| `ExcessoPesoState` | Acionado quando a carga na cabine ultrapassa a capacidade máxima (500 kg). Bloqueia o fechamento da porta até que o peso seja aliviado para um nível seguro. |

---

## Extensibilidade e o Princípio Aberto/Fechado (OCP)

A introdução dos estados `EmergenciaState` e `ExcessoPesoState` comprova na prática o **Open/Closed Principle (OCP)** do SOLID:

1. **Aberto para Extensão**: Novos comportamentos complexos de segurança e capacidade foram integrados ao ecossistema criando novas classes que herdam de `ElevatorStateBase`.
2. **Fechado para Modificação**: O `Elevador` (Context) permaneceu completamente livre de condicionais (`if`/`switch`) baseadas em estado. Nenhuma regra de transição pré-existente foi corrompida, garantindo que cada estado mantenha isolamento rígido sobre suas próprias validações e reações. Além disso, através da classe base `ElevatorStateBase` (DRY), novos estados e operações podem ser adicionados sem exigir código repetitivo de mensagens de bloqueio em todos os outros estados.

---

## Fluxo de funcionamento

Quando o usuário solicita uma subida:

1. `Program.cs` chama `elevador.Subir()`.
2. O `Elevador` delega a chamada para `EstadoAtual.Subir(this)`.
3. `PortaFechadaState` valida se é possível subir.
4. O estado muda para `SubindoState`.
5. `SubindoState` realiza o movimento, incrementa o andar e retorna para `PortaFechadaState`.

Essa mesma chamada produz resultados completamente diferentes dependendo do estado atual, sem necessidade de estruturas condicionais no `Context`.

---

## Demonstração

A animação abaixo apresenta uma execução da aplicação demonstrando as principais transições de estado.

![Demonstração](assets/demo.gif)

---

## Diagrama UML

O diagrama abaixo representa a arquitetura utilizada para implementar o padrão State.

![Diagrama UML](assets/uml.png)

---

## Como executar

Pré-requisito:

- .NET 8 SDK instalado.

```bash
git clone https://github.com/SEU-USUARIO/state-pattern-elevator.git

cd state-pattern-elevator

dotnet run
```

---

## Possíveis melhorias futuras

- Configurar o andar máximo e a carga máxima através do construtor do `Elevador`.
- Escrever testes unitários para validar todas as transições de estado.

---

## Aprendizados

Este projeto permitiu compreender, na prática, como o padrão State elimina grandes estruturas condicionais e distribui o comportamento entre classes especializadas.

Durante a evolução do projeto também ficou evidente a importância do encapsulamento. Restringir métodos responsáveis por alterar o estado interno (`internal`) impediu que qualquer código externo pudesse violar as regras do elevador, tornando a implementação mais consistente e aderente aos princípios da orientação a objetos.

Além disso, revisar o próprio código após a conclusão do trabalho mostrou como pequenos refinamentos arquiteturais podem melhorar significativamente a qualidade de uma implementação sem alterar sua funcionalidade.

---

## Licença

Este projeto está licenciado sob a licença MIT. Consulte o arquivo **LICENSE** para mais informações.