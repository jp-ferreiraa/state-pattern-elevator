namespace ElevadorState.States;

using ElevadorState.Elevator;

/// <summary>
/// Estado: porta fechada, elevador parado. É o estado "de repouso": a partir
/// dele o elevador pode abrir a porta, iniciar um movimento ou entrar em manutenção.
/// </summary>
public sealed class PortaFechadaState : ElevatorStateBase
{
    public override string Nome => "Porta Fechada";

    public override string AbrirPorta(Elevador elevador)
    {
        var transicao = elevador.MudarEstado(elevador.PortaAberta);
        return $"Porta aberta.\n{transicao}";
    }

    public override string FecharPorta(Elevador elevador) =>
        "A porta já está fechada.";

    public override string Subir(Elevador elevador)
    {
        if (elevador.AndarAtual >= Elevador.AndarMaximo)
        {
            return $"Não é possível subir: o elevador já está no andar mais alto ({Elevador.AndarMaximo}).";
        }

        var transicao = elevador.MudarEstado(elevador.Subindo);

        // O deslocamento em si é responsabilidade do estado "Subindo": chamamos
        // diretamente a instância do novo estado, sem reentrar pelo Context.
        var chegada = elevador.Subindo.Subir(elevador);

        return $"Elevador iniciando subida...\n{transicao}\n{chegada}";
    }

    public override string Descer(Elevador elevador)
    {
        if (elevador.AndarAtual <= Elevador.AndarMinimo)
        {
            return "Não é possível descer: o elevador já está no térreo.";
        }

        var transicao = elevador.MudarEstado(elevador.Descendo);

        // O deslocamento em si é responsabilidade do estado "Descendo": chamamos
        // diretamente a instância do novo estado, sem reentrar pelo Context.
        var chegada = elevador.Descendo.Descer(elevador);

        return $"Elevador iniciando descida...\n{transicao}\n{chegada}";
    }

    public override string EntrarManutencao(Elevador elevador)
    {
        var transicao = elevador.MudarEstado(elevador.Manutencao);
        return $"Entrando em manutenção...\n{transicao}";
    }
}
