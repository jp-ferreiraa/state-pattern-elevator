namespace ElevadorState.Elevator;

using ElevadorState.States;

/// <summary>
/// Context do padrão State.
/// Não conhece as regras de negócio de cada estado: apenas mantém o estado atual
/// e delega toda operação solicitada para ele. Os membros que permitem alterar
/// o andar ou trocar de estado diretamente são internos: só as próprias classes
/// de estado (que vivem no mesmo assembly) podem usá-los, evitando que qualquer
/// código externo viole as regras de negócio contornando os estados.
/// </summary>
public sealed class Elevador
{
    public const int AndarMinimo = 0;
    public const int AndarMaximo = 10;
    public const double CargaMaximaKg = 500.0;

    public int AndarAtual { get; private set; }
    public double CargaAtualKg { get; private set; }
    public ElevatorStateBase EstadoAtual { get; private set; }

    // Instâncias únicas de cada estado (evita recriar objetos a cada transição).
    // Internas: apenas as próprias classes de estado precisam referenciá-las
    // para solicitar transições.
    internal PortaAbertaState PortaAberta { get; } = new();
    internal PortaFechadaState PortaFechada { get; } = new();
    internal SubindoState Subindo { get; } = new();
    internal DescendoState Descendo { get; } = new();
    internal ManutencaoState Manutencao { get; } = new();
    internal EmergenciaState Emergencia { get; } = new();
    internal ExcessoPesoState ExcessoPeso { get; } = new();

    public Elevador()
    {
        AndarAtual = 0;
        CargaAtualKg = 0.0;
        EstadoAtual = PortaFechada; // Regra de negócio: começa no térreo, porta fechada.
    }

    /// <summary>
    /// Realiza a troca de estado e retorna uma mensagem descrevendo a transição.
    /// Chamado pelos próprios estados concretos — nunca pela camada de apresentação.
    /// </summary>
    internal string MudarEstado(ElevatorStateBase novoEstado)
    {
        EstadoAtual = novoEstado;
        return $">> Estado alterado para: {novoEstado.Nome}";
    }

    internal void IncrementarAndar() => AndarAtual++;

    internal void DecrementarAndar() => AndarAtual--;

    internal void DefinirAndar(int andar) => AndarAtual = andar;

    internal void AlterarCarga(double deltaKg)
    {
        CargaAtualKg = Math.Max(0.0, Math.Round(CargaAtualKg + deltaKg, 2));
    }

    // Operações do Context: cada uma delega imediatamente para o estado atual
    // e imprime a mensagem de resultado. Note que não existe if/switch aqui
    // verificando "qual é o estado" — quem decide o comportamento e a mensagem
    // é a própria classe de estado (polimorfismo); o Context só exibe o resultado.
    public void AbrirPorta() => Console.WriteLine(EstadoAtual.AbrirPorta(this));

    public void FecharPorta() => Console.WriteLine(EstadoAtual.FecharPorta(this));

    public void Subir() => Console.WriteLine(EstadoAtual.Subir(this));

    public void Descer() => Console.WriteLine(EstadoAtual.Descer(this));

    public void EntrarManutencao() => Console.WriteLine(EstadoAtual.EntrarManutencao(this));

    public void SairManutencao() => Console.WriteLine(EstadoAtual.SairManutencao(this));

    public void AcionarAlarme() => Console.WriteLine(EstadoAtual.AcionarAlarme(this));

    public void DesarmarAlarme() => Console.WriteLine(EstadoAtual.DesarmarAlarme(this));

    public void AdicionarPeso(double pesoKg) => Console.WriteLine(EstadoAtual.AdicionarPeso(this, pesoKg));

    public void RemoverPeso(double pesoKg) => Console.WriteLine(EstadoAtual.RemoverPeso(this, pesoKg));
}