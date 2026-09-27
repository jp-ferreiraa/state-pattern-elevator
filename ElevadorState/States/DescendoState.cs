namespace ElevadorState.States;

using ElevadorState.Elevator;

/// <summary>
/// Estado: elevador descendo. Simétrico ao SubindoState: sabe como mover
/// o elevador um andar para baixo e quando encerrar o movimento.
/// </summary>
public sealed class DescendoState : ElevatorStateBase
{
    public override string Nome => "Descendo";

    public override string Subir(Elevador elevador) =>
        "Não é possível inverter o sentido: o elevador já está descendo.";

    public override string Descer(Elevador elevador)
    {
        elevador.DecrementarAndar();
        var chegada = $"Elevador descendo... chegou ao andar {elevador.AndarAtual}.";

        // Ao chegar, o elevador volta a ficar parado com a porta fechada.
        var transicao = elevador.MudarEstado(elevador.PortaFechada);

        return $"{chegada}\n{transicao}";
    }
}
