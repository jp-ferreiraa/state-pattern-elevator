namespace ElevadorState.States;

using ElevadorState.Elevator;

/// <summary>
/// Classe base abstrata para todos os estados do elevador (State, no padrão GoF).
/// Fornece implementação virtual padrão com mensagem genérica de bloqueio
/// para todas as operações, permitindo que cada estado concreto sobrescreva
/// apenas as operações que suporta ou que necessitam de mensagens específicas (DRY).
/// </summary>
public abstract class ElevatorStateBase
{
    /// <summary>Nome amigável do estado, usado para exibição.</summary>
    public abstract string Nome { get; }

    public virtual string AbrirPorta(Elevador elevador) =>
        $"Não é possível abrir a porta no estado '{Nome}'.";

    public virtual string FecharPorta(Elevador elevador) =>
        $"Não é possível fechar a porta no estado '{Nome}'.";

    public virtual string Subir(Elevador elevador) =>
        $"Não é possível subir no estado '{Nome}'.";

    public virtual string Descer(Elevador elevador) =>
        $"Não é possível descer no estado '{Nome}'.";

    public virtual string EntrarManutencao(Elevador elevador) =>
        $"Não é possível entrar em manutenção no estado '{Nome}'.";

    public virtual string SairManutencao(Elevador elevador) =>
        $"O elevador não está em manutenção (estado atual: '{Nome}').";

    public virtual string AcionarAlarme(Elevador elevador)
    {
        var transicao = elevador.MudarEstado(elevador.Emergencia);
        var descida = string.Empty;
        if (elevador.AndarAtual > Elevador.AndarMinimo)
        {
            elevador.DefinirAndar(Elevador.AndarMinimo);
            descida = "\nElevador descendo com urgência para o térreo (0)...";
        }
        return $"ALERTA DE INCÊNDIO ACIONADO!{descida}\nPortas abertas para evacuação no térreo.\n{transicao}";
    }

    public virtual string DesarmarAlarme(Elevador elevador) =>
        "O alarme de incêndio não está acionado.";

    public virtual string AdicionarPeso(Elevador elevador, double pesoKg) =>
        $"Não é possível adicionar carga no estado '{Nome}'. A porta deve estar aberta.";

    public virtual string RemoverPeso(Elevador elevador, double pesoKg) =>
        $"Não é possível remover carga no estado '{Nome}'. A porta deve estar aberta.";
}
