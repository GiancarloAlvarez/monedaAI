using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using MonedaAI.Configuration;
using MonedaAI.Techniques;

public class ChainOfThought : IPromptingTechnique
{
    private readonly IChatClient _client;
    private readonly AssistantSettings _assistant;

    public ChainOfThought(
        IChatClient client,
        IOptions<AssistantSettings> assistant)
    {
        _client = client;
        _assistant = assistant.Value;
    }

    public string Name => "Chain-of-Thought";

    public async Task<string> ExecuteAsync(
        string userQuery,
        CancellationToken cancellationToken = default)
    {
        var prompt = $$"""
            Eres un asistente de {{_assistant.Domain}}.

            Analiza la información financiera del usuario y responde
            exactamente con la siguiente estructura:

            Análisis: resume los ingresos, gastos y categorías proporcionadas.
            Balance: calcula el dinero disponible si hay información suficiente.
            Recomendación: da una sugerencia simple para mejorar el control del dinero.
            Tabla: si hay montos, organízalos en una tabla Markdown.

            No inventes montos, fechas ni movimientos que el usuario no haya indicado.

            Consulta del usuario:
            {{userQuery}}
            """;

        var response = await _client.GetResponseAsync(
            prompt,
            cancellationToken: cancellationToken);

        return response.Text ?? string.Empty;
    }
}