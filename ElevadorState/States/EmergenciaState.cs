namespace ElevadorState.States;

using ElevadorState.Elevator;

/// <summary>
/// Estado: emergência (alarme de incêndio acionado).
/// O elevador desce imediatamente até o térreo (andar 0), abre a porta
/// e permanece travado. Todas as demais operações ficam bloqueadas até
/// que o alarme seja desarmado.
/// </summary>
public sealed class EmergenciaState : ElevatorStateBase
{
    public override string Nome => "Emergência (Incêndio)";

    public override string AbrirPorta(Elevador elevador) =>
        "A porta já está aberta para evacuação de emergência.";

    public override string FecharPorta(Elevador elevador) =>
        "BLOQUEADO: A porta não pode ser fechada enquanto o alarme de incêndio estiver ativo.";

    public override string Subir(Elevador elevador) =>
        "BLOQUEADO: Operação não permitida em estado de emergência.";

    public override string Descer(Elevador elevador) =>
        "BLOQUEADO: O elevador já se encontra no térreo em modo de emergência.";

    public override string EntrarManutencao(Elevador elevador) =>
        "BLOQUEADO: Não é possível entrar em manutenção enquanto o alarme de incêndio estiver ativo.";

    public override string AcionarAlarme(Elevador elevador) =>
        "O alarme de emergência já se encontra acionado.";

    public override string DesarmarAlarme(Elevador elevador)
    {
        if (elevador.CargaAtualKg > Elevador.CargaMaximaKg)
        {
            var transicaoExcesso = elevador.MudarEstado(elevador.ExcessoPeso);
            return $"Alarme de emergência desarmado.\nATENÇÃO: Carga acima do limite seguro!\n{transicaoExcesso}";
        }

        var transicao = elevador.MudarEstado(elevador.PortaAberta);
        return $"Alarme de emergência desarmado. Elevador restabelecido no térreo com portas abertas.\n{transicao}";
    }

    public override string AdicionarPeso(Elevador elevador, double pesoKg) =>
        "BLOQUEADO: Não é permitido embarque ou adição de carga durante evacuação de emergência.";

    public override string RemoverPeso(Elevador elevador, double pesoKg)
    {
        elevador.AlterarCarga(-pesoKg);
        return $"Peso removido: {pesoKg} kg. Carga atual: {elevador.CargaAtualKg} kg. (Emergência ainda ativa)";
    }
}

