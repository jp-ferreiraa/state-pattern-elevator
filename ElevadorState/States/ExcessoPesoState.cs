namespace ElevadorState.States;

using ElevadorState.Elevator;

/// <summary>
/// Estado: excesso de peso. Acontece quando a carga ultrapassa o limite máximo.
/// O fechamento da porta é bloqueado até que o peso seja aliviado para um valor seguro.
/// </summary>
public sealed class ExcessoPesoState : ElevatorStateBase
{
    public override string Nome => "Excesso de Peso";

    public override string AbrirPorta(Elevador elevador) =>
        "A porta já está aberta.";

    public override string FecharPorta(Elevador elevador) =>
        $"BLOQUEADO: Limite de peso excedido ({elevador.CargaAtualKg:0.#} kg / máx {Elevador.CargaMaximaKg:0.#} kg)! Reduza o peso para fechar a porta.";

    public override string Subir(Elevador elevador) =>
        "Não é possível subir: elevador com excesso de peso e porta aberta.";

    public override string Descer(Elevador elevador) =>
        "Não é possível descer: elevador com excesso de peso e porta aberta.";

    public override string EntrarManutencao(Elevador elevador) =>
        "BLOQUEADO: Descarregue o excesso de peso antes de colocar o elevador em manutenção.";

    public override string AdicionarPeso(Elevador elevador, double pesoKg)
    {
        elevador.AlterarCarga(pesoKg);
        return $"Peso adicional de {pesoKg} kg adicionado. Carga atual: {elevador.CargaAtualKg:0.#} kg (Limite: {Elevador.CargaMaximaKg:0.#} kg).";
    }

    public override string RemoverPeso(Elevador elevador, double pesoKg)
    {
        elevador.AlterarCarga(-pesoKg);
        var info = $"Peso de {pesoKg} kg removido. Carga atual: {elevador.CargaAtualKg:0.#} kg (Limite: {Elevador.CargaMaximaKg:0.#} kg).";

        if (elevador.CargaAtualKg <= Elevador.CargaMaximaKg)
        {
            var transicao = elevador.MudarEstado(elevador.PortaAberta);
            return $"{info}\nPeso normalizado. O elevador voltou ao funcionamento regular.\n{transicao}";
        }

        return $"{info}\nAviso: O elevador ainda se encontra com excesso de peso.";
    }
}

