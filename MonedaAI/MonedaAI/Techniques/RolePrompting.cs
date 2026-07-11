using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using MonedaAI.Configuration;

namespace MonedaAI.Techniques;

public class RolePrompting : IPromptingTechnique
{
    private readonly IChatClient _client;
    private readonly AssistantSettings _assistant;

    public RolePrompting(
        IChatClient client,
        IOptions<AssistantSettings> assistant)
    {
        _client = client;
        _assistant = assistant.Value;
    }

    public string Name => "Role Prompting";

    public async Task<string> ExecuteAsync(
        string userQuery,
        CancellationToken cancellationToken = default)
    {
        var systemPrompt = $$"""
            Actúa como un asesor de {{_assistant.Domain}} con experiencia
            en presupuestos personales.

            Sigue estas reglas:
            - Usa lenguaje sencillo, amable y claro.
            - No inventes información financiera.
            - Si hay ingresos y gastos, explica el balance.
            - Si existen varios montos, crea una tabla Markdown.
            - Ofrece sugerencias generales de organización y ahorro.
            - No recomiendes inversiones, préstamos ni decisiones financieras de riesgo.
            - Si faltan datos, indica qué información debe proporcionar el usuario.
            """;

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, userQuery)
        };

        var response = await _client.GetResponseAsync(
            messages,
            cancellationToken: cancellationToken);

        return response.Text ?? string.Empty;
    }
}