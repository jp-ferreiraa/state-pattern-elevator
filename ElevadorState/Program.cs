using System.Text;
using ElevadorState.Elevator;
using ElevadorState.States;

// Garante que acentos (ã, ç, é...) apareçam corretamente no console,
// independentemente do codepage padrão do terminal do usuário.
Console.OutputEncoding = Encoding.UTF8;

var elevador = new Elevador();
var continuar = true;

while (continuar)
{
    DesenharTela(elevador);
    Console.Write("  Escolha uma opção: ");
    var opcao = Console.ReadLine();

    string mensagem;
    TipoMensagem tipo;

    switch (opcao)
    {
        case "1":
            (mensagem, tipo) = Executar(elevador.AbrirPorta);
            break;
        case "2":
            (mensagem, tipo) = Executar(elevador.FecharPorta);
            break;
        case "3":
            (mensagem, tipo) = Executar(elevador.Subir);
            break;
        case "4":
            (mensagem, tipo) = Executar(elevador.Descer);
            break;
        case "5":
            (mensagem, tipo) = Executar(elevador.EntrarManutencao);
            break;
        case "6":
            (mensagem, tipo) = Executar(elevador.SairManutencao);
            break;
        case "7":
            Console.Write("  Informe o peso a adicionar em kg [padrão: 100]: ");
            var pesoAddStr = Console.ReadLine();
            var pesoAdd = string.IsNullOrWhiteSpace(pesoAddStr) || !double.TryParse(pesoAddStr, out var pAdd) ? 100.0 : pAdd;
            (mensagem, tipo) = Executar(() => elevador.AdicionarPeso(pesoAdd));
            break;
        case "8":
            Console.Write("  Informe o peso a remover em kg [padrão: 100]: ");
            var pesoRemStr = Console.ReadLine();
            var pesoRem = string.IsNullOrWhiteSpace(pesoRemStr) || !double.TryParse(pesoRemStr, out var pRem) ? 100.0 : pRem;
            (mensagem, tipo) = Executar(() => elevador.RemoverPeso(pesoRem));
            break;
        case "9":
            (mensagem, tipo) = Executar(elevador.AcionarAlarme);
            break;
        case "10":
            (mensagem, tipo) = Executar(elevador.DesarmarAlarme);
            break;
        case "0":
            continuar = false;
            mensagem = "Encerrando o sistema do elevador...";
            tipo = TipoMensagem.Neutra;
            break;
        default:
            mensagem = "Opção inválida. Digite um número correspondente às opções do menu.";
            tipo = TipoMensagem.Invalida;
            break;
    }

    DesenharTela(elevador, (mensagem, tipo));

    if (continuar)
    {
        Console.Write("  Pressione ENTER para continuar...");
        Console.ReadLine();
    }
}

// ---------------------------------------------------------------------------
// A partir daqui: apenas apresentação (UI de console). Nenhum método abaixo
// decide comportamento do elevador — eles só chamam a API pública já existente
// de Elevador (AbrirPorta, Subir...) e formatam o resultado. A lógica do
// padrão State continua inteiramente dentro de Elevador/States.
// ---------------------------------------------------------------------------

const string Separador = "----------------------------------------------";

static void DesenharTela(Elevador elevador, (string texto, TipoMensagem tipo)? mensagem = null)
{
    LimparTela();

    EscreverTitulo();
    Console.WriteLine();
    EscreverInfoElevador(elevador);
    Console.WriteLine();
    EscreverMenu();
    Console.WriteLine();

    if (mensagem is not null)
    {
        EscreverMensagem(mensagem.Value.texto, mensagem.Value.tipo);
        Console.WriteLine();
    }
}

static void LimparTela()
{
    try
    {
        Console.Clear();
    }
    catch (IOException)
    {
        // Console.Clear() pode falhar quando a saída está redirecionada
        // (ex.: pipelines de CI). Nesses casos, simplesmente seguimos sem limpar.
    }
}

static void EscreverTitulo()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(Separador);
    Console.WriteLine("  SISTEMA DE CONTROLE DE ELEVADOR");
    Console.WriteLine(Separador);
    Console.ResetColor();
}

static void EscreverInfoElevador(Elevador elevador)
{
    // Estado e andar ficam sempre visíveis no topo — por isso o menu não
    // precisa mais de opções separadas para consultá-los.
    Console.Write("  Estado atual : ");
    if (elevador.EstadoAtual is EmergenciaState)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[EMERGÊNCIA] {elevador.EstadoAtual.Nome}");
    }
    else if (elevador.EstadoAtual is ExcessoPesoState)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"[SOBRECARGA] {elevador.EstadoAtual.Nome}");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(elevador.EstadoAtual.Nome);
    }
    Console.ResetColor();

    Console.Write("  Andar atual  : ");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(elevador.AndarAtual == 0 ? "0 (térreo)" : elevador.AndarAtual.ToString());
    Console.ResetColor();

    Console.Write("  Carga atual  : ");
    if (elevador.CargaAtualKg > Elevador.CargaMaximaKg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{elevador.CargaAtualKg:0.#} kg / {Elevador.CargaMaximaKg:0.#} kg [EXCESSO DE PESO]");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{elevador.CargaAtualKg:0.#} kg / {Elevador.CargaMaximaKg:0.#} kg");
    }
    Console.ResetColor();
}

static void EscreverMenu()
{
    Console.WriteLine(Separador);
    Console.WriteLine("  MENU");
    Console.WriteLine(Separador);
    Console.WriteLine("  1   Abrir porta");
    Console.WriteLine("  2   Fechar porta");
    Console.WriteLine("  3   Subir");
    Console.WriteLine("  4   Descer");
    Console.WriteLine("  5   Entrar em manutenção");
    Console.WriteLine("  6   Sair da manutenção");
    Console.WriteLine("  7   Adicionar peso");
    Console.WriteLine("  8   Remover peso");
    Console.WriteLine("  9   Acionar alarme de incêndio");
    Console.WriteLine("  10  Desarmar alarme de incêndio");
    Console.WriteLine("  0   Encerrar aplicação");
    Console.WriteLine(Separador);
}

static void EscreverMensagem(string texto, TipoMensagem tipo)
{
    Console.WriteLine("  MENSAGEM");
    Console.WriteLine(Separador);

    if (tipo == TipoMensagem.Sucesso) Console.ForegroundColor = ConsoleColor.Green;
    else if (tipo == TipoMensagem.Erro) Console.ForegroundColor = ConsoleColor.Red;
    else if (tipo == TipoMensagem.Invalida) Console.ForegroundColor = ConsoleColor.DarkYellow;

    foreach (var linha in texto.Split('\n'))
    {
        Console.WriteLine($"  {linha}");
    }

    Console.ResetColor();
}

// Executa uma operação do Elevador capturando o texto que ela escreve no
// console (Elevador continua responsável por gerar essa mensagem — ver
// Elevador.cs) e classifica o resultado apenas para fins de exibição.
static (string mensagem, TipoMensagem tipo) Executar(Action operacao)
{
    var saidaOriginal = Console.Out;
    using var saidaCapturada = new StringWriter();
    Console.SetOut(saidaCapturada);
    try
    {
        operacao();
    }
    finally
    {
        Console.SetOut(saidaOriginal);
    }

    var texto = saidaCapturada.ToString().TrimEnd('\r', '\n');
    return (texto, ClassificarMensagem(texto));
}

static TipoMensagem ClassificarMensagem(string texto)
{
    if (texto.Contains("BLOQUEADO") || texto.Contains("Não é possível") || texto.Contains("ALERTA:"))
    {
        return TipoMensagem.Erro;
    }

    return texto.Contains(">> Estado alterado para:") || texto.Contains("Porta") || texto.Contains("Elevador") || texto.Contains("Alarme") || texto.Contains("Peso") || texto.Contains("Adicionado") || texto.Contains("Removido")
        ? TipoMensagem.Sucesso
        : TipoMensagem.Erro;
}

enum TipoMensagem
{
    Sucesso,
    Erro,
    Invalida,
    Neutra,
}