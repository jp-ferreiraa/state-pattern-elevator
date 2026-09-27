namespace ElevadorState.States;

using ElevadorState.Elevator;

/// <summary>
/// Estado: elevador em manutenção. Bloqueia porta e movimento. A única
/// saída possível é encerrar a manutenção, retornando ao estado Porta Fechada.
/// </summary>
public sealed class ManutencaoState : ElevatorStateBase
{
    public override string Nome => "Em Manutenção";

    public override string SairManutencao(Elevador elevador)
    {
        var transicao = elevador.MudarEstado(elevador.PortaFechada);
        return $"Manutenção encerrada. Elevador pronto para uso.\n{transicao}";
    }
}
