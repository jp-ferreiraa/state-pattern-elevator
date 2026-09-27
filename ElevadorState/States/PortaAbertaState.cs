namespace ElevadorState.States;

using ElevadorState.Elevator;

/// <summary>
/// Estado: porta aberta. O elevador não pode se mover enquanto a porta
/// estiver aberta; a porta precisa ser fechada primeiro. Permite também
/// entrada e saída de passageiros/cargas (pesagem).
/// </summary>
public sealed class PortaAbertaState : ElevatorStateBase
{
    public override string Nome => "Porta Aberta";

    public override string AbrirPorta(Elevador elevador) =>
        "A porta já está aberta.";

    public override string FecharPorta(Elevador elevador)
    {
        var transicao = elevador.MudarEstado(elevador.PortaFechada);
        return $"Porta fechada.\n{transicao}";
    }

    public override string EntrarManutencao(Elevador elevador)
    {
        var transicao = elevador.MudarEstado(elevador.Manutencao);
        return $"Entrando em manutenção...\n{transicao}";
    }

    public override string AdicionarPeso(Elevador elevador, double pesoKg)
    {
        elevador.AlterarCarga(pesoKg);
        var info = $"Adicionado {pesoKg} kg. Carga atual: {elevador.CargaAtualKg:0.#} kg (Limite: {Elevador.CargaMaximaKg:0.#} kg).";

        if (elevador.CargaAtualKg > Elevador.CargaMaximaKg)
        {
            var transicao = elevador.MudarEstado(elevador.ExcessoPeso);
            return $"{info}\nALERTA: Carga máxima excedida! Fechamento de portas bloqueado.\n{transicao}";
        }

        return info;
    }

    public override string RemoverPeso(Elevador elevador, double pesoKg)
    {
        elevador.AlterarCarga(-pesoKg);
        return $"Removido {pesoKg} kg. Carga atual: {elevador.CargaAtualKg:0.#} kg (Limite: {Elevador.CargaMaximaKg:0.#} kg).";
    }
}
